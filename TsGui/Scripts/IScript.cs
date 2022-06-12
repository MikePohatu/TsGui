using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsGui.Scripts
{
    public interface IScript
    {
        string Name { get; }
        string Path { get; }
        Task RunScriptAsync();
    }
}
