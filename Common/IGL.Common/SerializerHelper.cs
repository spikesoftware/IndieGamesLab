using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace IGL
{
    public static class XmlSerializerHelper
    {
        public static string Serialize<T>(T obj)
        {
            var outStream = new StringWriter();
            var ser = new XmlSerializer(typeof(T));
            ser.Serialize(outStream, obj);
            return outStream.ToString();
        }

        public static T Deserialize<T>(string serialized)
        {
            var inStream = new StringReader(serialized);
            var ser = new XmlSerializer(typeof(T));
            return (T)ser.Deserialize(inStream);
        }

    }

    public static class DatacontractSerializerHelper
    {
        public static string Serialize<T>(T item)
        {
            if (item != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(memoryStream, item);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
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
                    xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(item), XmlDictionaryReaderQuotas.Max);
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    return (T)serializer.ReadObject(xmlDictionaryReader, false);
                }
                finally
                {
                    if (xmlDictionaryReader != null)
                    {
                        xmlDictionaryReader.Close();
                    }
                }
            }
            return default(T);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            if (bytes.Length > 0)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                    return (T)serializer.ReadObject(stream);
                }
            }
            return default(T);
        }

    }

    public static class JsonSerializerHelper
    {
        public static string Serialize(object item)
        {
            if (item != null)
            {
                var serializer = new JavaScriptSerializer();
                return serializer.Serialize(item);
            }
            return null;
        }

        public static T Deserialize<T>(string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(item);
            }
            return default(T);
        }

    }
}
