using System;
using PriveSports.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Xamarin.Essentials;
using System.Diagnostics;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using PriveSports.Models;
using PriveSports.ViewModels;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PriveSports
{
    public partial class App : Application
    {
        public static bool notification_set = false;
        INotificationManager notificationManager;
        public static bool on_resume = false;
        int notificationNumber = 0;
        public static bool bg = false;
        public static string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public static string IPtableview = "http://139.138.223.53:8080/$TableGetView?";
        public string IP = "11.0.0.99";
        public string IP2 = "11.0.0.138";

        public ICommand PermissionCommand { get; set; }
        //private FeedbackPopupPage feedbackpopup;

        public App()
        {
           
            DependencyService.Get<INotificationManager>().Initialize();
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDE1NDE1QDMxMzgyZTM0MmUzMEJnNWRYaTNudkRWSTJlbzdnc1lrSHRZc0lxV0JUWWJoY2F2YU1SZVZ3Yk09");
            InitializeComponent();

            Device.SetFlags(new[] { "Shapes_Experimental", "MediaElement_Experimental", "Brush_Experimental", "Expander_Experimental" });

           

            MainPage = new LoadingPage();
             


            //Remove this method to stop OneSignal Debugging  
            //OneSignal.Current.SetLogLevel(LOG_LEVEL.VERBOSE, LOG_LEVEL.NONE);

        

            // The promptForPushNotificationsWithUserResponse function will show the iOS push notification prompt. We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 7)
            OneSignal.Current.RegisterForPushNotifications();

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };

  

        }

        protected override async void OnStart()
        {
            //await Current.MainPage.DisplayAlert("Location Use", "This app uses your location to track if at any time you are close to any of our stores & notify you about it!", "OK");
            // await BirthdayMessageAsync("here");
            bg = false;
            try
            {
                
                if (!Current.Properties.ContainsKey("popup"))
                    {
                   
                        await Current.MainPage.Navigation.PushPopupAsync(new DisclaimerPopupPage());
                        Current.Properties["popup"] = false;

                    }

                await Task.Run(async () =>
                {
                    await check_points();

                });
                var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (status == PermissionStatus.Granted)
                {
                    while (bg == false)
                    {
                        if (notification_set != true)
                        {

                            await Task.Run(async () => { await GetCurrentLocation(); });

                        }
                        else
                        {
                            notification_set = true;
                            break;
                        }
                    }
                }
               
               
              
              

                



            }

            catch (Exception)
            {
                await Current.MainPage.DisplayAlert("!!!", "THE CONNECTION TO THE SERVER WAS TERMINATED PLEASE TRY AGAIN LATER.", "ΟΚ");
            }
            
        }

        protected override async void OnSleep()
        {
            bg = true;
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (status == PermissionStatus.Granted)
                {
                    while (bg == true)
                    {
                        if (notification_set != true)
                        {

                            await Task.Run(async () => { await GetCurrentLocation(); });
                        }
                        else
                        {
                            notification_set = true;
                            break;
                        }
                    }
                }
            }

            catch (Exception)
            {
                await Current.MainPage.DisplayAlert("!!!", "THE CONNECTION TO THE SERVER WAS TERMINATED PLEASE TRY AGAIN LATER.", "ΟΚ");
            }
        }

        protected override async void OnResume()
        {
            bg = false;
            on_resume = true;
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status == PermissionStatus.Granted)
            {
                while (bg == false)
                {
                    if (notification_set != true)
                    {
                        await Task.Run(async () => { await GetCurrentLocation(); });
                    }
                    else
                    {
                        notification_set = true;
                        break;
                    }
                }
            }

        }

        public ObservableCollection<Models.Location> StoreLocations { get => GetLoc(); }

        private ObservableCollection<Models.Location> GetLoc()
        {
            return new ObservableCollection<Models.Location>
            {
                new Models.Location { store_name = "PriveSports Lakatamia", address = "Archiepiskopou Makariou III 190, 2311 Lakatamia, Nicosia", latitude = 35.112813, longitude = 33.306812},//FIX
                new Models.Location { store_name = "PriveSports Stavrou", address = "Stavrou 82, 2035 Strovolos, Nicosia", latitude = 35.134910, longitude = 33.358410},
                new Models.Location { store_name = "PriveSports Makedonitissa", address = "28th October Avenue 21, 2414 Egkomi, Nicosia", latitude = 35.16110127412308, longitude = 33.317101252338716},
                new Models.Location { store_name = "PriveSports Latsia", address = "Leoforos Archiepiskopou Makariou III 71, 2233 Latsia, Nicosia", latitude = 35.1112832120995, longitude = 33.38316733326175},
                new Models.Location { store_name = "PriveSports Aglantzia", address = "Leoforos Larnakos 2101 Aglantzia, Nicosia", latitude = 35.15916878027148, longitude = 33.390178390934025},
                new Models.Location { store_name = "PriveSports Outlet Athalassas", address = "Leoforos Athalassas 2024 Athalassas, Nicosia", latitude = 35.14329404311168, longitude = 33.36624677744001},
                new Models.Location { store_name = "PriveSports Kolonakiou", address = "Kolonakiou 56 Despoina Court 4103 Agios Athanasios, Limassol", latitude = 34.69902769953155, longitude = 33.07388591975708},
                new Models.Location { store_name = "PriveSports Anexartisias", address = "Anexartisias 130, &  Stasinou Corner, 3040 Limassol", latitude = 34.679114738112865, longitude = 33.04436440441477 },
                new Models.Location { store_name = "PriveSports Holykics", address = "My Mall Limassol, Franglinou Rousvelt 285, Limassol", latitude = 34.652948516822256, longitude = 32.99718826208728},
                new Models.Location { store_name = "PriveSports Nicosia", address = "staff st", latitude = 51.4496866, longitude = -0.9352241      },

            };
        }

        CancellationTokenSource cts;

       
    
    private async Task GetCurrentLocation()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (status == PermissionStatus.Granted)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    cts = new CancellationTokenSource();
                    var location = await Geolocation.GetLocationAsync(request, cts.Token);
                    int d;
                    string st_name;

                    if (location != null)
                    {
                        var lat = location.Latitude;
                        var lon = location.Longitude;

                        foreach (var loc in StoreLocations)
                        {
                            double dist = Xamarin.Essentials.Location.CalculateDistance(lat, lon, loc.latitude, loc.longitude, DistanceUnits.Kilometers);
                            //  Console.WriteLine("distance: " + dist);

                            if (dist <= 2.00)
                            {
                                notification_set = true;
                                d = ((int)Math.Round(dist, 0, MidpointRounding.AwayFromZero));
                                st_name = loc.store_name;
                                await Task.Run(async () =>
                                {
                                    await OnSendnotification(d, st_name);
                                });
                                break;
                            }
                        }

                        //  Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                        /*
                        Geocoder geoCoder = new Geocoder();

                        Position position = new Position(location.Latitude,location.Longitude);
                        IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
                        string address = possibleAddresses.FirstOrDefault();*/
                        var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);
                        var placemark = placemarks?.FirstOrDefault();

                        if (placemark != null)
                        {
                            var geocodeAddress =
                                $"AdminArea:       {placemark.AdminArea}\n" +
                                $"CountryCode:     {placemark.CountryCode}\n" +
                                $"CountryName:     {placemark.CountryName}\n" +
                                $"FeatureName:     {placemark.FeatureName}\n" +
                                $"Locality:        {placemark.Locality}\n" +
                                $"PostalCode:      {placemark.PostalCode}\n" +
                                $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                                $"SubLocality:     {placemark.SubLocality}\n" +
                                $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                                $"Thoroughfare:    {placemark.Thoroughfare}\n";

                            //Console.WriteLine(geocodeAddress);
                        }
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        public async Task check_points()
        {
           
            try
            {
                if (Application.Current.Properties.ContainsKey("phone"))
                {
                    string response2 = DoGetHttp(IPfind + Application.Current.Properties["phone"].ToString(), "", 10000);
                    if (response2 != "[]")
                    {
                        var des = JsonConvert.DeserializeObject<List<User>>(response2);
                        string transactions = DoGetHttp(IPtableview + "report_fields=_total&report=web_points&system=pos&file=trans&driving=people&fields=refer time date location _total points&record=" + des[0].value, "", 10000);//Getting the Details
                        var ti = JsonConvert.DeserializeObject<List<Transactions>>(transactions);
                        int p = 0;

                        foreach (var x in ti)
                        {
                            p = p + x.points;
                            Console.WriteLine(x.points);

                        }

                        if (p > 750 && p < 999)
                        {
                            notificationManager.SendNotification("ΣΥΓΧΑΡΗΤΗΡΙΑ!", "ΕΧΕΤΕ ΣΥΛΛΕΞΕΙ ΠΕΡΙΣΣΟΤΕΡΟΥΣ ΑΠΟ 750 ΒΑΘΜΟΥΣ. ΣΤΟΥΣ 1000 ΒΑΘΜΟΥΣ ΕΞΑΡΓΥΡΩΣΤΕ 20 ΕΥΡΩ REWARD VOUCHER. ΕΥΧΑΡΙΣΤΟΥΜΕ. ΙΣΧΥΟΥΝ ΟΡΟΙ ΚΑΙ ΠΡΟΥΠΟΘΕΣΕΙΣ.");
                           
                        }
                        else if (p >= 1000)
                        {
                            notificationManager.SendNotification("ΣΥΓΧΑΡΗΤΗΡΙΑ!", "ΕΧΕΤΕ ΣΥΛΛΕΞΕΙ ΠΕΡΙΣΣΟΤΕΡΟΥΣ ΑΠΟ 1000 ΒΑΘΜΟΥΣ. ΕΞΑΡΓΥΡΩΣΤΕ 20 ΕΥΡΩ REWARD VOUCHER. ΕΥΧΑΡΙΣΤΟΥΜΕ. ΙΣΧΥΟΥΝ ΟΡΟΙ ΚΑΙ ΠΡΟΥΠΟΘΕΣΕΙΣ.");
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public async void FeedbackClick(object sender, EventArgs e)
        {
            await Current.MainPage.Navigation.PushPopupAsync(new FeedbackForm());
            //string result = await Current.MainPage.DisplayPromptAsync("Feedback", "Let us know of any feeback you have on the app or website!");
        }

        public async void NewsLetterClick(object sender, EventArgs e)
        {
            await Current.MainPage.Navigation.PushPopupAsync(new Newsletter_popup());
            
        }

       public async Task OnSendnotification(int dist, string store)
        {
            notificationNumber++;
            string title;
            string message;
            if (dist == 0)
            {
                title = "Welcome!";
                message = "You're in " + store + "!";
            }
            else
            { 
                title = "You're near one of our stores!";
                message = "You're only " + dist + " km from " + store + "!";
            }
            notificationManager.SendNotification(title, message);
        }

       /* async Task BirthdayMessageAsync(string name)
        {
           
            string title;
            string message;

            using (var client = new HttpClient())
            {
                var url = new Uri("https://onesignal.com/api/v1/notifications");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "Rest ID");
                var obj = new
                {
                    app_id = "82809559-a634-4969-9549-b57684145e2c",
                    contents = new { en = "English Message" },
                    included_segments = new string[] { "All" }

                };
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
               // var response = await client.PostAsync(url, content);

            }

           // notificationManager.SendNotification (title, message);
        }*/

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
    


        /*public override bool OnBackPressed()
        {
           
            return true;
        }*/
    }

       



}


