using System;
using System.Collections;
using System.Collections.Generic;
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
using PriveSports.Models;
using PriveSports.Views;
using Xamarin.Essentials;

namespace PriveSports.ViewModels
{

    public class PointsPageViewModel : ContentPage
    {
        public string Points { get; set; }
        public string button_text { get; set; }
        public ICommand RefreshCommand => new Command(() => RefreshCmd());
        public ICommand TapCommand => new Command<string>(OpenBrowser);

        public static int pt;
        
        public string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public string IPtableview = "http://139.138.223.53:8080/$TableGetView?";
        public string IP = "11.0.0.99";
        public string IP2 = "11.0.0.138";

        public ObservableCollection<Transactions> Transactions { get; set; }

      


        public PointsPageViewModel()
        {


            if (Application.Current.Properties["phone"] != null && (string)Application.Current.Properties["phone"] != "")
            {
                CountPoints();
                button_text = "Refresh";


            }
            else
            {
                Points = "Register to view your Points!";
                button_text = "Register Now";
            }
            BindingContext = this;
        }

        public void CountPoints()
        {
            Hashtable ht = new Hashtable();
            ht.Add("user", "root");
            ht.Add("session-token", "tesT");

            int p = 0;
            try
            {
                string response2 = DoGetHttp(IPfind + Application.Current.Properties["phone"].ToString(), "", 10000);
                if (response2 != "[]")
                {
                    var des = JsonConvert.DeserializeObject<List<User>>(response2);
                    string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields=refer time date location _total points&record=" + des[0].value, "", 10000);//Getting the Details
                    if (transactions == "" | transactions == null)
                    {
                        Points = "Make your fist purchase to start collecting points!";

                    }
                    else
                    {
                        var ti = JsonConvert.DeserializeObject<List<Transactions>>(transactions);

                        foreach (var x in ti)
                        {
                            p = p + x.points;
                            Console.WriteLine(x.points);

                        }


                        if (p == 0 || p == null)
                        {
                            Points = "Make your fist purchase to start collecting points!";
                        }

                        else if (p > 750)
                        {
                            pt = p;
                        }
                        Points = p.ToString();
                    }
                       
                }
                else
                {
                    Points = "Register to view your Points!";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERRORS: " + e.StackTrace);
            }
        }

        public async void OpenBrowser(string url)
        {
            try
            {
                if (Points == "Make your fist purchase to start collecting points!")
                {
                    await Browser.OpenAsync("https://www.privesports.com.cy", BrowserLaunchMode.SystemPreferred);
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Το Link δεν είναι διαθέσιμο ", "OK");

            }
        }

        public void RefreshCmd()
        {
            if (Application.Current.Properties["phone"] != null && (string)Application.Current.Properties["phone"] != "")
            {
                Application.Current.MainPage.Navigation.PushAsync(new PointsPage());
            }
            else
            {
                Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
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




