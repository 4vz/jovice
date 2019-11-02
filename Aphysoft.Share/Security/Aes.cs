using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Aes
    {
        private static byte[] EncryptProcess(byte[] data, AesManaged am)
        {
            byte[] edata = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, am.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }

                edata = ms.ToArray();
            }

            return edata;
        }

        public static byte[] Encrypt(byte[] data, byte[] iv, byte[] key)
        {
            byte[] edata = null;

            using (AesManaged am = new AesManaged())
            {
                am.IV = iv;
                am.Key = key;

                edata = EncryptProcess(data, am);
            }

            return edata;
        }

        public static byte[] Encrypt(byte[] data, out byte[] iv, out byte[] key)
        {
            byte[] edata = null;

            using (AesManaged am = new AesManaged())
            {
                am.Padding = PaddingMode.PKCS7;

                am.GenerateIV();
                am.GenerateKey();

                iv = am.IV;
                key = am.Key;

                edata = EncryptProcess(data, am);
            }

            return edata;
        }

        public static byte[] Decrypt(byte[] edata, byte[] iv, byte[] key)
        {
            byte[] data = null;

            using (AesManaged am = new AesManaged())
            {
                am.IV = iv;
                am.Key = key;
                am.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, am.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(edata, 0, edata.Length);
                    }

                    data = ms.ToArray();
                }
            }

            return data;
        }

        public static int EncryptedLength(int length)
        {
            return ((int)Math.Floor((double)length / 16) + 1) * 16;
        }
    }
}
