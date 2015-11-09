using System;

namespace core
{
    public class TsFreeText: IGuiOption
    {
        private TsVariable tsvar;
        private string name;
        private string value;

        public TsVariable TsVariable
        {
            get { return tsvar; }
            //set { this.tsvar = value; }
        }
    }
}
