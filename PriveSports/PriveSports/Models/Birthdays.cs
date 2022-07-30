using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PriveSports.Models
{
    public class Birthdays
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string birth { get; set; }
        [DataMember]
        public string nameday { get; set; }
        [JsonProperty(PropertyName = "$$record")]
        public string record { get; set; }
    }
}
