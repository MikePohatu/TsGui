using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsGui
{
    public class SccmConnector: ITsVariableOutput
    {
        dynamic objTSProgUI = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TsProgressUI"));
        dynamic objTSEnv = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TSEnvironment "));

        public SccmConnector()
        {
            objTSProgUI.CloseProgressDialog();
        }

        public void AddVariable(TsVariable Variable)
        {
            //this.cmComObject.Value[Name] = Value;
            objTSEnv.Value[Variable.Name] = Variable.Value;
        }

        public void Release()
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
    }
}
