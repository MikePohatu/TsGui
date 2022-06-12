using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsGui.Scripts
{
    internal class ScriptResult<T>
    {
        public int ReturnCode { get; set; }
        public T ReturnedObject { get; set; }
    }
}
