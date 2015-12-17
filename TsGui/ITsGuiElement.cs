using System.Xml.Linq;

namespace TsGui
{
    public interface ITsGuiElement
    {
        void LoadXml(XElement Xml);
    }
}
