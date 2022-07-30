using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PriveSportsEmployees.Controls
{
    public class Employee
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string surname { get; set; }
        [DataMember]
        public string mobile { get; set; }
        [JsonProperty(PropertyName = "$$position")]
        public int posnum { get; set; }
        [DataMember]
        public string GUID { get; set; }
        [DataMember]
        public string position { get; set; }
        [DataMember]
        public string location { get; set; }
    }
}
