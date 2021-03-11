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

// HardwareEvaluator.cs - class to identify hardware type
// ChassisTypes - https://technet.microsoft.com/en-us/library/ee156537.aspx
// updated: https://blogs.technet.microsoft.com/brandonlinton/2017/09/15/updated-win32_systemenclosure-chassis-types/ 

using System;
using System.Collections.Generic;
using System.Management;
using System.Xml.Linq;
using TsGui.Connectors;

namespace TsGui
{
    public class HardwareEvaluator
    {
        private string _namespace = @"root\CIMV2";
        private string _path = null;

        public bool IsLaptop { get; private set; } = false;
        public bool IsDesktop { get; private set; } = false;
        public bool IsServer { get; private set; } = false;
        public bool IsVirtualMachine { get; private set; } = false;
        public bool IsConvertible { get; private set; } = false;
        public bool IsDetachable { get; private set; } = false;
        public bool IsTablet { get; private set; } = false;
        public string IPAddresses4 { get; private set; } = string.Empty;
        public string IPAddresses6 { get; private set; } = string.Empty;
        public string IPNetMask4 { get; private set; } = string.Empty;
        public string IPNetMask6 { get; private set; } = string.Empty;
        public string DefaultGateways4 { get; private set; } = string.Empty;
        public string DefaultGateways6 { get; private set; } = string.Empty;
        public string DHCPServer { get; private set; } = string.Empty;

        /// <summary>
        /// Query WMI and populate the data for the TsGui_xxx variables
        /// </summary>
        public HardwareEvaluator(XElement inputxml)
        {
            this._path = XmlHandler.GetStringFromXAttribute(inputxml, "Path", this._path);
            if (string.IsNullOrWhiteSpace(this._path)) { this._path = Director.Instance.DefaultPath; }
            this.Evaluate();
        }

        public List<Variable> GetTsVariables()
        {
            List<Variable> vars = new List<Variable>();

            if (IsLaptop) { vars.Add(new Variable("TsGui_IsLaptop", "TRUE", this._path)); }
            else { vars.Add(new Variable("TsGui_IsLaptop", "FALSE", this._path)); }

            if (IsDesktop) { vars.Add(new Variable("TsGui_IsDesktop", "TRUE", this._path)); }
            else { vars.Add(new Variable("TsGui_IsDesktop", "FALSE", this._path)); }

            if (IsServer) { vars.Add(new Variable("TsGui_IsServer", "TRUE", this._path)); }
            else { vars.Add(new Variable("TsGui_IsServer", "FALSE", this._path)); }

            if (IsVirtualMachine) { vars.Add(new Variable("TsGui_IsVirtualMachine", "TRUE", this._path)); }
            else { vars.Add(new Variable("TsGui_IsVirtualMachine", "FALSE", this._path)); }

            if (IsConvertible) { vars.Add(new Variable("TsGui_IsConvertible", "TRUE", this._path)); }
            else { vars.Add(new Variable("TsGui_IsConvertible", "FALSE", this._path)); }

            if (IsDetachable) { vars.Add(new Variable("TsGui_IsDetachable", "TRUE", this._path)); }
            else { vars.Add(new Variable("TsGui_IsDetachable", "FALSE", this._path)); }

            if (IsTablet) { vars.Add(new Variable("TsGui_IsTablet", "TRUE", this._path)); }
            else { vars.Add(new Variable("TsGui_IsTablet", "FALSE", this._path)); }

            vars.Add(new Variable("TsGui_IPv4", this.IPAddresses4, this._path));
            vars.Add(new Variable("TsGui_IPv6", this.IPAddresses6, this._path));
            vars.Add(new Variable("TsGui_DefaultGateway4", this.DefaultGateways4, this._path));
            vars.Add(new Variable("TsGui_DefaultGateway6", this.DefaultGateways6, this._path));
            vars.Add(new Variable("TsGui_IPSubetMask4", this.IPNetMask4, this._path));
            vars.Add(new Variable("TsGui_IPSubnetMask6", this.IPNetMask6, this._path));
            vars.Add(new Variable("TsGui_DHCPServer", this.DHCPServer, this._path));

            return vars;
        }

