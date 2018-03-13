using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace RI
{
    public class CustomJsonConverter : JsonConverter
    {
        public static CustomJsonConverter Default = new CustomJsonConverter();
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            return objectType;
            //    throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            JObject jObject = (JObject)t;
            // IList<string> propertyNames = jObject.Properties().Select(p => p.Name).ToList();
            if (t.Type != JTokenType.Object)
            {
                Debug.Log(t.Type + " is JTokenType.Object!");
            }

            foreach (var j in jObject.PropertyValues())
            {
                //                Debug.Log(j.Type);
            }
        }
    }

    public class DataFile
    {
        public object data { get; private set; }
        public FileInfo fileInfo { get; private set; }

        public DataFile(FileInfo info, string json, System.Type type)
        {
            this.fileInfo = info;
            FromJson(type, json);
        }

        public DataFile(FileInfo info, object data)
        {
            this.fileInfo = info;
            this.data = data;
        }
        public string ToJson()
        {
            Debug.Log(data.ToString());
            return JsonUtility.ToJson(data, true);
            return JsonConvert.SerializeObject(data, Formatting.Indented, new CustomJsonConverter());
        }
        public void FromJson(System.Type type, string val)
        {
            data = JsonUtility.FromJson(val, type);
            //   data = JsonConvert.DeserializeObject(val, type, CustomJsonConverter.Default);
        }


        public static DataFile Create(object dataFile)
        {
            System.Type type = dataFile.GetType();
            string directoryPath = Application.dataPath + "/Generated/" + type;
            string newFilename = System.String.Format(type + "-{0}.txt", System.DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss"));
            DataFile df = new DataFile(new FileInfo(directoryPath + "/" + newFilename), dataFile);
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            File.WriteAllText(directoryPath + "/" + newFilename, df.ToJson());
            return df;
        }

        public static DirectoryInfo GetDirectoryInfo(string path)
        {
            if (!Directory.Exists(path))
                return Directory.CreateDirectory(path);
            else return new DirectoryInfo(path);
        }
        public static void Ammend(DataFile dataFile)
        {
            Debug.Log("Ammend");
            File.WriteAllText(dataFile.fileInfo.FullName, dataFile.ToJson());
        }
        public static DataFile[] LoadAll()
        {
            DirectoryInfo di = GetDirectoryInfo(Application.dataPath + "/Generated/");
            FileInfo[] fileInfos = di.GetFiles("*.txt", SearchOption.AllDirectories);
            return fileInfos.Where(w => System.Type.GetType(w.Directory.Name) != null).Select(x => new DataFile(x, File.ReadAllText(x.FullName), System.Type.GetType(x.Directory.Name))).ToArray();
        }

    }

}