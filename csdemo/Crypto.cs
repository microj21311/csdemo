using OpenSSL.Crypto;
using System;
using System.IO;
using System.Text;

namespace CsDemo
{
    /// <summary>
    /// rsa加解密(openssl)
    /// </summary>
    class Crypto
    {
        //RSA最大加密明文大小
        private const int MAX_ENCRYPT_BLOCK = 117;
        //RSA最大解密密文大小
        private const int MAX_DECRYPT_BLOCK = 128;

        /// <summary>
        /// rsa加密 至base64
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string EncryptToBase64(string publicKey, string strSource)
        {
            return Convert.ToBase64String(Encrypt(publicKey, Encoding.UTF8.GetBytes(strSource)));
        }
        /// <summary>
        /// rsa加密
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string publicKey, byte[] source)
        {
            CryptoKey d = CryptoKey.FromPublicKey(publicKey, null);
            using (OpenSSL.Crypto.RSA rsa = d.GetRSA())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int inputLen = source.Length;
                    int offSet = 0;
                    int i = 0;
                    // 对数据分段加密
                    while (inputLen - offSet > 0)
                    {
                        byte[] cache = new byte[MAX_ENCRYPT_BLOCK];
                        if (inputLen - offSet > MAX_ENCRYPT_BLOCK)
                        {
                            Array.Copy(source, offSet, cache, 0, MAX_ENCRYPT_BLOCK);
                        }
                        else
                        {
                            Array.Copy(source, offSet, cache, 0, inputLen - offSet);
                        }
                        cache = rsa.PublicEncrypt(cache, OpenSSL.Crypto.RSA.Padding.PKCS1);
                        ms.Write(cache, 0, cache.Length);
                        offSet = ++i * MAX_ENCRYPT_BLOCK;
                    }
                    return ms.ToArray();
                }
            }
        }
        /// <summary>
        /// rsa解密 从base64
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DecryptFromBase64(string publicKey, string source)
        {
            return Encoding.UTF8.GetString(Decrypt(publicKey, Convert.FromBase64String(source)));
        }
        /// <summary>
        /// rsa解密
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] Decrypt(string publicKey, byte[] source)
        {
            CryptoKey d = CryptoKey.FromPublicKey(publicKey, null);
            using (OpenSSL.Crypto.RSA rsa = d.GetRSA())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int inputLen = source.Length;
                    int offSet = 0;
                    int i = 0;
                    // 对数据分段解密
                    while (inputLen - offSet > 0)
                    {
                        byte[] cache = new byte[MAX_DECRYPT_BLOCK];
                        if (inputLen - offSet > MAX_DECRYPT_BLOCK)
                        {
                            Array.Copy(source, offSet, cache, 0, MAX_DECRYPT_BLOCK);
                        }
                        else
                        {
                            Array.Copy(source, offSet, cache, 0, inputLen - offSet);
                        }
                        cache = rsa.PublicDecrypt(cache, OpenSSL.Crypto.RSA.Padding.PKCS1);
                        ms.Write(cache, 0, cache.Length);
                        offSet = ++i * MAX_DECRYPT_BLOCK;
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}
