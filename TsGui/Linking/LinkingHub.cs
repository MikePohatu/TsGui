﻿#region license
// Copyright (c) 2025 Mike Pohatu
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

// LinkableLibrary.cs - stores IOptions against their ID

using MessageCrap;
using System.Collections.Generic;
using Core.Diagnostics;
using Core.Logging;
using TsGui.Options;

namespace TsGui.Linking
{
    public class LinkingHub: ITopicSubscriber
    {
        private Dictionary<string, IOption> _sources = new Dictionary<string, IOption>();
        private static LinkingHub _instance = new LinkingHub();
        public static LinkingHub Instance { get { return _instance; } }
        private LinkingHub()
        {
            MessageHub.Subscribe(Topics.ReprocessRequest, this);
        }


        /// <summary>
        /// Remove all styles for reload
        /// </summary>
        public void Reset()
        {
            this._sources.Clear();
        }

        public IOption GetSourceOption(string ID)
        {
            if (string.IsNullOrWhiteSpace(ID)) { return null; }

            IOption option;
            this._sources.TryGetValue(ID, out option);
            return option;
        }

        public void AddSource(IOption NewSource)
        {
            IOption testoption;

            if (this._sources.TryGetValue(NewSource.ID, out testoption) == true ) { throw new KnownException("Duplicate ID found in LinkingLibrary: " + NewSource.ID,""); }
            else { this._sources.Add(NewSource.ID,NewSource); }
        }

        public async void OnTopicMessageReceived(string topic, Message message)
        {
            switch (topic)
            {
                case Topics.ReprocessRequest:
                    IOption option = this.GetSourceOption(message.Payload as string);
                    if (option != null)
                    {
                        await option.UpdateLinkedValueAsync(message);
                    }
                    break;
                default:
                    break;
            }
        }

        public Message SendUpdateMessage(ILinkSource source, Message message)
        {
            if (message == null)
            {
                Log.Trace($"New update message create, no response. Source ID: {source?.ID}");
            }
            return MessageHub.CreateMessage(source, message).SetTopic(Topics.SourceValueChanged).SetPayload(source.CurrentValue).Send();
        }

        public Message SendReprocessRequestMessage(object sender, string id, Message message)
        {
            return MessageHub.CreateMessage(sender, message).SetTopic(Topics.ReprocessRequest).SetPayload(id).SetResponseExpected(true).Send();
        }

        public void RegisterLinkTarget(ILinkTarget target, ILinkSource source)
        {
            if (target == null) { 
                if (source == null) { throw new KnownException($"Error registering target. Target is null", null); }
                throw new KnownException($"Error registering target. Target is null, source: {source.ID}", null);
            }
            if (source == null) { 
                throw new KnownException("Error registering target. Source is null. ", null);            
            }
            MessageHub.Subscribe(source, (Message message) =>
            {
                target.OnSourceValueUpdatedAsync(message);
            });
        }
    }
}
