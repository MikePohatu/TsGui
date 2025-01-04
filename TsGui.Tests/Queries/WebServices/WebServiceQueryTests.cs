#region license
// Copyright (c) 2025 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using NUnit.Framework;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Queries.WebServices;

namespace TsGui.Tests
{
    [TestFixture]
    public class WebServiceQueryTests
    {
        [Test]
        [TestCase("GetCMDiscoveredUsers", new string[]{"secret", "a4f6accb-0746-4c94-a9d3-e700ae8109e0"}, 
            ExpectedResult = @"<?xml version=""1.0"" encoding=""UTF - 8""?>< ArrayOfCMUser xmlns = ""http://www.scconfigmgr.com"" xmlns: xsi = ""http://www.w3.org/2001/XMLSchema-instance"" xmlns: xsd = ""http://www.w3.org/2001/XMLSchema"" /> ")]       
        public async Task<string> BasicWebServiceTestAsync(string Method, string[] Parameters)
        {
            XElement conf = new XElement("Query");            
            conf.Add(new XElement("ServiceURL", "http://sccm01/ConfigMgrWebService/ConfigMgr.asmx"));
            conf.Add(new XElement("Method", Method));
            conf.Add(new XAttribute("AuthID", "TestCase"));
            for (int i=0;i<Parameters.GetLength(0);i=i+2)
            {
                XElement x = new XElement("Parameter", Parameters[i+1]);
                x.Add(new XAttribute("Name", Parameters[i]));
                conf.Add(x);              
            }

            TestDirector director = new TestDirector();
            Director.OverrideInstance(director);
            WebServicesQuery newquery = new WebServicesQuery(conf, null);
            await newquery.ProcessQueryAsync(null);
            return newquery.Response;
        }
    }
}
