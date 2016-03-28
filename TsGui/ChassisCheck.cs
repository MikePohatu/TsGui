using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Management;

namespace TsGui
{
    public class ChassisCheck
    {
        private bool _islaptop = false;
        private bool _isdesktop = false;
        private bool _isserver = false;

        public bool IsLaptop { get { return this._islaptop; } }
        public bool IsDesktop { get { return this._isdesktop; } }
        public bool IsServer { get { return this._isserver; } }

        public ChassisCheck()
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

                return vars;
            }
        }

        private void Evaluate()
        { 
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjects("select ChassisTypes from Win32_SystemEnclosure"))
            {
                if (m != null)
                {
                    foreach (PropertyData propdata in m.Properties)
                    {
                        int i = (int)propdata.Value;


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
                                    this._islaptop = true;
                                    break;
                                }
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 15:
                            case 16:
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
                                {
                                    break;
                                    //nothing
                                }
                        }
                    }
                }
            }
        }
    }
}
