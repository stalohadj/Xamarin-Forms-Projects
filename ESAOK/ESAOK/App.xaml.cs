using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using ESAOK.ViewModels;
using ESAOK.Views;
using Newtonsoft.Json;
using Plugin.Permissions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ESAOK
{
    public partial class App : Application
    {
        public string IPtableview = "http://213.7.221.3:9080/$TableGetView?";
        public static bool notification_set = false;
        INotificationManager notificationManager;
        public static bool on_resume = false;
        int notificationNumber = 0;
        public static bool bg = false;
        private static int x;

        public class events
        {
            [DataMember]
            public string date { get; set; }

            [DataMember]
            public string time { get; set; }

            [DataMember]
            public string place { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string duration { get; set; }

            [JsonProperty(PropertyName = "$$hint")]
            public string hint { get; set; }
        }

        public App()
        {
           
            DependencyService.Get<INotificationManager>().Initialize();
            InitializeComponent();
            MainPage = new LoadingPage();

            OneSignal.Current.StartInit("00b7da98-df1f-484e-81d8-2753af8381bb")
            .Settings(new Dictionary<string, bool>() {
            { IOSSettings.kOSSettingsKeyAutoPrompt, false },
            { IOSSettings.kOSSettingsKeyInAppLaunchURL, false } })
            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
            .EndInit();

            // The promptForPushNotificationsWithUserResponse function will show the iOS push notification prompt. We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 7)
            OneSignal.Current.RegisterForPushNotifications();

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };

           
        }

        protected override void OnStart()
        {
            bg = false;
            /* try
             {

                 Task.Run(async () => { await notification_task(); }).Wait();
             }
             catch (Exception e)
             {

                     await Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                 Console.WriteLine(e.StackTrace + " " + e.GetBaseException());

             }*/
        }

        protected override void OnSleep()
        {
            bg = true;
           // Task.Run(async () => { await notification_task(); }).Wait();

        }

        protected override void OnResume()
        {

            bg = false;
            //Task.Run(async () => { await notification_task(); }).Wait();

        }

        private async Task notification_task()
        {
            try
            {
                Thread.Sleep(1000);
                string jsondata = DoGetHttp(IPtableview + "system=office&file=event&report=events&compact=true", "", 10000);
                var Evnts = JsonConvert.DeserializeObject<List<events>>(jsondata);
                if (Evnts != null)
                {
                    x = Evnts.Count();

                    if (SimplePageViewModel.events_token < x)
                    {
                        notification_set = true;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            notificationManager.SendNotification("Καινούρια εκδήλωση έχει καταχωριθεί!!", "Ανανεώστε το ημερολόγιό σας!");
                        });



                    }
                }
            }
            catch (Exception e)
            {

                    
                await Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                Console.WriteLine(e.StackTrace + " " + e.GetBaseException());
               



            }

        }
        public async Task OnSendnotification()
        {
            notificationNumber++;
            string title;
            string message;

            title = "Καινούρια εκδήλωση έχει καταχωριθεί!!";
            message = "Ανανεώστε το ημερολόγιό σας!";

            notificationManager.SendNotification(title, message);
        }

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
            });
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

