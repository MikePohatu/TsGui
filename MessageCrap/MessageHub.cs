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
        #pragma warning disable CS0067
        public static event MessagingErrorLogHandler Fatal;
        public static event MessagingErrorLogHandler Error;
        public static event MessagingErrorLogHandler Warn;
        public static event MessagingLogHandler Info;
        public static event MessagingLogHandler Debug;
        public static event MessagingLogHandler Trace;
        #pragma warning restore CS0067

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
        /// List of callbacks by object.ID
        /// </summary>
        private static Dictionary<string, List<Action<Message>>> _objectCallbacks = new Dictionary<string, List<Action<Message>>>();

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

            Trace?.Invoke($"Sending new message: {message.ID} from {message.Sender}, topic: {message.Topic}");
            message.Sent = DateTime.Now;
            if (message.ResponseExpected) { message.StartTimer(); }

            InvokeMessageSubscriptions(message);

            //fire events
            MessageSent?.Invoke(message, new EventArgs());
        }

        /// <summary>
        /// Respond to a message. Always use this when respond to a received message as it will detect loops
        /// </summary>
        /// <param name="response"></param>
        public static void Respond(Message response)
        {
            if (response == null) { throw new MessagingException(response, "Response can't be null"); }
            if (response.RespondingTo == null) { throw new MessagingException(response, "Can't respond to a null message"); }

            //remove message from the pending list
            Message m;
            var pending= _pendingResponses;
            if (_pendingResponses.TryGetValue(response.RespondingTo.ID, out m) == true)
            {
                _pendingResponses.Remove(response.RespondingTo.ID);
                response.RespondingTo.StopTimer();
            }

            Trace?.Invoke($"Sending response message: {response.ID}, in reply to: {response.RespondingTo.ID}, topic: {response.Topic}");
            response.Sent = DateTime.Now;

            if (response.RootMessage == null) { throw new MessagingException(response, "Response has no root message"); }
            else
            {
                if (response.RootMessage.Sender == response.Sender || response.ChainIncludesSender(response.Sender))
                {
                    // message has come in a loop. kill it
                    Trace?.Invoke($"Closing looping message: {response.ID}");
                    LoopClosed?.Invoke(response, new EventArgs());
                    return;
                }
            }


            if (response.ResponseExpected) { response.StartTimer(); }
            InvokeMessageSubscriptions(response);

            //fire events
            MessageSent?.Invoke(response, new EventArgs());
            ResponseSent?.Invoke(response, new EventArgs());
        }

        private static void InvokeMessageSubscriptions(Message message)
        {
            //invoke topic subscribers
            List<ITopicSubscriber> subs;
            if (string.IsNullOrWhiteSpace(message.Topic) == false && _topicSubscribers.TryGetValue(message.Topic, out subs))
            {
                foreach (ITopicSubscriber sub in subs) { sub.OnTopicMessageReceivedAsync(message.Topic, message); }
            }

            //invoke object subscribers
            List<Action<Message>> callbacks;
            var temp = _objectCallbacks;
            ISubscribable sender = message.Sender as ISubscribable;
            if (string.IsNullOrWhiteSpace(sender?.ID) == false && _objectCallbacks.TryGetValue(sender.ID, out callbacks))
            {
                foreach (Action<Message> cb in callbacks) { cb(message); }
            }
        }

        /// <summary>
        /// Cancel message expecting response e.g. it has timed out
        /// </summary>
        /// <param name="message"></param>
        public static void CancelPending(Message message)
        {
            if (message == null) { throw new MessagingException(message, "Message can't be null"); }
            Trace?.Invoke($"Cancelling message: {message.ID}");
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
            string s = $"Message timed out waiting for response, ID: {message.ID}, topic: {message.Topic}, sender: {message.GetChainRoot().Sender.ToString()}";
            Warn?.Invoke(new Exception(s),s);
        }

        /// <summary>
        /// Create a message object. If not responding to an existing message use CreateMessage(sender, null)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="originalMessage"></param>
        /// <returns></returns>
        public static Message CreateMessage(object sender, Message originalMessage)
        {
            Message m = new Message();
            m.RespondingTo = originalMessage;
            if ((originalMessage != null) && (originalMessage.RootMessage == null))
            {
                m.RootMessage = originalMessage;
            } 
            else if (originalMessage != null)
            {
                m.RootMessage = originalMessage.RootMessage;
            }

            m.Sender = sender;
            Trace?.Invoke($"Created message: {m.ID}");
            return m;
        }

        /// <summary>
        /// Register a new subsciber
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="sub"></param>
        public static void Subscribe(string topic, ITopicSubscriber sub)
        {
            Debug?.Invoke($"Creating subscribtion to topic: {topic}");
            List<ITopicSubscriber> subs;
            if (_topicSubscribers.TryGetValue(topic, out subs))
            {
                subs.Add(sub);
            } 
            else
            {
                subs = new List<ITopicSubscriber>();
                subs.Add(sub);
                _topicSubscribers.Add(topic, subs);
            }
        }

        /// <summary>
        /// Register a new callback when a message is received from a sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="callback"></param>
        public static void Subscribe(ISubscribable sender, Action<Message> callback)
        {
            List<Action<Message>> subs;
            Debug?.Invoke($"Creating subscribtion to ID: {sender.ID}");
            if (_objectCallbacks.TryGetValue(sender.ID, out subs))
            {
                subs.Add(callback);
            }
            else
            {
                subs = new List<Action<Message>>();
                subs.Add(callback);
                _objectCallbacks.Add(sender.ID, subs);
            }
        }
    }
}
