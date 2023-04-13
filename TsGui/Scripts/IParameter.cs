using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.Scripts
{
    public interface IParameter
    {
        string Name { get; }

        /// <summary>
        /// Get the value for the parameter. Will be a string for normal Parameter types, SecureString for SecureStringParameter types
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<object> GetValue(Message message);
    }
}
