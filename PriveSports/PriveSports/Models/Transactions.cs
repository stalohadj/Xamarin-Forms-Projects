using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace PriveSports.Models
{
    public class Transactions
    {
        public Transactions(string division, string date, string time, string refer, int points, string location, string _raw_total, string _total)
        {
            this.division = division;
            this.date = date;
            this.time = time;
            this.refer = refer;
            this.item = item;
            this.points = points;
            this.location = location;
            this._raw_total = _raw_total;
            this._total = _total;
           


        }
        [DataMember]
        public string division { get; set; }

        [DataMember]
        public string refer { get; set; }

        [DataMember]
        public string date { get; set; }

        [DataMember]
        public string time { get; set; }

        [DataMember]
        public int points { get; set; }

        [DataMember]
        public string item { get; set; }

        [DataMember]
        public string location { get; set; }

        [DataMember]
        public string _raw_total { get; set; }

        [DataMember]
        public string _total { get; set; }

        [JsonProperty(PropertyName = "$$position")]
        public string position { get; set; }

        

        // [JsonProperty(PropertyName = "@modify_stamp")]
        // public string stamp { get; set; }

    }

}
