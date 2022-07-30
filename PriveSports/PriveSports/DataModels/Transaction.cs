using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PriveSports.DataModels
{
    public class Transaction
    {
        private string refer;
        private string date;
        private string time;
        private int points;
        private string item;
        private string location;
        private string _total;
        private string position;

       
        [DataMember]
        public string Refer { get { return refer; } set { this.refer = value; } }

        [DataMember]
        public string Date { get { return date; } set { this.date = value; } }

        [DataMember]
        public string Time { get { return time; } set { this.time = value; } }

        [DataMember]
        public int Points { get { return points; } set { this.points = value; } }

        [DataMember]
        public string Item { get { return item; } set { this.item = value; } }

        [DataMember]
        public string Location { get { return location; } set { this.location = value; } }

        [DataMember]
        public string Total { get { return _total; } set { this._total = value; } }

        [JsonProperty(PropertyName = "$$position")]
        public string Position { get { return position; } set { this.position = value; } }


        public Transaction(string refer, string date, string time, string item, string location, string _total)
        {
            this.Refer = refer;
            this.Date = date;
            this.Time = time;
            this.Item = item;
            this.Location = location;
            this.Total = _total;

        }


    }
}
