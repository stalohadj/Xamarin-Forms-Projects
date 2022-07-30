using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms.Core;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xamarin.Forms;
using PriveSports.Models;
using System.Diagnostics;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.Linq;
using Rg.Plugins.Popup.Extensions;
using PriveSports.Views;

namespace PriveSports.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class Transaction2ViewModel : BaseViewModel
    {
        public static string trs = "";
        public static string dt;
        public static string tp;
        public float grandtotal;
        string res;
        string res1;
        string res2;
        string points; 
        public class item_mod 
        {
            [JsonProperty(PropertyName = "item.price")]
            public string price { get; set; }

            [JsonProperty(PropertyName = "item.name")]
            public string name { get; set; }

            [JsonProperty(PropertyName = "@modify_stamp")]
            public string stamp { get; set; }

            [JsonProperty(PropertyName = "$trans")]
            public string trans { get; set; }

            [JsonProperty(PropertyName = "trans.date")]
            public string date { get; set; }

           
        }


        public class trans_mod 
        {
            [DataMember]
            public string division { get; set; }

            [DataMember]
            public string refer { get; set; }

            [DataMember]
            public string date { get; set; }

            [DataMember]
            public string time { get; set; }

            [DataMember]
            public string location { get; set; }

            [DataMember]
            public int points { get; set; }

            [DataMember]
            public string _total { get; set; }

            [DataMember]
            public string _raw_total { get; set; }

            [DataMember]
            public string _raw_discount { get; set; }

            [JsonProperty(PropertyName = "$$position")]
            public string position { get; set; }

            [JsonProperty(PropertyName = "@modify_stamp")]
            public string stamp { get; set; }

            [DataMember]
            public string _raw_voucher_discount { get; set; }

            [JsonProperty(PropertyName = "$trans")]
            public string trans { get; set; }

           
        }

        public string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public string IPtableview = "http://139.138.223.53:8080/$TableGetView?"; //system=account&file=people&report=BIRTHDAY
        public string IP = "11.0.0.99";
        public string IP2 = "11.0.0.138";

        private string Phone = Application.Current.Properties["phone"] as string;
        private string Name = Application.Current.Properties["name"] as string;

       public ICommand LoadTransactionCommand => new Command<string[]>( (x) =>  LoadTransactionAsync(x));
       public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    RefreshCmd();

                    IsRefreshing = false;
                });
            }
        }//=> new Command(() => RefreshCmd());



        private Transaction _oldtrans;
        public ObservableCollection<Transaction> Transactions { get; set; }

        public Transaction2ViewModel()
        {
            Transactions = new ObservableCollection<Transaction>();

            Hashtable ht = new Hashtable();
            ht.Add("user", "root");
            ht.Add("session-token", "tesT");
            try
            {
                string response2 = DoGetHttp(IPfind + Phone, "", 10000);
                if (response2 != "[]")
                {
                   

                    var des = JsonConvert.DeserializeObject<List<User>>(response2);
                    string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields= division refer time date location _total _raw_total _raw_discount points _raw_voucher_discount&record=" + des[0].value, "", 10000);//Getting the Details
            
                    if (transactions == "" || transactions == null || transactions == " ")
                    {
                        
                        Transactions.Add(new Transaction { Details = "No transactions found! Make a purchase at one of our stores and refresh the page to see it here!", refer= "No Transactions!", Isvisible = false });
                    }
                    else
                    {
                        var t = JsonConvert.DeserializeObject<List<trans_mod>>(transactions);

                        int sx = t.Count();

                        for(int i = sx-1; i>=0; i--)
                        {
                            float x;
                            Console.WriteLine(t[i].date);
                            //div = t[i].refer;
                            if (t[i]._total == null)
                            {
                                if (t[i].division != "TRANSFER IN" && t[i].division != "TRANSFER OUT" && t[i].division != "TRANSFER REQUEST" && t[i].division != "TRANSFER")
                                {
                                    Transactions.Add(new Transaction { Details = "DIVISION: " + t[i].division + "\n" + "DATE: " + t[i].date + "\n" + "LOCATION: " + t[i].location + "\n", Isvisible = true, Date = t[i].date, refer = "#" + t[i].refer, passthrough = new string[5] { t[i].date, t[i]._raw_discount, t[i].points.ToString(), t[i]._raw_total, t[i]._raw_voucher_discount } });
                                }
                            }
                            else
                            {
                                if (t[i].division != "TRANSFER IN" && t[i].division != "TRANSFER OUT" && t[i].division != "TRANSFER REQUEST" && t[i].division != "TRANSFER")
                                {
                                    if (t[i]._total.Contains('-'))
                                    {
                                        string new_t = t[i]._total.Replace('-', ' ');
                                        Transactions.Add(new Transaction { Details = "DIVISION: " + t[i].division + "\n" + "DATE: " + t[i].date + "\n" + "LOCATION: " + t[i].location + "\n" + "TOTAL: €" + new_t, Isvisible = true, Date = t[i].date, refer = "#" + t[i].refer, passthrough = new string[5] { t[i].date, t[i]._raw_discount, t[i].points.ToString(), t[i]._raw_total, t[i]._raw_voucher_discount } });

                                    }
                                    else
                                    {
                                        Transactions.Add(new Transaction { Details = "DIVISION: " + t[i].division + "\n" + "DATE: " + t[i].date + "\n" + "LOCATION: " + t[i].location + "\n" + "TOTAL: €" + t[i]._total, Isvisible = true, Date = t[i].date, refer = "#" + t[i].refer, passthrough = new string[5] { t[i].date, t[i]._raw_discount, t[i].points.ToString(), t[i]._raw_total, t[i]._raw_voucher_discount } });
                                    }
                                }
                            }
                        }
                        /*
                         * 
                         *  Details = t[i].refer + " | Date: " + t[i].date + " | Time: " + t[i].time + " | Location: " + t[i].location + " | Total: €" + t[i]._total + " | Discount: €" + t[i]._discount + " | Points Received: " + t[i].points, Isvisible = false, Date = t[i].date, refer = t[i].refer
                        foreach (var obj in t)
                        {
                            Transactions.Add(new Transaction { Details = obj.refer + " | Date: " + obj.date + " | Time: " + obj.time + " | Location: " + obj.location + " | Total: €" + obj._total + " | Discount: €" + obj._discount + " | Points Received: " + obj.points, Isvisible = false, Date = obj.date, refer = obj.refer });
                            
                        }
                        Transactions.OrderByDescending(Transaction => Convert.ToDateTime(Transaction.Date));*/

                    }



                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace + "\n " + e.InnerException + "\n" + e.GetBaseException());
                Application.Current.MainPage.DisplayAlert("AN ERROR HAS OCCURED", "PLEASE CHECK YOUR NETWORK CONNECTION AND TRY AGAIN LATER.", "OK");
            }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }


        private void FillSlot(object obj)
        {
            string[] arr = ((IEnumerable)obj).Cast<object>()
                            .Select(x => x.ToString())
                            .ToArray();//Way of accessing multiple parameters binded in xaml
            string KoderName = arr[0];
            string KoderEmail = arr[1];
            Console.WriteLine("Koder Name.." + KoderName + "\n" + "Koder Email.." + KoderEmail);
        }
        public async Task LoadTransactionAsync(string[] pass)
        {

           

            trs = "";
            Console.WriteLine("TEST: " + pass[0]);
            Hashtable ht = new Hashtable();
            ht.Add("user", "root");
            ht.Add("session-token", "tesT");
            try
            {
                string response2 = DoGetHttp(IPfind + Phone, "", 10000);
                if (response2 != "[]")
                {
                    var des = JsonConvert.DeserializeObject<List<User>>(response2);
                   // string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields= division refer time date location _total _discount points&record=" + des[0].value, "", 10000);
                    string itm = DoGetHttp(IPtableview + "fields=item.name trans.date item.price trans&system=pos&file=transdet&driving=trans.people&__internal=true&from=" + des[0].value, "", 10000);//Getting the Details //des[0].value
                    var it = JsonConvert.DeserializeObject<List<item_mod>>(itm);

                    var q1 = it.GroupBy(x => x.date)
                                     .Select(g => new
                                     {
                                         Date = g.Key,
                                         Details = g.Select(x => new
                                         {
                                             Name = x.name,
                                             Price = x.price,
                                             Trans = x.trans,
                                         })
                                       .ToList()
                                     })
                             .ToList();

                    foreach(var obj in q1)
                    {
                        if (obj.Date == pass[0])
                        {

                            foreach (var x in obj.Details)
                            {
                                trs = "∙ " + x.Name + ".... €" + x.Price + "\n" + trs;
                                dt = "Transaction #" + x.Trans;

                                //1 [i]._discount, 2 t[i].points.ToString(), 3 t[i]._total, 4 t[i].v_discount } });


                                switch (pass[1])
                                {
                                    case null:
                                        res1 = "0";
                                        break;
                                    case "":
                                        res1 = "0";
                                        break;
                                    default:
                                        if (pass[1].Contains(',') == true)
                                        {
                                            res1 = pass[1].Replace(',', '.');
                                        }
                                        else
                                        {
                                            res1 = pass[1];
                                        }
                                        break;
                                }

                                Console.WriteLine(res1);
                               // Debugger.Break();
                                switch (pass[2])
                                {
                                    case null:
                                        points = "0";
                                        break;
                                    case "":
                                        points = "0";
                                        break;
                                    default:
                                        points = pass[2];
                                        break;

                                }
                                Console.WriteLine(points);
                              //  Debugger.Break();
                                switch (pass[4])
                                {
                                    case null:
                                        res2 = "0";
                                        break;
                                    case "":
                                        res2 = "0";
                                        break;
                                    default:
                                        if (pass[4].Contains(',') == true)
                                        {
                                            res2 = pass[4].Replace(',', '.');
                                        }
                                        else
                                        {
                                            res2 = pass[4];
                                        }
                                        break;
                                }

                                Console.WriteLine(res2);
                                //Debugger.Break();
                                switch (pass[3])
                                {
                                    case null:
                                        res = "";
                                        tp = "TOTAL: €" + "0" + "\n" + "DISCOUNT: €" + res1 + "\n" + "POINTS COLLECTED: " + points + "\n" + "VOUCHER DISCOUNT: €" + res2;
                                        break;
                                    case "":
                                        res = "";
                                        tp = "TOTAL: €" + "0" + "\n" + "DISCOUNT: €" + res1 + "\n" + "POINTS COLLECTED: " + points + "\n" + "VOUCHER DISCOUNT: €" + res2;
                                        break;
                                    default:
                                        if (pass[3].Contains(',') == true)
                                        {
                                            res = pass[3].Replace(',', '.');
                                            if (float.Parse(res) > 0)
                                            {
                                                grandtotal = float.Parse(res) - float.Parse(res1) - float.Parse(res2);
                                                tp = "TOTAL: €" + res + "\n" + "DISCOUNT: €" + res1 + "\n" + "VOUCHER DISCOUNT: €" + res2 + "\n" + "TOTAL COLLECTED: €" + grandtotal + "\n" + "POINTS COLLECTED: " + points;
                                            }
                                            else
                                            {
                                                grandtotal = float.Parse(res) + float.Parse(res1) + float.Parse(res2);
                                                if (grandtotal.ToString().Contains('-'))
                                                {
                                                    string new_t = grandtotal.ToString().Replace('-', ' ');
                                                    res = res.Replace('-', ' ');
                                                    tp = "TOTAL: €" + res + "\n" + "DISCOUNT: €" + res1 + "\n" + "VOUCHER DISCOUNT: €" + res2 + "\n" + "TOTAL COLLECTED: €" + new_t + "\n" + "POINTS COLLECTED: " + points;

                                                }
                                                else
                                                {
                                                    tp = "TOTAL: €" + res + "\n" + "DISCOUNT: €" + res1 + "\n" + "VOUCHER DISCOUNT: €" + res2 + "\n" + "TOTAL COLLECTED: €" + grandtotal + "\n" + "POINTS COLLECTED: " + points;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            res = pass[3];
                                            if (float.Parse(res) > 0)
                                            {
                                                grandtotal = float.Parse(res) - float.Parse(res1) - float.Parse(res2);
                                                tp = "TOTAL: €" + res + "\n" + "DISCOUNT: €" + res1 + "\n" + "VOUCHER DISCOUNT: €" + res2 + "\n" + "TOTAL COLLECTED: €" + grandtotal + "\n" + "POINTS COLLECTED: " + points;
                                            }
                                            else
                                            {
                                                grandtotal = float.Parse(res) + float.Parse(res1) + float.Parse(res2);

                                                if (grandtotal.ToString().Contains('-'))
                                                {

                                                    res = res.Replace('-', ' ');
                                                    string new_t = grandtotal.ToString().Replace('-', ' ');
                                                  
                                                    tp = "TOTAL: €" + res + "\n" + "DISCOUNT: €" + res1 + "\n" + "VOUCHER DISCOUNT: €" + res2 + "\n" + "TOTAL COLLECTED: €" + new_t + "\n" + "POINTS COLLECTED: " + points;
                                                }
                                                else
                                                {
                                                    tp = "TOTAL: €" + res + "\n" + "DISCOUNT: €" + res1 + "\n" + "VOUCHER DISCOUNT: €" + res2 + "\n" + "TOTAL COLLECTED: €" + grandtotal + "\n" + "POINTS COLLECTED: " + points;
                                                }
                                            }

                                        }
                                        break;
                                }




                            } 
                               
                                    
                        }
                      
                    }
                }

    

                await Application.Current.MainPage.Navigation.PushPopupAsync(new TransPopupPage());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace + "\n " + e.InnerException + "\n" + e.GetBaseException());
                await Application.Current.MainPage.DisplayAlert("AN ERROR HAS OCCURED", "PLEASE CHECK YOUR NETWORK CONNECTION AND TRY AGAIN LATER.", "OK");
                
            }
        }

        public void RefreshCmd()
        {
            Application.Current.MainPage.Navigation.PushAsync(new TransactionPagev2());
        }

        public void ShoworHiddenProducts(Transaction trans)
        {
            if (_oldtrans == trans)
            {
                trans.Isvisible = !trans.Isvisible;
                UpDateProducts(trans);
            }
            else
            {
                if (_oldtrans != null)
                {
                    _oldtrans.Isvisible = false;
                    UpDateProducts(_oldtrans);

                }
                trans.Isvisible = true;
                UpDateProducts(trans);
            }
            _oldtrans = trans;
        }

        private void UpDateProducts(Transaction trans)
        {

            var Index = Transactions.IndexOf(trans);
            Transactions.Remove(trans);
            Transactions.Insert(Index, trans);

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


