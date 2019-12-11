using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TsGui.Linking;

namespace TsGui.Tests.Linking
{
    public class DummyLinkTarget: ILinkTarget
    {


        public void RefreshValue()
        {
            Debug.WriteLine("RefreshValue");
        }

        public void RefreshAll()
        {
            Debug.WriteLine("RefreshAll");
        }
    }
}
