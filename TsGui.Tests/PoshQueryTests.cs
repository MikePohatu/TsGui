using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using TsGui.Queries;
using TsGui.Tests.Linking;
using TsGui.Scripts;
using MessageCrap;
using System.IO;

namespace TsGui.Tests
{
    public class PoshQueryTests
    {
        [Test]
        [TestCase("Write-Output 'Done'", ExpectedResult = "Done, \0")]
        public async Task<string> MultiScriptRunTest(string posh)
        {
            string testpath = Environment.CurrentDirectory + "\\Test1.ps1";
            this.CreateScript(testpath, posh);

            XElement config = new XElement("Query",
                new XAttribute("Type", "PowerShell"),
                new XElement("Script", new XElement("Name", "Test1.ps1")));
            PoshQuery query = new PoshQuery(config,null);

            Message message = MessageHub.CreateMessage(this, null);

            List<Task> tasks = new List<Task>();
            tasks.Add(query.ProcessQueryAsync(null));
            tasks.Add(query.ProcessQueryAsync(null));
            tasks.Add(query.ProcessQueryAsync(null));
            tasks.Add(query.ProcessQueryAsync(null));
            tasks.Add(query.ProcessQueryAsync(null));

            await Task.WhenAll(tasks);

            var result = await query.GetResultWrangler(message);

            return result.GetString();
        }

        private void CreateScript(string filepath, string content)
        {
            using (FileStream fs = File.Create(filepath))
            {
                // writing data in string
                string dataasstring = content; //your data
                byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                fs.Write(info, 0, info.Length);

                // writing data in bytes already
                byte[] data = new byte[] { 0x0 };
                fs.Write(data, 0, data.Length);
            }
        }
    }
}
