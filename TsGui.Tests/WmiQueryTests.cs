//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using NUnit.Framework;
using System.Xml.Linq;
using System.Management;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TsGui.Queries;
using TsGui.Tests.Linking;

namespace TsGui.Tests
{
    [TestFixture]
    public class WmiQueryTests
    {
        [Test]
        [TestCaseSource("AddWmiPropertiesToWrangler_TestCases")]
        public void AddWmiPropertiesToWrangler_Test(WmiQueryTestArgs TestArgs)
        {
            //protected void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
            WmiQuery query = new WmiQuery(new DummyLinkTarget());
            PrivateObject obj = new PrivateObject(query);

            object[] args = new object[3] { TestArgs.Wrangler, TestArgs.ManagementObjectList, TestArgs.PropertyTemplates };
            obj.Invoke("AddWmiPropertiesToWrangler",args);

            ResultWrangler wrangler = TestArgs.Wrangler;
            string s = wrangler.GetString();
            NUnit.Framework.Assert.AreEqual(s, TestArgs.ExpectedResult);
        }

        public static IEnumerable<TestCaseData> AddWmiPropertiesToWrangler_TestCases
        {
            get
            {
                yield return new TestCaseData(AddWmiPropertiesToWrangler_TestArgs1(true));
                yield return new TestCaseData(AddWmiPropertiesToWrangler_TestArgs1(false));
                yield return new TestCaseData(AddWmiPropertiesToWrangler_TestArgs2(true));
                yield return new TestCaseData(AddWmiPropertiesToWrangler_TestArgs2(false));
                yield return new TestCaseData(AddWmiPropertiesToWrangler_TestArgs3());
                yield return new TestCaseData(AddWmiPropertiesToWrangler_TestArgsNull());
            }
        }

        private static WmiQueryTestArgs AddWmiPropertiesToWrangler_TestArgs1(bool IncludeFalseValues)
        {
            string expectedresult;

            if (IncludeFalseValues == true) { expectedresult = ",1"; }
            else { expectedresult = "1"; }

            //protected void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
            List<KeyValuePair<string, XElement>> proptemplates = new List<KeyValuePair<string, XElement>>();
            XElement propx = new XElement("Property");
            propx.Add(new XAttribute("Name", "BatteryStatus"));
            proptemplates.Add(new KeyValuePair<string, XElement>("BatteryStatus", propx));

            ResultWrangler wrangler = new ResultWrangler();
            wrangler.IncludeNullValues = IncludeFalseValues;
            wrangler.Separator = ",";

            ManagementClass batt1 = new ManagementClass();
            ManagementClass batt2 = new ManagementClass();
            batt1.Properties.Add("BatteryStatus", null, CimType.UInt16);
            batt2.Properties.Add("BatteryStatus", 1, CimType.UInt16);

            List<ManagementObject> objcollection = new List<ManagementObject>();
            objcollection.Add(batt1);
            objcollection.Add(batt2);

            return new WmiQueryTestArgs( expectedresult, wrangler, objcollection, proptemplates );
        }

        private static WmiQueryTestArgs AddWmiPropertiesToWrangler_TestArgs2(bool IncludeFalseValues)
        {
            string expectedresult;

            if (IncludeFalseValues == true) { expectedresult = "1,"; }
            else { expectedresult = "1"; }

            //protected void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
            List<KeyValuePair<string, XElement>> proptemplates = new List<KeyValuePair<string, XElement>>();

            ResultWrangler wrangler = new ResultWrangler();
            wrangler.IncludeNullValues = IncludeFalseValues;
            wrangler.Separator = ",";

            ManagementClass batt1 = new ManagementClass();
            ManagementClass batt2 = new ManagementClass();
            batt1.Properties.Add("BatteryStatus", 1, CimType.UInt16);
            batt2.Properties.Add("BatteryStatus", null, CimType.UInt16);

            List<ManagementObject> objcollection = new List<ManagementObject>();
            objcollection.Add(batt1);
            objcollection.Add(batt2);

            return new WmiQueryTestArgs(expectedresult, wrangler, objcollection, proptemplates);
        }

        private static WmiQueryTestArgs AddWmiPropertiesToWrangler_TestArgs3()
        {
            string expectedresult = "Test Model,Test_VM";

            //protected void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
            List<KeyValuePair<string, XElement>> proptemplates = new List<KeyValuePair<string, XElement>>();

            XElement propx = new XElement("Property");
            propx.Add(new XAttribute("Name", "Model"));
            proptemplates.Add(new KeyValuePair<string, XElement>("Model", propx));

            propx = new XElement("Property");
            propx.Add(new XAttribute("Name", "Serial"));
            proptemplates.Add(new KeyValuePair<string, XElement>("Serial", propx));

            ResultWrangler wrangler = new ResultWrangler();
            wrangler.Separator = ",";

            ManagementClass wmi1 = new ManagementClass();

            wmi1.Properties.Add("Model", "Test Model", CimType.String);
            wmi1.Properties.Add("Serial", "Test_VM", CimType.String);

            List<ManagementObject> objcollection = new List<ManagementObject>();
            objcollection.Add(wmi1);

            return new WmiQueryTestArgs(expectedresult, wrangler, objcollection, proptemplates);
        }

        private static WmiQueryTestArgs AddWmiPropertiesToWrangler_TestArgsNull()
        {
            string expectedresult = null;

            //protected void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
            List<KeyValuePair<string, XElement>> proptemplates = new List<KeyValuePair<string, XElement>>();

            XElement propx = new XElement("Property");
            propx.Add(new XAttribute("Name", "BatteryStatus"));
            proptemplates.Add(new KeyValuePair<string, XElement>("BatteryStatus", propx));

            ResultWrangler wrangler = new ResultWrangler();
            wrangler.Separator = null;

            List<ManagementObject> objcollection = new List<ManagementObject>();

            return new WmiQueryTestArgs(expectedresult, wrangler, objcollection, proptemplates);
        }
    }
}
