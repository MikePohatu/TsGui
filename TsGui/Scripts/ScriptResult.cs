namespace TsGui.Scripts
{
    public class ScriptResult<T>
    {
        public int ReturnCode { get; set; }
        public T ReturnedObject { get; set; }
    }
}
