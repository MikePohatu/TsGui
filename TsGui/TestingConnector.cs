using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TsGui
{
    public class TestingConnector: ITsVariableOutput
    {
        private List<TsVariable> variables = new List<TsVariable>();

        public void AddVariable(TsVariable Variable)
        {
            this.variables.Add(Variable);
        }

        public void Release()
        {
            string msg = "Task sequence variables created:" + Environment.NewLine + Environment.NewLine;

            foreach (TsVariable variable in this.variables)
            {
                msg = msg + variable.Name + ": " + variable.Value + Environment.NewLine;
            }

            MessageBox.Show(msg);
        }

        public void Hide()
        { }
    }
}
