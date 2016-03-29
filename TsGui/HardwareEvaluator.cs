// ChassisTypes - https://technet.microsoft.com/en-us/library/ee156537.aspx

using System;
using System.Collections.Generic;

using System.Management;

namespace TsGui
{
    public class HardwareEvaluator
    {
        private bool _islaptop = false;
        private bool _isdesktop = false;
        private bool _isserver = false;
        private bool _isvm = false;

        public bool IsLaptop { get { return this._islaptop; } }
        public bool IsDesktop { get { return this._isdesktop; } }
        public bool IsServer { get { return this._isserver; } }
        public bool IsVirtualMachine { get { return this._isvm; } }

        public HardwareEvaluator()
        {
            this.Evaluate();
        }

        public List<TsVariable> GetTsVariables
        {
            get
            {
                List<TsVariable> vars = new List<TsVariable>();

                if (_islaptop) { vars.Add(new TsVariable("TsGui_IsLaptop", "TRUE")); }
                else { vars.Add(new TsVariable("TsGui_IsLaptop", "FALSE")); }

                if (_isdesktop) { vars.Add(new TsVariable("TsGui_IsDesktop", "TRUE")); }
                else { vars.Add(new TsVariable("TsGui_IsDesktop", "FALSE")); }

                if (_isserver) { vars.Add(new TsVariable("TsGui_IsServer", "TRUE")); }
                else { vars.Add(new TsVariable("TsGui_IsServer", "FALSE")); }

                if (_isvm) { vars.Add(new TsVariable("TsGui_IsVirtualMachine", "TRUE")); }
                else { vars.Add(new TsVariable("TsGui_IsVirtualMachine", "FALSE")); }

                return vars;
            }
        }

        private void Evaluate()
        {
            //virtual machine tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjects("select Model from Win32_ComputerSystem"))
            {
                string model = (string)m["Model"]; 
                switch (model)
                {
                    case "VMware Virtual Platform":
                        {
                            this._isvm = true;
                            break;
                        }
                    default:
                        { break; }
                }
            }

            //chassis type tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjects("select ChassisTypes from Win32_SystemEnclosure"))
            {
                Int16[] chassistypes = (Int16[])m["ChassisTypes"];

                foreach (Int16 i in chassistypes)
                {
                    switch (i)
                    {
                        case 8: case 9: case 10: case 11:  case 12: case 14: case 18: case 21:
                            {
                                this._islaptop = true;
                                break;
                            }
                        case 3: case 4: case 5:  case 6: case 7: case 15: case 16:
                            {
                                this._isdesktop = true;
                                break;
                            }
                        case 23:
                            {
                                this._isserver = true;
                                break;
                            }
                        default:
                            { break; }
                    }
                }
            }
        }
    }
}
