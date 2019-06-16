//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// HardwareEvaluator.cs - class to identify hardware type
// ChassisTypes - https://technet.microsoft.com/en-us/library/ee156537.aspx
// updated: https://blogs.technet.microsoft.com/brandonlinton/2017/09/15/updated-win32_systemenclosure-chassis-types/ 

using System;
using System.Collections.Generic;
using System.Management;

using TsGui.Connectors;

namespace TsGui
{
    public class HardwareEvaluator
    {
        private string _namespace = @"root\CIMV2";

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

        public HardwareEvaluator()
        {
            this.Evaluate();
        }

        public List<TsVariable> GetTsVariables()
        {
            List<TsVariable> vars = new List<TsVariable>();

            if (IsLaptop) { vars.Add(new TsVariable("TsGui_IsLaptop", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsLaptop", "FALSE")); }

            if (IsDesktop) { vars.Add(new TsVariable("TsGui_IsDesktop", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsDesktop", "FALSE")); }

            if (IsServer) { vars.Add(new TsVariable("TsGui_IsServer", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsServer", "FALSE")); }

            if (IsVirtualMachine) { vars.Add(new TsVariable("TsGui_IsVirtualMachine", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsVirtualMachine", "FALSE")); }

            if (IsConvertible) { vars.Add(new TsVariable("TsGui_IsConvertible", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsConvertible", "FALSE")); }

            if (IsDetachable) { vars.Add(new TsVariable("TsGui_IsDetachable", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsDetachable", "FALSE")); }

            if (IsTablet) { vars.Add(new TsVariable("TsGui_IsTablet", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsTablet", "FALSE")); }

            vars.Add(new TsVariable("TsGui_IPv4", this.IPAddresses4));
            vars.Add(new TsVariable("TsGui_IPv6", this.IPAddresses6));
            vars.Add(new TsVariable("TsGui_DefaultGateway4", this.DefaultGateways4));
            vars.Add(new TsVariable("TsGui_DefaultGateway6", this.DefaultGateways6));
            vars.Add(new TsVariable("TsGui_IPSubetMask4", this.IPNetMask4));
            vars.Add(new TsVariable("TsGui_IPSubnetMask6", this.IPNetMask6));
            vars.Add(new TsVariable("TsGui_DHCPServer", this.DHCPServer));

            return vars;
        }

        private void Evaluate()
        {
            //virtual machine tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(this._namespace, "select Model,Manufacturer from Win32_ComputerSystem"))
            {
                string model = (string)m["Model"];
                string maker = (string)m["Manufacturer"];
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
                //Xen
                if (maker.Contains("Xen"))
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

            //chassis type tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(this._namespace,"select ChassisTypes from Win32_SystemEnclosure"))
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
