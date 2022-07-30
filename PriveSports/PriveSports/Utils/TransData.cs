using System;
using System.Collections.Generic;
using PriveSports.ViewModels;
using PriveSports.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xamarin.Forms;
namespace PriveSports.Utils
{

    public class TransData
    {
        public string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public string IPtableview = "http://139.138.223.53:8080/$TableGetView?";
        public string IP = "11.0.0.99";
        public string IP2 = "11.0.0.138";

        public class Location
        {
            [DataMember]
            public string location { get; set; }
        }

        public class Details
        {
            [JsonProperty(PropertyName = "item.name")]
            public string name { get; set; }

            [JsonProperty(PropertyName = "item.price")]
            public string price { get; set; }

            [DataMember]
            public double discount { get; set; }

        }

        public class us
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

        public List<Transactions> GetTransactions()
        {
            Hashtable ht = new Hashtable();
            ht.Add("user", "root");
            ht.Add("session-token", "tesT");
            var data = new List<Transactions>();
            string response2 = DoGetHttp(IPfind + Application.Current.Properties["phone"] as string, "", 10000);
            if (response2 != "[]")
            {
                var des = JsonConvert.DeserializeObject<List<us>>(response2);
                string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields=refer time date location _total points&record=" + des[0].value, "", 10000);//Getting the Details

                var ti = JsonConvert.DeserializeObject<List<Transactions>>(transactions);


                string details = DoGetHttp(IPtableview + "fields=item.name item.price discount&system=pos&file=transdet&driving=trans.people&from=" + des[0].value, "", 10000);//Getting the Details

                var detail = JsonConvert.DeserializeObject<List<Details>>(details,
                new JsonSerializerSettings
                {
                    Error = delegate
                    {

                    },

                });

                string loc = DoGetHttp(IPtableview + "fields=location&system=pos&file=transdet&driving=trans.people&from=" + des[0].value, "", 10000);//Getting the Details
                var location = JsonConvert.DeserializeObject<List<Location>>(loc);

                int size = ti.Count;
                int size2 = detail.Count;
                string itemdet = "";
                var trans = new Transactions();
                for (int i = 0; i < size; i++)
                {
                    string total = "0";
                    int points = 0;
                    if (ti[i]._total != null)
                    {

                        total = ti[i]._total;

                    }
                    if (ti[i].points != 0)
                    {

                        points = ti[i].points;
                    }
                    trans = new Transactions()
                    {
                        refer = ti[i].refer,
                        date = ti[i].date,
                        time = ti[i].time,
                        item = ti[i].item,
                        location = ti[i].location,
                        _total = ti[i]._total

                    };


                }
                for (int i = 0; i < 10; i++)
                {
                    data.Add(trans);

                }

               
            }
            return data;
        }


        public string DoGetHttp(string sUri, string sHeaders, int nTimeout)
        {
            Hashtable ht = new Hashtable();

            if (!string.IsNullOrEmpty(sHeaders))
            {
                string[] headers = sHeaders.Split('\n');
                foreach (string header in headers)
                {
                    string[] parts = header.Split('\t');

                    ht.Add(parts[0], parts[1]);
                }
            }

            string post_guid = new Guid().ToString();


            ht.Add("User", "root");
            ht.Add("Session-Token", post_guid);


            WebResponse response = (WebResponse)_GetHttp(sUri, ht, nTimeout);
            if (response == null)
                return "";

            try
            {
                Stream s = response.GetResponseStream();
                StreamReader reader = new StreamReader(s);

                string tmp = reader.ReadToEnd();
                response.Close();
                return tmp;
            }
            catch (Exception e)
            {

            }

            return "";

        }
        static public Object _GetHttp(string sUri, Hashtable ht, int nTimeout)
        {
            Uri uri = new Uri(sUri);

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                HttpWebRequest request = null;
                try
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Method = WebRequestMethods.Http.Get;
                    request.Timeout = nTimeout;

                    if (ht != null)
                        foreach (DictionaryEntry pair in ht)
                            request.Headers.Add((string)pair.Key, (string)pair.Value);

                    return request.GetResponse();


                }
                catch (Exception e)
                {
                    string message = "Exception on uri: " + sUri + ", Error: " + e.Message + " Timeout: " + nTimeout;
                    Console.WriteLine(message);

                    return null;
                }
            }
            else


                return null;
        }
    }

}

}
