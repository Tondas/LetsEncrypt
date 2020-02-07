using System;
using System.IO;
using System.Security.Cryptography;

namespace LetsEncrypt.Client.Extensions
{
    public static partial class Extensions
    {
        public static RSAParameters CreateRsaParametersFromKeyBytes(this byte[] rsaKeyPemBytes)
        {
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(rsaKeyPemBytes)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binReader.ReadUInt16();
                if (twobytes == 0x8130)
                    binReader.ReadByte();
                else if (twobytes == 0x8230)
                    binReader.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binReader.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binReader.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                var result = new RSAParameters();
                result.Modulus = binReader.ReadBytes(GetIntegerSize(binReader));
                result.Exponent = binReader.ReadBytes(GetIntegerSize(binReader));
                result.D = binReader.ReadBytes(GetIntegerSize(binReader));
                result.P = binReader.ReadBytes(GetIntegerSize(binReader));
                result.Q = binReader.ReadBytes(GetIntegerSize(binReader));
                result.DP = binReader.ReadBytes(GetIntegerSize(binReader));
                result.DQ = binReader.ReadBytes(GetIntegerSize(binReader));
                result.InverseQ = binReader.ReadBytes(GetIntegerSize(binReader));
                return result;
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowByte = 0x00;
            byte highByte = 0x00;
            int count = 0;

            bt = binr.ReadByte();
            if (bt != 0x02)
            {
                return 0;
            }
            bt = binr.ReadByte();

            if (bt == 0x81)
            {
                count = binr.ReadByte();
            }
            else if (bt == 0x82)
            {
                highByte = binr.ReadByte();
                lowByte = binr.ReadByte();
                byte[] modint = { lowByte, highByte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }
    }
}