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
namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Standard LDAP result codes (RFC 4511 §4.1.9).
    /// </summary>
    public enum LdapResultCode
    {
        Success                      = 0,
        OperationsError              = 1,
        ProtocolError                = 2,
        TimeLimitExceeded            = 3,
        SizeLimitExceeded            = 4,
        CompareFalse                 = 5,
        CompareTrue                  = 6,
        AuthMethodNotSupported       = 7,
        StrongerAuthRequired         = 8,
        Referral                     = 10,
        AdminLimitExceeded           = 11,
        UnavailableCriticalExtension = 12,
        ConfidentialityRequired      = 13,
        SaslBindInProgress           = 14,
        NoSuchAttribute              = 16,
        UndefinedAttributeType       = 17,
        InappropriateMatching        = 18,
        ConstraintViolation          = 19,
        AttributeOrValueExists       = 20,
        InvalidAttributeSyntax       = 21,
        NoSuchObject                 = 32,
        AliasProblem                 = 33,
        InvalidDNSyntax              = 34,
        IsLeaf                       = 35,
        AliasDereferencingProblem    = 36,
        InappropriateAuthentication  = 48,
        InvalidCredentials           = 49,
        InsufficientAccessRights     = 50,
        Busy                         = 51,
        Unavailable                  = 52,
        UnwillingToPerform           = 53,
        LoopDetect                   = 54,
        NamingViolation              = 64,
        ObjectClassViolation         = 65,
        NotAllowedOnNonLeaf          = 66,
        NotAllowedOnRDN              = 67,
        EntryAlreadyExists           = 68,
        ObjectClassModsProhibited    = 69,
        AffectsMultipleDSAs          = 71,
        Other                        = 80,
    }
}
