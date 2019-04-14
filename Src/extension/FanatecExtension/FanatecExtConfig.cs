using SimFeedback.conf;
using System;

namespace SimFeedback.extension.fanatec
{
    [Serializable]
    public class FanatecExtConfig : ICustomConfig
    {
        public int Test { get; set; }
    }


}
