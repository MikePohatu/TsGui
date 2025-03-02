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

// ITsVariableOutput.cs - provides a common type for testingconnector and sccmconnector

namespace TsGui
{
    public interface IVariableOutput
    {
        void AddVariable(Variable Variable);

        /// <summary>
        /// 'Release' the options to their output, i.e. write the values to the destination
        /// </summary>
        void Release();

        /// <summary>
        /// Initialise things before starting up, e.g. hide the TS progress dialog
        /// </summary>
        void Init();
    }
}
