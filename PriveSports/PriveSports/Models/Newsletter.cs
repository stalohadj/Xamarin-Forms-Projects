using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PriveSports.Models
{
    public class Newsletter
    {
        [DataMember]
        public string date { get; set; }

        [JsonProperty(PropertyName = "$$record")]
        public string position { get; set; }

        [DataMember]
        public string link { get; set; }

        [DataMember]
        public string name { get; set; }
    }
}
