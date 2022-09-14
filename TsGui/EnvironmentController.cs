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
using Core.Logging;
using Core.Diagnostics;
using TsGui.Config;

namespace TsGui
{
    public static class EnvironmentController
    {
        public enum ConnectorType { ConfigMgr, Test, Registry };

        private static IVariableOutput _outputconnector;
        private static SccmConnector _sccmconnector;
        private static ConnectorType _type = ConnectorType.ConfigMgr;

        public static ConnectorType OutputType { get { return _type; } }
        public static SccmConnector SccmConnector { get { return _sccmconnector; } }
        public static IVariableOutput OutputConnector { get { return _outputconnector; } }

        public static bool Init()
        {
            SetOutputType(TsGuiRootConfig.OutputType);
            return CreateConnector();
        }

        private static bool CreateConnector()
        {
            switch(_type)
            {
                case ConnectorType.ConfigMgr:
                    try
                    {
                        _sccmconnector = new SccmConnector();
                        _outputconnector = _sccmconnector;
                        _sccmconnector.Init();
                        return true;
                    }
                    catch
                    {
                        Log.Trace("Couldn't create SCCM connector. Creating testing connector");
                        _outputconnector = new TestingConnector();
                        return false;
                    }
                case ConnectorType.Test:
                    _outputconnector = new TestingConnector();
                    return false;
                case ConnectorType.Registry:
                    _outputconnector = new RegistryConnector();
                    return true;
                default:
                    throw new KnownException("Invalid connector type specified", null);
            }
        }

        public static void AddVariable(Variable Variable)
        {
            _outputconnector.AddVariable(Variable);
        }

        //release the output connectors.
        public static void Release()
        {
            _outputconnector.Release();
        }

        public static void SetOutputType(string type)
        {
            switch (type.ToLower())
            {
                case "sccm":
                case "configmgr":
                    _type = ConnectorType.ConfigMgr;
                    break;
                case "test":
                    _type = ConnectorType.Test;
                    break;
                case "registry":
                case "reg":
                    _type = ConnectorType.Registry;
                    break;
                default:
                    Log.Warn("Invalid OutputType set, defaulting to ConfigMgr");
                    _type = ConnectorType.ConfigMgr;
                    break;
            }
        }
    }
}
