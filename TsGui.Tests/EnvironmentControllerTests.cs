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

namespace TsGui.Tests
{
    [TestFixture]
    public class EnvironmentControllerTests
    {

        [Test]
        [TestCase(null, ExpectedResult = null)]
        [TestCase("ComputerName", ExpectedResult = "WIN10")]
        public string GetEnvVarTest(string VariableName)
        {
            EnvironmentController controller = new EnvironmentController();
            return controller.GetEnvVar(VariableName);
        }


        [Test]
        [TestCase(true, ExpectedResult = ",1")]
        [TestCase(false, ExpectedResult = "1")]
        public string AddWmiPropertiesToWranglerTest(bool IncludeFalseValues)
        {
            //protected void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
            //< Query Type = "Wmi" >
            //    < Wql > SELECT BatteryStatus FROM Win32_Battery </ Wql >
            //    < Property Name = "BatteryStatus" />
            //    < Separator ></ Separator >
            // </ Query >
            List<KeyValuePair<string, XElement>> proptemplates = new List<KeyValuePair<string, XElement>>();
            XElement propx = new XElement("Property");
            propx.Add(new XAttribute("Name", "BatteryStatus"));
            proptemplates.Add(new KeyValuePair<string, XElement>("BatteryStatus",propx));

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

            EnvironmentController envcontroller = new EnvironmentController();
            PrivateObject obj = new PrivateObject(envcontroller);
            object[] args = new object[3] { wrangler, objcollection, proptemplates};
            obj.Invoke("AddWmiPropertiesToWrangler",args);

            string s = wrangler.GetString();
            return s;
        }
    }
}
