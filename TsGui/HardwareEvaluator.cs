#region license
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

// HardwareEvaluator.cs - class to identify hardware type
// ChassisTypes - https://technet.microsoft.com/en-us/library/ee156537.aspx
// updated: https://blogs.technet.microsoft.com/brandonlinton/2017/09/15/updated-win32_systemenclosure-chassis-types/ 

using Core.Logging;
using System;
using System.Collections.Generic;
using System.Management;
using System.Xml.Linq;
using TsGui.Connectors;
using TsGui.Options.NoUI;
using TsGui.Options;
using System.Threading.Tasks;

namespace TsGui
{
    public static class HardwareEvaluator
    {
        private static string _namespace = @"root\CIMV2";
        private static string _path = null;

        public static bool IsLaptop { get; private set; } = false;
        public static bool IsDesktop { get; private set; } = false;
        public static bool IsServer { get; private set; } = false;
        public static bool IsVirtualMachine { get; private set; } = false;
        public static bool IsConvertible { get; private set; } = false;
        public static bool IsDetachable { get; private set; } = false;
        public static bool IsTablet { get; private set; } = false;
        public static string IPAddresses4 { get; private set; } = string.Empty;
        public static string IPAddresses6 { get; private set; } = string.Empty;
        public static string IPNetMask4 { get; private set; } = string.Empty;
        public static string IPNetMask6 { get; private set; } = string.Empty;
        public static string DefaultGateways4 { get; private set; } = string.Empty;
        public static string DefaultGateways6 { get; private set; } = string.Empty;
        public static string DHCPServer { get; private set; } = string.Empty;

        /// <summary>
        /// Query WMI and populate the data for the TsGui_xxx variables
        /// </summary>
        public static async Task InitAsync(XElement inputxml)
        {
            var x = inputxml.Element("HardwareEval");
            if (x != null)
            {
                _path = XmlHandler.GetStringFromXml(x, "Path", _path);
                if (string.IsNullOrWhiteSpace(_path)) { _path = XmlHandler.GetStringFromXml(inputxml, "Path", _path); }
                if (string.IsNullOrWhiteSpace(_path)) { _path = Director.Instance.DefaultPath; }

                Log.Debug("Running hardware evaluator");
                Evaluate();
                foreach (Variable var in GetTsVariables())
                {
                    NoUIOption newhwoption = new NoUIOption();
                    await newhwoption.ImportFromTsVariableAsync(var);
                    OptionLibrary.Add(newhwoption);
                    if (var.Name.StartsWith("TsGui_Is") == false) { continue; }

                    newhwoption.AddToggle(var.Name, false, false);
                    newhwoption.AddToggle($"{var.Name}_Hide", true, false);
                    newhwoption.AddToggle($"{var.Name}_Invert", false, true);
                    newhwoption.AddToggle($"{var.Name}_Hide_Invert", true, true);
                }
            }
        }

        public static List<Variable> GetTsVariables()
        {
            List<Variable> vars = new List<Variable>();

            if (IsLaptop) { vars.Add(new Variable("TsGui_IsLaptop", "TRUE", _path)); }
            else { vars.Add(new Variable("TsGui_IsLaptop", "FALSE", _path)); }

            if (IsDesktop) { vars.Add(new Variable("TsGui_IsDesktop", "TRUE", _path)); }
            else { vars.Add(new Variable("TsGui_IsDesktop", "FALSE", _path)); }

            if (IsServer) { vars.Add(new Variable("TsGui_IsServer", "TRUE", _path)); }
            else { vars.Add(new Variable("TsGui_IsServer", "FALSE", _path)); }

            if (IsVirtualMachine) { vars.Add(new Variable("TsGui_IsVirtualMachine", "TRUE", _path)); }
            else { vars.Add(new Variable("TsGui_IsVirtualMachine", "FALSE", _path)); }

            if (IsConvertible) { vars.Add(new Variable("TsGui_IsConvertible", "TRUE", _path)); }
            else { vars.Add(new Variable("TsGui_IsConvertible", "FALSE", _path)); }

            if (IsDetachable) { vars.Add(new Variable("TsGui_IsDetachable", "TRUE", _path)); }
            else { vars.Add(new Variable("TsGui_IsDetachable", "FALSE", _path)); }

            if (IsTablet) { vars.Add(new Variable("TsGui_IsTablet", "TRUE", _path)); }
            else { vars.Add(new Variable("TsGui_IsTablet", "FALSE", _path)); }

            vars.Add(new Variable("TsGui_IPv4", IPAddresses4, _path));
            vars.Add(new Variable("TsGui_IPv6", IPAddresses6, _path));
            vars.Add(new Variable("TsGui_DefaultGateway4", DefaultGateways4, _path));
            vars.Add(new Variable("TsGui_DefaultGateway6", DefaultGateways6, _path));
            vars.Add(new Variable("TsGui_IPSubnetMask4", IPNetMask4, _path));
            vars.Add(new Variable("TsGui_IPSubnetMask6", IPNetMask6, _path));
            vars.Add(new Variable("TsGui_DHCPServer", DHCPServer, _path));

            return vars;
        }

