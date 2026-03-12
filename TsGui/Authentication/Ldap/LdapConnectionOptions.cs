#region license
// Copyright (c) 2025 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Configuration options for <see cref="LdapConnection"/>.
    /// </summary>
    public class LdapConnectionOptions
    {
        /// <summary>LDAP server hostname or IP address.</summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Port number. Set to 0 to use the default based on <see cref="UseSsl"/>:
        /// 636 for LDAPS, 389 for plain or StartTLS.
        /// </summary>
        public int Port { get; set; } = 0;

        /// <summary>Connect directly over SSL/TLS (LDAPS, port 636).</summary>
        public bool UseSsl { get; set; } = false;

        /// <summary>Upgrade a plain connection to TLS using StartTLS (RFC 4513).</summary>
        public bool UseStartTls { get; set; } = false;

        /// <summary>
        /// Optional callback to validate the server's TLS certificate.
        /// If null, the system default validation is used.
        /// Set to <c>(_, _, _, _) => true</c> to skip validation (testing only).
        /// </summary>
        public RemoteCertificateValidationCallback CertificateValidation { get; set; }

        /// <summary>Client certificates to present during the TLS handshake (optional).</summary>
        public X509CertificateCollection ClientCertificates { get; set; }

        /// <summary>TLS protocol versions to allow.</summary>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls11;

        /// <summary>TCP connect timeout in milliseconds.</summary>
        public int ConnectTimeoutMs { get; set; } = 5000;

        /// <summary>Network read timeout in milliseconds (0 = infinite).</summary>
        public int ReadTimeoutMs { get; set; } = 30000;

        /// <summary>Initial size of the internal receive buffer in bytes.</summary>
        public int ReceiveBufferSize { get; set; } = 65536;

        internal int ResolvedPort => Port > 0 ? Port : (UseSsl ? 636 : 389);
    }
}
