using System;
using System.Runtime.Serialization;

namespace PriveSports.Models
{
    public class User
    {
        [DataMember]
        public string label { get; set; }

        [DataMember]
        public string bookmark { get; set; }

        [DataMember]
        public string value { get; set; }

        [DataMember]
        public string any { get; set; }

        [DataMember]
        public string other { get; set; }
    }
}
