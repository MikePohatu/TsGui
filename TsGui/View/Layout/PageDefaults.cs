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

// PageDefaults.cs - Default settings for new TsPages.

namespace TsGui.View.Layout
{
    public class PageDefaults
    {
        public TsPageHeader PageHeader { get; set; }
        public TsButtons Buttons { get; set; }
        public IDirector RootController { get; set; }
        public TsMainWindow MainWindow { get; set; }
        public TsTable Table { get; set; }
        public TsPane LeftPane { get; set; }
        public TsPane RightPane { get; set; }
    }
}
