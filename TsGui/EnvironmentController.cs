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

// EnvironmentController.cs - responsible for getting the right information from the right connector
// in the right format so it can be passed back to the MainController. 

using System.Xml.Linq;
using System.Collections.Generic;
using System;

using TsGui.Queries;
using TsGui.Validation;
using TsGui.Connectors;
using TsGui.Linking;
using TsGui.Diagnostics.Logging;
using TsGui.Diagnostics;

namespace TsGui
{
    public class EnvironmentController
    {
        public enum ConnectorType { ConfigMgr, Test, Registry };

        private IVariableOutput _outputconnector;
        private SccmConnector _sccmconnector;
        private ConnectorType _type = ConnectorType.ConfigMgr;

        public ConnectorType OutputType { get { return this._type; } }
        public SccmConnector SccmConnector { get { return this._sccmconnector; } }
        public IVariableOutput OutputConnector { get { return this._outputconnector; } }

        public bool Init()
        {
            return this.CreateConnector();
        }

        private bool CreateConnector()
        {
            switch(this._type)
            {
                case ConnectorType.ConfigMgr:
                    try
                    {
                        this._sccmconnector = new SccmConnector();
                        this._outputconnector = this._sccmconnector;
                        return true;
                    }
                    catch
                    {
                        LoggerFacade.Trace("Couldn't create SCCM connector. Creating testing connector");
                        this._outputconnector = new TestingConnector();
                        return false;
                    }
                case ConnectorType.Test:
                    this._outputconnector = new TestingConnector();
                    return false;
                case ConnectorType.Registry:
                    this._outputconnector = new RegistryConnector();
                    return true;
                default:
                    throw new TsGuiKnownException("Invalid connector type specified", null);
            }
        }

        public void AddVariable(Variable Variable)
        {
            this._outputconnector.AddVariable(Variable);
        }

        //release the output connectors.
        public void Release()
        {
            this._outputconnector.Release();
        }

        public void SetOutputType(string type)
        {
            switch (type.ToLower())
            {
                case "sccm":
                case "configmgr":
                    this._type = ConnectorType.ConfigMgr;
                    break;
                case "test":
                    this._type = ConnectorType.Test;
                    break;
                case "registry":
                case "reg":
                    this._type = ConnectorType.Registry;
                    break;
                default:
                    LoggerFacade.Warn("Invalid OutputType set, defaulting to ConfigMgr");
                    this._type = ConnectorType.ConfigMgr;
                    break;
            }

        }
    }
}