        private static void Evaluate()
        {
            //virtual machine tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(_namespace, "select Model,Manufacturer from Win32_ComputerSystem"))
            {
                string model = (string)m["Model"];
                string maker = (string)m["Manufacturer"];
                if (string.IsNullOrWhiteSpace(model) == false)
                {
                    //vmware
                    if (model.Contains("VMware"))
                    {
                        IsVirtualMachine = true;
                        break;
                    }
                    //hyper-v
                    if (model == "Virtual Machine")
                    {
                        IsVirtualMachine = true;
                        break;
                    }
                    //virtualbox
                    if (model.Contains("VirtualBox"))
                    {
                        IsVirtualMachine = true;
                        break;
                    }
                    //Parallels
                    if (model.Contains("Parallels"))
                    {
                        IsVirtualMachine = true;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(maker) == false)
                {
                    //Xen
                    if (maker.Contains("Xen"))
                    {
                        IsVirtualMachine = true;
                        break;
                    }
                }
            }

            //chassis type tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(_namespace, "select ChassisTypes from Win32_SystemEnclosure"))
            {
                Int16[] chassistypes = (Int16[])m["ChassisTypes"];

                foreach (Int16 i in chassistypes)
                {
                    switch (i)
                    {
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 14:
                        case 18:
                        case 21:
                            {
                                IsLaptop = true;
                                break;
                            }
                        case 30:
                            {
                                IsLaptop = true;
                                IsTablet = true;
                                break;
                            }
                        case 31:
                            {
                                IsLaptop = true;
                                IsConvertible = true;
                                break;
                            }
                        case 32:
                            {
                                IsLaptop = true;
                                IsDetachable = true;
                                break;
                            }
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 15:
                        case 16:
                        case 24:
                        case 34:
                        case 35:
                        case 36:
                            {
                                IsDesktop = true;
                                break;
                            }
                        case 23:
                        case 25:
                        case 28:
                        case 29:
                            {
                                IsServer = true;
                                break;
                            }
                        default:
                            { break; }
                    }
                }
            }

            //ip info gather
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(_namespace, @"Select DefaultIPGateway,IPAddress,IPSubnet,DHCPServer from Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'True'"))
            {
                string[] ipaddresses = (string[])m["IPAddress"];

                if (ipaddresses != null)
                {
                    foreach (string s in ipaddresses)
                    {
                        if (string.IsNullOrEmpty(s) == false)
                        {
                            if (s.Contains(":")) { IPAddresses6 = AppendToStringList(IPAddresses6, s); }
                            else { IPAddresses4 = AppendToStringList(IPAddresses4, s); }
                        }
                    }
                }


                string[] defaultgateways = (string[])m["DefaultIPGateway"];
                if (defaultgateways != null)
                {
                    foreach (string s in defaultgateways)
                    {
                        if (s.Contains(":")) { DefaultGateways6 = AppendToStringList(DefaultGateways6, s); }
                        else { DefaultGateways4 = AppendToStringList(DefaultGateways4, s); }
                    }
                }

                string[] netmasks = (string[])m["IPSubnet"];
                if (netmasks != null)
                {
                    foreach (string s in netmasks)
                    {
                        if (s.Contains(".")) { IPNetMask4 = AppendToStringList(IPNetMask4, s); }
                        else { IPNetMask6 = AppendToStringList(IPNetMask6, s); }
                    }
                }

                string svr = (string)m["DHCPServer"];
                if (string.IsNullOrWhiteSpace(svr) == false) { DHCPServer = AppendToStringList(DHCPServer, svr); }
            }
        }

        private static string AppendToStringList(string basestring, string newstring)
        {
            string s;
            if (string.IsNullOrWhiteSpace(basestring)) { s = newstring; }
            else { s = basestring + ", " + newstring; }
            return s;
        }

        public static void Reset()
        {
            _namespace = @"root\CIMV2";
            _path = null;

            IsLaptop = false;
            IsDesktop = false;
            IsServer = false;
            IsVirtualMachine = false;
            IsConvertible = false;
            IsDetachable = false;
            IsTablet = false;
            IPAddresses4 = string.Empty;
            IPAddresses6 = string.Empty;
            IPNetMask4 = string.Empty;
            IPNetMask6 = string.Empty;
            DefaultGateways4 = string.Empty;
            DefaultGateways6 = string.Empty;
            DHCPServer = string.Empty;
        }
    }
}