        private void Evaluate()
        {
            //virtual machine tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(this._namespace, "select Model,Manufacturer from Win32_ComputerSystem"))
            {
                string model = (string)m["Model"];
                string maker = (string)m["Manufacturer"];
                if (string.IsNullOrWhiteSpace(model) == false)
                {
                    //vmware
                    if (model.Contains("VMware"))
                    {
                        this.IsVirtualMachine = true;
                        break;
                    }
                    //hyper-v
                    if (model == "Virtual Machine")
                    {
                        this.IsVirtualMachine = true;
                        break;
                    }
                    //virtualbox
                    if (model.Contains("VirtualBox"))
                    {
                        this.IsVirtualMachine = true;
                        break;
                    }
                    //Parallels
                    if (model.Contains("Parallels"))
                    {
                        this.IsVirtualMachine = true;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(maker) == false)
                {
                    //Xen
                    if (maker.Contains("Xen"))
                    {
                        this.IsVirtualMachine = true;
                        break;
                    }
                }
            }

            //chassis type tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(this._namespace, "select ChassisTypes from Win32_SystemEnclosure"))
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
                                this.IsLaptop = true;
                                break;
                            }
                        case 30:
                            {
                                this.IsLaptop = true;
                                this.IsTablet = true;
                                break;
                            }
                        case 31:
                            {
                                this.IsLaptop = true;
                                this.IsConvertible = true;
                                break;
                            }
                        case 32:
                            {
                                this.IsLaptop = true;
                                this.IsDetachable = true;
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
                                this.IsDesktop = true;
                                break;
                            }
                        case 23:
                        case 25:
                        case 28:
                        case 29:
                            {
                                this.IsServer = true;
                                break;
                            }
                        default:
                            { break; }
                    }
                }
            }

            //ip info gather
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(this._namespace, @"Select DefaultIPGateway,IPAddress,IPSubnet,DHCPServer from Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'True'"))
            {
                string[] ipaddresses = (string[])m["IPAddress"];

                if (ipaddresses != null)
                {
                    foreach (string s in ipaddresses)
                    {
                        if (string.IsNullOrEmpty(s) == false)
                        {
                            if (s.Contains(":")) { this.IPAddresses6 = AppendToStringList(this.IPAddresses6, s); }
                            else { this.IPAddresses4 = AppendToStringList(this.IPAddresses4, s); }
                        }
                    }
                }


                string[] defaultgateways = (string[])m["DefaultIPGateway"];
                if (defaultgateways != null)
                {
                    foreach (string s in defaultgateways)
                    {
                        if (s.Contains(":")) { this.DefaultGateways6 = AppendToStringList(this.DefaultGateways6, s); }
                        else { this.DefaultGateways4 = AppendToStringList(this.DefaultGateways4, s); }
                    }
                }

                string[] netmasks = (string[])m["IPSubnet"];
                if (netmasks != null)
                {
                    foreach (string s in netmasks)
                    {
                        if (s.Contains(".")) { this.IPNetMask4 = AppendToStringList(this.IPNetMask4, s); }
                        else { this.IPNetMask6 = AppendToStringList(this.IPNetMask6, s); }
                    }
                }

                string svr = (string)m["DHCPServer"];
                if (string.IsNullOrWhiteSpace(svr) == false) { this.DHCPServer = AppendToStringList(this.DHCPServer, svr); }
            }
        }

        private static string AppendToStringList(string basestring, string newstring)
        {
            string s;
            if (string.IsNullOrWhiteSpace(basestring)) { s = newstring; }
            else { s = basestring + ", " + newstring; }
            return s;
        }
    }
}
