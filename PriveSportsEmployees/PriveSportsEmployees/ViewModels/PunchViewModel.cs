using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using PriveSportsEmployees.Controls;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections;
using PriveSportsEmployees.Views;

namespace PriveSportsEmployees.ViewModels
{
    public class PunchViewModel : ContentPage
    {

        //public DateTime Dt {get; set;}
        public bool x;
       
        public static string IPEmpl = "http://139.138.223.53:8080/$PayrollClock?employee=";

        public int count=0; 
        public bool isvis { get; set; }
        public string name { get; set; }
        public string loc_ { get; set; }
        public string conf { get; set; }
        public string img { get; set; }
        public string enable_btn { get; set; }
        public int ps = 0;

        public ICommand PunchInCmd => new Command(async () => await PunchAsync());
        public ICommand Rewards => new Command(() =>  RewardsCmd());
        public ICommand LogoutCmd => new Command(()=>Logout());
        public ICommand Coordinates => new Command(() => CoordinatesCmd());

        public PunchViewModel()
        {
         
            if (App.inloc == true)
            {
                enable_btn = "1";
                img = "tick.png";
                loc_ = App.stname;
                conf = "Ready to Punch In";
            }
            else
            {
                img = "not.png";
                loc_ = "Not in Location";
                conf = "Punch Disabled";
                enable_btn = "0";
            }

            name = (string)Application.Current.Properties["name"]; //RegisterViewModel.emplname;

           
            isvis = true;

            if (!Application.Current.Properties.ContainsKey("punchedin") || !Application.Current.Properties.ContainsKey("punchedout")) 
            {
                Application.Current.Properties["punchedin"] = false;
                Application.Current.Properties["punchedout"] = false;
            }
            

        }
        public void Logout()
        {
            Application.Current.Properties["id"] = null;
            Application.Current.Properties["phone"] = null;
            Application.Current.Properties["name"] = null;
            Application.Current.Properties["pos"] = null;
            Application.Current.MainPage = new RegisterPage();

        }

        public void CoordinatesCmd()
        {
            Application.Current.MainPage = new CoordinatesPage();
        }

        public void RewardsCmd()
        {
            Application.Current.MainPage = new EmployeeListPage();
        }


        public async Task PunchAsync()
        {
            if (Application.Current.Properties["pos"] != null)
            {
                ps = (int)Application.Current.Properties["pos"];
            }

           // Console.WriteLine("DEBUG HERE: " + (bool)Application.Current.Properties["punchedin"]);

            if (App.inloc)
            {
                //Console.WriteLine("DEBUG HERE: " + (bool)Application.Current.Properties["punchedin"]);
                count++;
                if ((bool)Application.Current.Properties["punchedin"] == false)
                {
                    bool answer = await Application.Current.MainPage.DisplayAlert("PUNCH IN", "You are about to Punch in on " + DateTime.Now.ToString() + ". Do you want to proceed?", "Yes", "No");

                    if (answer == true)
                    {
                        DoGetHttp(IPEmpl + ps, "", 10000);
                        App.onstart = false;
                        Application.Current.Properties["punchedin"] = true;
                        Application.Current.Properties["punchedout"] = false;
                        await Application.Current.MainPage.DisplayAlert("WELCOME", "You have succesfully been punched in!", "OK");
                    }

                }
                else if ((bool)Application.Current.Properties["punchedout"] == false)
                {
                    bool answer = await Application.Current.MainPage.DisplayAlert("PUNCH OUT", "You are about to Punch out on " + DateTime.Now.ToString() + ". Do you want to proceed?", "Yes", "No");
                    if (answer == true)
                    {
                        App.onstart = true;
                        DoGetHttp(IPEmpl + ps, "", 10000);
                        Application.Current.Properties["punchedout"] = true;
                        Application.Current.Properties["punchedin"] = false;
                        await Application.Current.MainPage.DisplayAlert("GOODBYE", "You have succesfully been punched out!", "OK");
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You no longer are in the right location. Unable to Punch.", "OK");
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


