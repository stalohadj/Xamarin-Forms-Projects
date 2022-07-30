using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PriveSports.Models
{
    public class TransDetails
    {
            [JsonProperty(PropertyName = "item.name")]
            public string item { get; set; }

            [JsonProperty(PropertyName = "item.price")]
            public string total { get; set; }

            [DataMember]
            public double discount { get; set; }
    }
}
