#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace MessageCrap
{
    public static class MessageHub
    {
        public static event MessagingErrorLogHandler Fatal;
        public static event MessagingErrorLogHandler Error;
        public static event MessagingErrorLogHandler Warn;
        public static event MessagingLogHandler Info;
        public static event MessagingLogHandler Debug;
        public static event MessagingLogHandler Trace;

        /// <summary>
        /// MessageSent fires when any new message is sent, including responses
        /// </summary>
        public static event MessageSentHandler MessageSent;
        /// <summary>
        /// ResponseSent fires only when a response is sent
        /// </summary>
        public static event MessageSentHandler ResponseSent;
        /// <summary>
        /// LoopClosed fires only when a response is received by the original sender
        /// </summary>
        public static event MessageSentHandler LoopClosed;

        /// <summary>
        /// Lists of ITopicSubscribers by topic
        /// </summary>
        private static Dictionary<string, List<ITopicSubscriber>> _topicSubscribers = new Dictionary<string, List<ITopicSubscriber>>();

        /// <summary>
        /// Messages waiting for a reponse by Message.ID
        /// </summary>
        private static Dictionary<string, Message> _pendingResponses = new Dictionary<string, Message>();

        public static void Send(Message message)
        {
            if (message == null) { throw new MessagingException(message, "Message can't be null"); }
            if (message.RespondingTo != null) { throw new MessagingException(message, "Message sent in response. Use Respond() method"); }
            if (message.ResponseExpected)
            {
                Message m;
                if (_pendingResponses.TryGetValue(message.ID, out m) == false)
                {
                    _pendingResponses.Add(message.ID, message);
                }
                else
                {
                    throw new MessagingException(message, "Duplicate message ID detected");
                }
            }
            message.Sent = DateTime.Now;
            if (message.ResponseExpected) { message.StartTimer(); }

            //invoke topic subscribers
            List<ITopicSubscriber> subs;
            if (string.IsNullOrWhiteSpace(message.Topic) == false && _topicSubscribers.TryGetValue(message.Topic, out subs))
            {
                foreach(ITopicSubscriber sub in subs) { sub.OnTopicMessageReceived(message.Topic, message); }
            }

            //fire events
            MessageSent?.Invoke(message, new EventArgs());
        }

        /// <summary>
        /// Respond to a message. Always use this when respond to a received message as it will detect loops
        /// </summary>
        /// <param name="response"></param>
        /// <param name="originalMessage"></param>
        public static void Respond(Message response, Message originalMessage)
        {
            if (response == null) { throw new MessagingException(response, "Response can't be null"); }
            if (originalMessage == null) { throw new MessagingException(response, "Can't respond to a null message"); }

            //remove message from the pending list
            Message m;
            if (_pendingResponses.TryGetValue(originalMessage.ID, out m) == true)
            {
                _pendingResponses.Remove(originalMessage.ID);
            }

            response.Sent = DateTime.Now;
            if (response.ResponseExpected) { response.StartTimer(); }

            if (response.RootMessage == null) { throw new MessagingException(response, "Response has no root message"); }
            else
            {
                if (response.RootMessage.Sender == response.Sender)
                {
                    // message has come in a loop. kill it
                    LoopClosed?.Invoke(response, new EventArgs());
                    return;
                }
            }

            //invoke topic subscribers
            List<ITopicSubscriber> subs;
            if (string.IsNullOrWhiteSpace(response.Topic) == false && _topicSubscribers.TryGetValue(response.Topic, out subs))
            {
                foreach (ITopicSubscriber sub in subs) { sub.OnTopicMessageReceived(response.Topic, response); }
            }

            //fire events
            MessageSent?.Invoke(response, new EventArgs());
            ResponseSent?.Invoke(response, new EventArgs());
        }

        /// <summary>
        /// Cancel message expecting response e.g. it has timed out
        /// </summary>
        /// <param name="message"></param>
        public static void CancelPending(Message message)
        {
            if (message == null) { throw new MessagingException(message, "Message can't be null"); }

            //remove message from the pending list
            Message m;
            if (_pendingResponses.TryGetValue(message.ID, out m) == true)
            {
                _pendingResponses.Remove(message.ID);
            }
        }

        /// <summary>
        /// Report a message as timed out. Message hub will clean up and send logs if appropriate
        /// </summary>
        /// <param name="message"></param>
        public static void ReportTimeout(Message message)
        {
            CancelPending(message);
            string s = $"Message timed out waiting for response, topic: {message.Topic}";
            Warn?.Invoke(new Exception(s),s);
        }

        /// <summary>
        /// Create a message object. Only use this for new messages, not for responding to a previous message
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static Message CreateMessage(object sender)
        {
            Message m = new Message();
            m.RootMessage = null;
            m.Sender = sender;
            return m;
        }

        /// <summary>
        /// Create a message object in reponse to another message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="originalMessage"></param>
        /// <returns></returns>
        public static Message CreateResponse(object sender, Message originalMessage)
        {
            Message m = new Message();
            m.RespondingTo = originalMessage;
            m.RootMessage = originalMessage.RootMessage == null ? originalMessage : originalMessage.RootMessage;
            m.Sender = sender;
            return m;
        }
    }
}
