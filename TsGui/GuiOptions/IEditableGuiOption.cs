

namespace TsGui
{
    public interface IEditableGuiOption
    {
        bool IsValid { get; }
        bool IsActive { get; }
        void ClearToolTips();
    }
}
