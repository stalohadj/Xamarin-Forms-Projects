using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
    public class UserViewModel : ContentPage


    {
        public string IPBalance = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointslog&driving=employee&__internal=true&from=";
        public string IPpoints = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointslog&driving=";
        public string IPrbalance = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointspro&report=real_balance";
        public static int Balance { get; set; }
        public int balance_ { get; set; }
        public bool isVis { get; set; }
        public int realbalance_ { get; set; }
        public static int r_balance { get; set; }
        public ICommand RedeemPointsCmd => new Command(() => RedeemPoints());
       
        public class Blnc
        {
            public int points { get; set; }
        }

        public class RBlnc
        {
            [JsonProperty(PropertyName = "action.type")]
            public string type { get; set; }
            [JsonProperty(PropertyName = "action.points")]
            public int points { get; set; }
            [JsonProperty(PropertyName = "$employee")]
            public int employee { get; set; }
            public string pointslog { get; set; }
            public string reject { get; set; }

        }
        public UserViewModel()
        {
           // balance_ = 0;
            Balance = 0;
            try
            {
                string response = DoGetHttp(IPBalance + Application.Current.Properties["pos"], "", 10000);
                if (response != null && response!="")
                {
                    var des = JsonConvert.DeserializeObject<List<Blnc>>(response);
                    foreach (var p in des)
                    {
                        Balance += p.points;
                        // balance_ += p.points;
                    }
                }
                balance_ = Balance;

                int s = 0;
                string response2 = DoGetHttp(IPrbalance, "", 10000);
                if (response2 != null && response2 != "")
                {
                    var des_ = JsonConvert.DeserializeObject<List<RBlnc>>(response2);
                    Console.WriteLine((int)Application.Current.Properties["pos"]);
             
                    foreach (var o in des_)
                    {
                        Console.WriteLine(o.employee + " " +o.type);
                        if(o.employee == (int)Application.Current.Properties["pos"] && o.pointslog==null && o.reject == null && o.type == "Reward")
                        {
                            
                            isVis = true;
                            s += o.points;
                           
                        }
                        else
                        {
                            isVis = false;
                            realbalance_ = balance_;
                            r_balance = realbalance_;
                        }
                        
                    }

                    realbalance_ = balance_ - s;
                    r_balance = realbalance_;
                    
                }
            }
            catch (Exception e)
            {

            }
        }


        public void RedeemPoints()
        {
            Application.Current.MainPage.Navigation.PushAsync(new RedeemPointsPage());
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


