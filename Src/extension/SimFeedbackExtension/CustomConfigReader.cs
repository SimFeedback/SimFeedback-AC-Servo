using SimFeedback.extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace SimFeedback.conf
{
    public class CustomConfigReader : IXmlSerializable
    {
        [XmlIgnore]
        public ICustomConfig CustomConfig;

        public void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.Name.Equals("CustomConfig"))
                {
                    string strType = reader.GetAttribute("type");
                    Type type = LoadType(strType);
                    if (type != null)
                    {
                        while (true)
                        {
                            if (reader.Name.Equals(type.Name))
                            {
                                XmlSerializer serializer = new XmlSerializer(type);
                                CustomConfig = (ICustomConfig)serializer.Deserialize(reader);
                            }

                            if(reader.Name.Equals("CustomConfig") && !reader.IsStartElement())
                            {
                                reader.Read();
                                break;
                            }
                            reader.Read();
                        }
                    }
                }
            } catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }



        public void WriteXml(XmlWriter writer)
        {
            if (CustomConfig != null)
            {
                writer.WriteAttributeString("type", CustomConfig.GetType().FullName);
                XmlSerializer serializer = new XmlSerializer(CustomConfig.GetType(), new XmlRootAttribute(CustomConfig.GetType().Name));
                serializer.Serialize(writer, CustomConfig);
            }
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        
        public ICustomConfig GetCustomConfig()
        {
            return CustomConfig;
        }


        private Type LoadType(string typeName)
        {
            string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extensions");

            if (!Directory.Exists(binPath))
            {
                return null;
            }

            // Load all DLL provider
            foreach (string dll in Directory.GetFiles(binPath, "*.dll", SearchOption.AllDirectories))
            {
                Assembly loadedAssembly = Assembly.LoadFile(dll);
                Type myType = loadedAssembly.GetTypes().FirstOrDefault(s => s.FullName.Equals(typeName));
                if (myType != null)
                    return myType;
            }
            return null;
        }


    }
}