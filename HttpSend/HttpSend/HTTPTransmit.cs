using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace HttpSend
{
    public class HTTPTransmit
    {
        public static string Send(string url, object data)
        {
            string jsonStr = JsonConvert.SerializeObject(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";
            byte[] payload = Encoding.UTF8.GetBytes(jsonStr);
            request.ContentLength = payload.Length;
            Stream writer = null;
            try
            {
                writer = request.GetRequestStream();
            }
            catch (Exception)
            {
                return "fail";
            }
            writer.Write(payload, 0, payload.Length);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return "fail";
            }
            writer.Close();
            return "success";
        }
    }
}
