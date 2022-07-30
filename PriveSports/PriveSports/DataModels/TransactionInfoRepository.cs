using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;

namespace PriveSports.DataModels
{

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

    public class TransactionInfoRepository
    {
      
        public string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public string IPtableview = "http://139.138.223.53:8080/$TableGetView?";
        public string IP = "11.0.0.99";
        public string IP2 = "11.0.0.138";

      
        private ObservableCollection<Transaction> transinfo;
        public ObservableCollection<Transaction> TransInfoCollection
        {
            get { return transinfo; }
            set { this.transinfo = value; }
        }
        public TransactionInfoRepository()
        {
            transinfo = new ObservableCollection<Transaction>();
            this.GenerateTransactions();
        }

        private void GenerateTransactions()
        {
            try
            {
                string phone = Application.Current.Properties["phone"] as string;
                Hashtable ht = new Hashtable();
                ht.Add("user", "root");
                ht.Add("session-token", "tesT");
                string response2 = DoGetHttp(IPfind + phone, "", 10000);
               // var des = JsonConvert.DeserializeObject<List<us>>(response2);
                if (response2 != "[]")
                {
                    var des = JsonConvert.DeserializeObject<List<us>>(response2);
                    string trs = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields=refer time date location _total points&record=" + des[0].value, "", 10000);//Getting the Details

                  //  var ti = JsonConvert.DeserializeObject<List<trans>>(transactions);


                    string details = DoGetHttp(IPtableview + "fields=item.name item.price discount&system=pos&file=transdet&driving=trans.people&from=" + des[0].value, "", 10000);//Getting the Details
                    var detail = JsonConvert.DeserializeObject<List<Details>>(details);


                    string loc = DoGetHttp(IPtableview + "fields=location&system=pos&file=transdet&driving=trans.people&from=" + des[0].value, "", 10000);//Getting the Details
                    var location = JsonConvert.DeserializeObject<List<Location>>(loc);


                    string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields=refer time date location _total points&record=" + des[0].value, "", 10000);//Getting the Details

                    var ti = JsonConvert.DeserializeObject<List<Transaction>>(transactions);

                    string itemname = DoGetHttp(IPtableview + "report_fields=item&report=web_points&system=pos&file=transdet&record=" + ti[0].Position + "&driving=trans", "", 10000);
                    var nam = JsonConvert.DeserializeObject<List<Transaction>>(itemname);
                    int i = 0;
                    int sx = detail.Count;
                    string items;
                    string locs;

                    foreach (var obj in ti)
                    {
                        for (i = 0; i < sx; i++)
                        {
                            
                        }

                        transinfo.Add(new Transaction(obj.Refer, obj.Date, obj.Time, obj.Item, obj.Location, obj.Total));

                    }

                    
                    
                }
            }

            
            catch (Exception)
            {

            }
           
        }

        public void Refresh()
        {
            try
            {
                string phone = Application.Current.Properties["phone"] as string;
                Hashtable ht = new Hashtable();
                ht.Add("user", "root");
                ht.Add("session-token", "tesT");
                string response2 = DoGetHttp(IPfind + phone, "", 10000);
                var des = JsonConvert.DeserializeObject<List<us>>(response2);
                if (response2 != "[]")
                {
                    //  var des = JsonConvert.DeserializeObject<List<us>>(response2);
                    // string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields=refer time date location _total points&record=" + des[0].value, "", 10000);//Getting the Details

                    // var ti = JsonConvert.DeserializeObject<List<Transaction>>(transactions);

                    //  string itemname = DoGetHttp(IPtableview + "report_fields=item&report=web_points&system=pos&file=transdet&record=" + ti[0].position + "&driving=trans", "", 10000);
                    //   var nam = JsonConvert.DeserializeObject<List<Transaction>>(itemname);

                    //  int size = ti.Count;
                    // int size2 = nam.Count;

                    string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields=refer time date location _total points&record=" + des[0].value, "", 10000);//Getting the Details

                    var ti = JsonConvert.DeserializeObject<List<Transaction>>(transactions);

                    string itemname = DoGetHttp(IPtableview + "report_fields=item&report=web_points&system=pos&file=transdet&record=" + ti[0].Position + "&driving=trans", "", 10000);
                    var nam = JsonConvert.DeserializeObject<List<Transaction>>(itemname);
                    int i = 0;

                    foreach (var obj in ti)
                    {

                        transinfo.Add(new Transaction(obj.Refer, obj.Date, obj.Time, obj.Item = nam[i].Item, obj.Location = nam[i].Location, obj.Total));

                        i++;
                    }
                }
            }


            catch (Exception)
            {

            }
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

