using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsGui
{
    internal class SccmConnector
    {
        dynamic objTSProgUI = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TsProgressUI"));
        dynamic objTSEnv = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TSEnvironment "));

        internal void AddVariable(string Name,string Value)
        {
            //this.cmComObject.Value[Name] = Value;
            objTSEnv.Value[Name] = Value;
        }

        internal void Release()
        {
            // Release the comm objects.
            if (System.Runtime.InteropServices.Marshal.IsComObject(this.objTSProgUI) == true)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(this.objTSProgUI);
            }

            if (System.Runtime.InteropServices.Marshal.IsComObject(this.objTSEnv) == true)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(this.objTSEnv);
            }
        }

        internal void CloseProgressUI()
        {
            objTSProgUI.CloseProgressDialog();
        }
    }
}
