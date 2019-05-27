﻿using System.IO;
using Newtonsoft.Json;

namespace EFTLauncher.Utility
{
    public static class JsonHelper
    {
        public static T LoadJson<T>(string filepath)
        {
            T data;

            // load the json data
            using (StreamReader sr = new StreamReader(filepath))
            {
                string json = sr.ReadToEnd();
                data = JsonConvert.DeserializeObject<T>(json);
            }

            return data;
        }

        public static string ReadJson(string filepath)
        {
            string data = "";

            // read the json data
            using (StreamReader sr = new StreamReader(filepath))
            {
                data = sr.ReadToEnd();
            }

            return data;
        }

        public static string SaveJson<T>(string filepath, T obj)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(filepath))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(sw, obj);
                }
            }

            return "";
        }

        public static string NormalizeJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }
    }
}
