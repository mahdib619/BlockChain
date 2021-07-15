using System;
using System.Security.Cryptography;
using System.Text;

namespace BlockChainSample.Utilities
{
    public static class Tools
    {
        public static string Hash(string input)
        {
            var hashBytes = new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
