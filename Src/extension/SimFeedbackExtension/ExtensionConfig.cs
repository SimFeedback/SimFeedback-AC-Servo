using SimFeedback.conf;
using System.Xml.Serialization;

namespace SimFeedback.extension
{
    public class ExtensionConfig
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("autostart")]
        public bool AutoStart;
        [XmlAttribute("activated")]
        public bool Activated;
        [XmlElement( ElementName = "CustomConfig", IsNullable = true)]
        public CustomConfigReader CustomConfigReader;

        [XmlIgnore]
        public ICustomConfig CustomConfig {

            get { return CustomConfigReader.CustomConfig; }
            set { CustomConfigReader.CustomConfig = value; }
        }
    }
}
