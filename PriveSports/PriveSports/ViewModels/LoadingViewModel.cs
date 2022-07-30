using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using Com.OneSignal;
using Newtonsoft.Json;
using PriveSports.Models;
using PriveSports.Views;
using Xamarin.Forms;

namespace PriveSports.ViewModels
{
    public class LoadingViewModel : ContentPage
    {
        public LoadingViewModel()
        {

            Device.InvokeOnMainThreadAsync(() => DelayedShow());

        }
/*
        //Data you need to set:
        let birthdayMonth = 0;//Format is MM 0 indexed, January = 0, December = 11
        let birthdayDay = 29;//Format is DD

        let currentDate = new Date();
        let currentDateUnixTimestamp = Math.round(currentDate.getTime() / 1000);
        let currentYear = currentDate.getFullYear();
        let birthdayMonthDay = new Date(currentYear, birthdayMonth, birthdayDay); // Format YYYY, MM, DD 
        let birthdayUnixTimestamp = Math.round(birthdayMonthDay.getTime() / 1000);
        let currentBirthdayPassed = Math.sign(birthdayUnixTimestamp - currentDateUnixTimestamp);

        let birthdayTimestamp = 0;

if (currentBirthdayPassed === 1) {
  console.log("birthday has not occurred yet!")
  birthdayTimestamp = birthdayUnixTimestamp

    } else if (currentBirthdayPassed === -1) {
  console.log("we will celebrate next year")
  birthdayMonthDay = new Date(currentYear + 1, birthdayMonth, birthdayDay);
    birthdayUnixTimestamp = Math.round(birthdayMonthDay.getTime() /1000);
  birthdayTimestamp = birthdayUnixTimestamp;

} else
{
    console.log("birthdate time not set properly")
}
OneSignal.push(function() {
    OneSignal.sendTag("birthday", birthdayTimestamp).then(function(tagsSent) {
        // Callback called when tags have finished sending
        console.log(tagsSent);
    });
});*/
        public string BIP = "http://139.138.223.53:8080/$PosFindPeople?find=";
        private async Task DelayedShow()
        {
            await Task.Delay(1800);

            if (Application.Current.Properties.ContainsKey("phone"))
            {
                if (Application.Current.Properties["phone"] != null && (string)Application.Current.Properties["phone"] != "")
                {
                    Application.Current.MainPage = new AppShell();
                    //Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    string bdays = DoGetHttp(BIP + Application.Current.Properties["phone"], "", 10000);
                  
                    var b = JsonConvert.DeserializeObject<List<Birthdays>>(bdays);
                    DateTime bday = Convert.ToDateTime(b[0].birth);
                    int birthdayMonth = bday.Month;
                    int birthdayDay = bday.Day;
                    DateTime currentDate = DateTime.Now;
                    double currentDateUnixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                    int currentYear = currentDate.Year;
                    DateTime birthdayMonthDay = new DateTime(currentYear, birthdayMonth, birthdayDay);
                    Int32 birthdayUnixTimestamp = (Int32)(DateTime.UtcNow.Subtract(birthdayMonthDay)).TotalSeconds;
                    double currentBirthdayPassed = Math.Sign(birthdayUnixTimestamp - currentDateUnixTimestamp);
                    int birthdayTimestamp = 0;

                    string[] date = b[0].nameday.Split(' ');

                    DateTime nday = Convert.ToDateTime(date[3]);
                    Debugger.Break();
                    int namedayMonth = bday.Month;
                    int namedayDay = bday.Day;
                    DateTime namedayMonthDay = new DateTime(currentYear, namedayMonth, namedayDay);
                    Int32 namedayUnixTimestamp = (Int32)(DateTime.UtcNow.Subtract(namedayMonthDay)).TotalSeconds;
                    double currentNamedayPassed = Math.Sign(namedayUnixTimestamp - currentDateUnixTimestamp);
                    int namedayTimeStamp = 0;
                    Debugger.Break();
                    if (currentBirthdayPassed == 1)
                    {
                        Console.WriteLine("birthday has not occurred yet!");

                        birthdayTimestamp = birthdayUnixTimestamp;
                        namedayTimeStamp = namedayUnixTimestamp;
                        Debugger.Break();

                    }
                    else if (currentBirthdayPassed == -1)
                    {
                        Console.WriteLine("we will celebrate next year");
                        birthdayMonthDay = new DateTime(currentYear + 1, birthdayMonth, birthdayDay);
                        birthdayUnixTimestamp = (Int32)(DateTime.UtcNow.Subtract(birthdayMonthDay)).TotalSeconds;
                        birthdayTimestamp = birthdayUnixTimestamp;

                        namedayMonthDay = new DateTime(currentYear + 1, namedayMonth, namedayDay);
                        namedayUnixTimestamp = (Int32)(DateTime.UtcNow.Subtract(namedayMonthDay)).TotalSeconds;
                        namedayTimeStamp = namedayUnixTimestamp;
                        Debugger.Break();

                    }
                    else
                    {
                        Console.WriteLine("birthdate time not set properly");
                    }
                       
                    
                   // var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                    OneSignal.Current.SendTag("birthday", birthdayTimestamp.ToString());
                    OneSignal.Current.SendTag("nameday", namedayTimeStamp.ToString());
                    Debugger.Break();
                    // Debugger.Break();


                }

                else
                {
                    Application.Current.MainPage = new NavigationPage(new RegisterPage());
                    //Application.Current.MainPage = new  NavigationPage(new LoginPage());
                }

            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new RegisterPage());
                //Application.Current.MainPage = new  NavigationPage(new LoginPage());
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
    


