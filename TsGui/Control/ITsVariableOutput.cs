using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsGui
{
    public interface ITsVariableOutput
    {
        void AddVariable(TsVariable Variable);
        void Release();
        void Hide();
    }
}
