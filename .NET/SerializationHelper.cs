using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Helpers
{
    public static class SerializationHelper
    {
        private static readonly Dictionary<Type, XmlSerializer> Serializers;
        static SerializationHelper()
        {
            Serializers = new Dictionary<Type, XmlSerializer>();
        }

        private static XmlSerializer GetSerializer(Type type)
        {
            //If it exists, return it
            if ((Serializers.ContainsKey(type)))
            {
                return Serializers[type];
            }
            //If not, create it, add it to the list and return it.
            var xmlSerializer = new XmlSerializer(type);
            Serializers.Add(type, xmlSerializer);
            return xmlSerializer;
        }

        /// <summary>
        /// Serializes the object to XML using the given xml writer settings.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlWriterSettings"></param>
        /// <returns></returns>
        public static string ToXml<T>(this T obj, XmlWriterSettings xmlWriterSettings)
        {
            if (Equals(obj, default(T)))
            {
                return String.Empty;
            }

            using (var memoryStream = new MemoryStream())
            {
                var xmlSerializer = GetSerializer(obj.GetType());

                //Remove the xmlns attribute.
                var xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add(String.Empty, String.Empty);

                using (var xmlTextWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    xmlSerializer.Serialize(xmlTextWriter, obj, xmlnsEmpty);
                    memoryStream.Seek(0, SeekOrigin.Begin); //Rewind the Stream.
                }

                var xml = xmlWriterSettings.Encoding.GetString(memoryStream.ToArray());
                return xml;
            }
        }

        /// <summary>
        /// Serializes the object to XML, with or without the declaration.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="omitXmlDeclaration">Gets or sets a value indicating whether to write an XML declaration.</param>
        /// <returns></returns>
        public static string ToXml<T>(this T obj, bool omitXmlDeclaration)
        {
            return obj.ToXml(new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = omitXmlDeclaration
            });
        }

        /// <summary>
        /// Serializes the object to XML
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml<T>(this T obj)
        {
            return obj.ToXml(false);
        }

        /// <summary>
        /// Deserializes the object from xml.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T FromXml<T>(this string xml)
        {
            var s = GetSerializer(typeof(T));
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Auto };
            using (var stringReader = new StringReader(xml))
            {
                using (var xmlReader = XmlReader.Create(stringReader, settings))
                {
                    var obj = s.Deserialize(xmlReader);
                    return (T)obj;
                }
            }
        }


        /// <summary>
        /// Serializes the object to DataContract
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(this T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Deserializes the object from DataContract.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return (T)deserializer.ReadObject(stream);
            }
        }

        public static string ToBinary<T>(this T obj)
        {
            if (Equals(obj, default(T)))
            {
                return null;
            }

            string serializedData;
            var binaryFormatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                serializedData = Convert.ToBase64String(memoryStream.ToArray());
            }

            return serializedData;
        }

        public static T FromBinary<T>(this string rawData)
        {
            if (rawData == null)
            {
                return default(T);
            }

            var streamData = Convert.FromBase64String(rawData);
            var binaryFormatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream(streamData))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);

                return result;
            }
        }
    }
}