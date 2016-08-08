
namespace TsGui
{
    public interface IGroupParent
    {
        event ParentToggleEvent ParentChanged;
        Group Group { get; }
    }
}
