using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace CsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //公钥
            string publicKey = "-----BEGIN PUBLIC KEY-----" + Environment.NewLine +
                                "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCy5CulBZvqtUqL1w5iaO6lJibW" + Environment.NewLine +
                                "CU3yuIes+K65cxZVDscu0i6KPju1ktOPCVxSsWybMEejXsX0xn1Vyx6Aglnp+cd2" + Environment.NewLine +
                                "c6xpIAwg9e6N9G95R+tlh5efhDB3f+RoUXzOmqpqtjs0KdUDIbsJ68W4OMRIDL6A" + Environment.NewLine +
                                "C8ae1GgDbw6Areb7kwIDAQAB" + Environment.NewLine +
                                "-----END PUBLIC KEY-----";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("merchantOrderNo", "C2221122234");
            param.Add("amount", 100);
            param.Add("type", 0);
            param.Add("notifyUrl", "http://www.baidu.com/callback");
            param.Add("remark", "xxxxxxx");
            param.Add("ip", "45.58.55.55");

            string data = Crypto.EncryptToBase64(publicKey, JsonConvert.SerializeObject(param));
            string result = DoPost("https://user.wbzf.info:4431/merchant-api/api/open/createOrder", "merchantId=800000&data=" + HttpUtility.UrlEncode(data, Encoding.UTF8));
            Console.WriteLine(result);
            JObject resultJson = (JObject)JsonConvert.DeserializeObject(result);
            if (int.Parse(resultJson.GetValue("code").ToString()) == 0)
            {
                Console.WriteLine(Crypto.DecryptFromBase64(publicKey, resultJson.GetValue("data").ToString()));
            }
            Console.Read();
        }

        public static string DoPost(string url, string data)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            string strResult = string.Empty;
            try
            {
                request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                if (data != null)
                {
                    request.AllowWriteStreamBuffering = true;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
                        requestStream.Flush();
                    }
                }
                response = (HttpWebResponse)request.GetResponse();
                StreamReader srReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                response.Close();
            }
            catch (System.Exception err)
            {
                Console.Write(err.StackTrace);
            }
            return strResult;
        }
    }
}
