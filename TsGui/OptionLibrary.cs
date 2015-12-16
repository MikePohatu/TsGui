using System.Collections.Generic;

namespace TsGui
{
    class OptionLibrary
    {
        private List<IGuiOption> _options = new List<IGuiOption>();

        public List<IGuiOption> Options { get { return this._options; } }

        public void Add(IGuiOption Option)
        {
            this._options.Add(Option);
        }
    }
}
