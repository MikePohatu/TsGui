namespace TsGui
{
    public interface IGroupable
    {
        bool IsActive { get; }
        bool IsEnabled { get; set; }
        bool IsHidden { get; set; }
    }
}
