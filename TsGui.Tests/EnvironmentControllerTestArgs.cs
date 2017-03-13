using System.Collections.Generic;
using System.Xml.Linq;
using System.Management;
using TsGui.Queries;

namespace TsGui.Tests
{
    public class EnvironmentControllerTestArgs
    {
        //{ expectedresult, wrangler, objcollection, proptemplates };
        public string ExpectedResult { get; set; }
        public ResultWrangler Wrangler { get; set; }
        public List<ManagementObject> ManagementObjectList { get; set; }
        public List<KeyValuePair<string, XElement>> PropertyTemplates { get; set; }

        public EnvironmentControllerTestArgs (string ExpectedResult, ResultWrangler Wrangler, List<ManagementObject> ManagementObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            this.ExpectedResult = ExpectedResult;
            this.Wrangler = Wrangler;
            this.ManagementObjectList = ManagementObjectList;
            this.PropertyTemplates = PropertyTemplates;
        }
    }
}
