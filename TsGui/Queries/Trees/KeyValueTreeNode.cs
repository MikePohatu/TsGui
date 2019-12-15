using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsGui.Queries.Trees
{
    public class KeyValueTreeNode
    {
        private List<KeyValueTreeNode> _nodes = new List<KeyValueTreeNode>();

        public KeyValuePair<string,string> Value { get; set; }
        public List<KeyValueTreeNode> Nodes { get { return this._nodes; } }
    }
}
