using System;
using System.Runtime.InteropServices;
using System.Security;

namespace LetsEncrypt.Core.Extensions
{
    public static partial class Extensions
    {
        public static string ToStandardString(this SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}