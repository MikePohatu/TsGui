using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsGui.Authentication
{
    public interface IAuthenticatorConsumer: IAuthenticationComponent
    {
        IAuthenticator Authenticator { get; set; }
    }
}
