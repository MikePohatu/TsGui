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

using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.Linking
{
    public interface ILinkSource: ISubscribable
    {
        /// <summary>
        /// The current value of the source
        /// </summary>
        string CurrentValue { get; }

        /// <summary>
        /// Request a link source to update the value set on the option.
        /// </summary>
        Task UpdateValueAsync(Message message);
    }
}
