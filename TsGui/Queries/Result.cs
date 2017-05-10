//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// Result.cs - encapsulates the results from a query i.e the key value, the properties
// and any branch/sub results associated with it

using System.Collections.Generic;

namespace TsGui.Queries
{
    public class Result
    {
        public List<PropertyFormatter> Properties { get; set; }
        public ResultWrangler Branch { get; set; }
        public PropertyFormatter KeyProperty { get; set; }

        public Result()
        {
            this.Properties = new List<PropertyFormatter>();
        }

        public void Add(PropertyFormatter newpropertyformatter)
        {
            if (this.KeyProperty == null) { this.KeyProperty = newpropertyformatter; }
            else { this.Properties.Add(newpropertyformatter); }
        }

        public List<PropertyFormatter> GetAllPropertyFormatters()
        {
            List<PropertyFormatter> l = new List<PropertyFormatter>();
            l.AddRange(this.Properties);
            l.AddRange(Branch.GetAllPropertyFormatters());
            return l;
        }
    }
}
