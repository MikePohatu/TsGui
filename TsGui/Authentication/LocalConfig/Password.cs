#region license
// Copyright (c) 2020 Mike Pohatu
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Core.Diagnostics;

namespace TsGui.Authentication.LocalConfig
{
    public class Password
    {
        private static int _blocksize = 128;
        public string HashedPassword { get; private set; }
        public string HashKey { get; private set; }

        public Password(XElement inputxml)
        {
            this.LoadXml(inputxml);

            //Test the hashes so errors can be thrown on load
            ConvertFromBase64(this.HashKey);
            ConvertFromBase64(this.HashedPassword);
        }

        public Password(string passwordhash, string key)
        {
            this.HashKey = key;
            this.HashedPassword = passwordhash;

            //Test the hashes so errors can be thrown on load
            ConvertFromBase64(this.HashKey);
            ConvertFromBase64(this.HashedPassword);
        }

        private void LoadXml(XElement inputxml)
        {
            this.HashedPassword = XmlHandler.GetStringFromXElement(inputxml, "PasswordHash", this.HashedPassword);
            this.HashKey = XmlHandler.GetStringFromXElement(inputxml, "Key", this.HashKey);
        }

        public bool PasswordMatches(string clearpassword)
        {
            string testhash = HashPassword(clearpassword, this.HashKey);

            if (testhash == this.HashedPassword) { return true; }
            else { return false; }
        }

        public static byte[] EncryptString(SymmetricAlgorithm alg, string inString)
        {
            ICryptoTransform xfrm = alg.CreateEncryptor();
            byte[] inBlock = Encoding.Unicode.GetBytes(inString);
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

            return outBlock;
        }

        public static string CreateKey()
        {
            SymmetricAlgorithm alg = Aes.Create();
            alg.BlockSize = _blocksize;
            byte[] key = alg.Key.Concat(alg.IV).ToArray(); //concat the key and IV arrays together
            return Convert.ToBase64String(key);
        }

        public static string HashPassword(string cleartextpassword, string key)
        {
            SymmetricAlgorithm alg = Aes.Create();
            byte[] keyivarr = ConvertFromBase64(key);
            
            alg.BlockSize = _blocksize;

            byte[] encrypted;
            try
            {
                alg.Key = keyivarr.Take(32).ToArray(); //key and IV are concated together. first 32 is key
                alg.IV = keyivarr.Skip(32).ToArray();

                encrypted = EncryptString(alg, cleartextpassword);
            }
            catch (Exception e)
            {
                throw new KnownException("There was an error encrypting data. There may be an issue with the configured hash values", e.Message);
            }

            return Convert.ToBase64String(encrypted); 
        }

        private static byte[] ConvertFromBase64(string input)
        {
            byte[] output;
            try
            {
                output = Convert.FromBase64String(input);
            }
            catch (FormatException e)
            {
                throw new KnownException("Error with hash value", e.Message);
            }
            return output;
        }
    }
}
