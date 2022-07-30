using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using Rg.Plugins.Popup.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
	public class PointsPopupViewModel : PopupPage
	{
        public static int emp { get; set; }
        public string Balance { get; set; }
        public class Hist
        {
            [DataMember]
            public string date { get; set; }
            [DataMember]
            public string action { get; set; }
            [DataMember]
            public int points { get; set; }

        }
        public string IPBalance = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointslog&driving=employee&__internal=true&from=";

       
        public event PropertyChangedEventHandler PropertyChanged;

        private string _links;
        public ObservableCollection<PointsHistory> History { get; set; }

        public PointsPopupViewModel()
        {


            History = new ObservableCollection<PointsHistory>();
            History.Clear();
            AddData(emp);
        }


        public void AddData(int pos)
        {
            try
            {
                int balance = 0;
                History.Clear();
                string response = DoGetHttp(IPBalance + pos, "", 10000);
                   
                if (response != null && response!="")
                {
                    var des = JsonConvert.DeserializeObject<List<Hist>>(response);
                    foreach (var p in des)
                    {
                        balance += p.points;
                        if (p.points > 0)
                        {
                            History.Add(new PointsHistory { Isvisible = true, Date = p.date, Details = p.action, Points = "POINTS GAINED: " + p.points });
                        }
                        else
                        {
                            History.Add(new PointsHistory { Isvisible = true, Date = p.date, Details = p.action, Points = "POINTS REDEEMED: " + p.points });
                        }

                    }

                    Balance = "Points Balance: " + balance;
                } 
               
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                Console.WriteLine(e.StackTrace + " " + e.GetBaseException());
              

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


