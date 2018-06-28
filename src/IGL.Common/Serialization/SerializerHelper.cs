using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace IGL.Serialization
{
    public static class DatacontractSerializerHelper
    {
        public static string Serialize<T>(T item, List<Type> knownTypes = null)
        {
            if (item != null)
                using (var memoryStream = new MemoryStream())
                {
                    // List<Type> knownTypes = new List<Type> { typeof(List<string>), typeof(NameValueCollection) };
                    var serializer = new DataContractSerializer(typeof(T), knownTypes);
                    serializer.WriteObject(memoryStream, item);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            return null;
        }

        public static T Deserialize<T>(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                XmlDictionaryReader xmlDictionaryReader = null;
                try
                {
                    xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(item),
                        XmlDictionaryReaderQuotas.Max);
                    var serializer = new DataContractSerializer(typeof(T));
                    return (T) serializer.ReadObject(xmlDictionaryReader, false);
                }
                finally
                {
                    if (xmlDictionaryReader != null)
                        xmlDictionaryReader.Close();
                }
            }
            return default(T);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            if (bytes.Length > 0)
                using (var stream = new MemoryStream(bytes))
                {
                    var serializer = new DataContractSerializer(typeof(T));

                    return (T) serializer.ReadObject(stream);
                }
            return default(T);
        }
    }

    public static class JsonSerializerHelper
    {
        public static string Serialize(object item)
        {
            if (item != null)
                return JsonConvert.SerializeObject(item);
            return null;
        }

        public static T Deserialize<T>(string item)
        {
            if (!string.IsNullOrEmpty(item))
                return JsonConvert.DeserializeObject<T>(item);
            return default(T);
        }
    }

    public static class GameEventSerializer
    {
        public static string Serialize(GameEvent item)
        {
            string response;
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                writer.Write(item.Properties.Count);

                foreach (var property in item.Properties)
                {
                    writer.Write(property.Key);
                    writer.Write(property.Value);
                }

                stream.Position = 0;
                response = Convert.ToBase64String(stream.ToArray());
                stream.Close();
            }

            return response;
        }

        public static GameEvent Deserialize(string content)
        {
            var bytes = Convert.FromBase64String(content);

            var gameEvent = new GameEvent {Properties = new Dictionary<string, string>()};

            using (var reader = new BinaryReader(new MemoryStream(bytes)))
            {
                var numProperties = reader.ReadInt32();

                for (var index = 0; index < numProperties; index++)
                    gameEvent.Properties.Add(reader.ReadString(), reader.ReadString());
            }

            return gameEvent;
        }
    }
}