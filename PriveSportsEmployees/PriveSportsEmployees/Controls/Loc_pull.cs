using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PriveSportsEmployees.Controls
{
    public class Loc_pull
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string add1 { get; set; }
        [DataMember]
        public string longitude { get; set; }
        [DataMember]
        public string latitude { get; set; }
        //  [JsonProperty(PropertyName = "$longitude")]
    }
}
