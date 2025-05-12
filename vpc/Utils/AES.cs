using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace vpc
{
    internal class AESMtd
    {
        internal static void GenKeys(out string PublicKey, out string PrivateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                PublicKey = rsa.ToXmlString(false); // 公钥
                PrivateKey = rsa.ToXmlString(true); // 私钥
                System.Windows.Forms.MessageBox.Show(PublicKey + "\r\n\r\n" + PrivateKey);
            }
        }
        internal static void AESEncrypt(Cognex.VisionPro.ToolBlock.CogToolBlock cb, string filepath)
        {
            var memo = new System.IO.MemoryStream();
            Cognex.VisionPro.CogSerializer.SaveObjectToStream(cb, memo);
            AESEncrypt(memo, filepath);
        }
        internal static void AESEncrypt(MemoryStream bufm, string filepath)
        {
            if (bufm == null || bufm.Length == 0)
                return;
            byte[] buf = bufm.GetBuffer();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            string key = Settings.Default.k2;
            if (Settings.Default.test)
                key = "F69511F8DAD7F6DC09A2AAA9058D2478";
            byte[] iv = new byte[16];
            if (key != null && key.Length > 16)
            {
                Encoding.ASCII.GetBytes(key, 0, 16, iv, 0);
                aes.IV = iv;
                aes.Key = Encoding.ASCII.GetBytes(GetMD5str(key));
                aes.Padding = PaddingMode.Zeros;
                var trans = aes.CreateEncryptor();
                int len = (int)bufm.Length / trans.InputBlockSize * trans.InputBlockSize;
                byte[] outbuffer;
                if (bufm.Length > len)
                    outbuffer = new byte[len + trans.OutputBlockSize];
                else
                    outbuffer = new byte[len];
                if (len > 0)
                {
                    trans.TransformBlock(buf, 0, len, outbuffer, 0);
                }
                if (bufm.Length > len)
                {
                    var reb = trans.TransformFinalBlock(buf, len, (int)bufm.Length - len);
                    Array.Copy(reb, 0, outbuffer, len, reb.Length);
                }
                File.WriteAllBytes(filepath, outbuffer);
            }
        }
        internal static Cognex.VisionPro.ToolBlock.CogToolBlock AESDecryptCogToolBlock(string filepath)
        {
            var strm = AESDecrypt(filepath);
            return Cognex.VisionPro.CogSerializer.LoadObjectFromStream(strm) as Cognex.VisionPro.ToolBlock.CogToolBlock;
        }
        internal static MemoryStream AESDecrypt(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                string key = Settings.Default.k2;
                if (Settings.Default.test)
                    key = "F69511F8DAD7F6DC09A2AAA9058D2478";
                byte[] iv = new byte[16];
                if (key != null && key.Length > 16)
                {
                    Encoding.ASCII.GetBytes(key, 0, 16, iv, 0);
                    aes.IV = iv;
                    aes.Key = Encoding.ASCII.GetBytes(GetMD5str(key));
                    aes.Padding = PaddingMode.Zeros;
                    var trans = aes.CreateDecryptor();
                    byte[] buf = System.IO.File.ReadAllBytes(filepath);
                    int len = buf.Length / trans.InputBlockSize * trans.InputBlockSize;
                    if (len != buf.Length)
                        return null;
                    byte[] msbuf = new byte[len];
                    int translen = trans.TransformBlock(buf, 0, len, msbuf, 0);
                    if (translen < len)
                    {
                        byte[] lastblock = trans.TransformFinalBlock(buf, translen, len - translen);
                        Array.Copy(lastblock, 0, msbuf, translen, lastblock.Length);
                        translen += lastblock.Length;
                    }
                    return new MemoryStream(msbuf, 0, translen);
                }
            }
            return null;
        }
        internal static byte[] GetMD5(byte[] data)
        {
            byte[] hash = new byte[16];
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            hash = md5.ComputeHash(data);
            return hash;
        }
        internal static byte[] GetMD5(string data)
        {
            return GetMD5(Encoding.UTF8.GetBytes(data));
        }
        internal static string GetMD5str(string data)
        {
            return bytesToString(GetMD5(Encoding.UTF8.GetBytes(data)));
        }
        internal static string bytesToString(byte[] data)
        {
            if (data == null)
                return string.Empty;
            else
            {
                StringBuilder s = new StringBuilder(data.Length * 2);
                for (int i = 0; i < data.Length; i++)
                    s.Append(data[i].ToString("X2"));
                return s.ToString();
            }
        }
    }
}
