using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.Views;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using OneSignalSDK.Xamarin;
using OneSignalSDK.Xamarin.Core;
using Xamarin.Forms.Xaml;

namespace PriveSportsEmployees
{
 
    public partial class App : Application
    {

       
        public string lat;
        public string lon;
        public static bool inloc;
        public static string stname;
        public static bool onstart;
        public static string IPLoc= "http://139.138.223.53:8080/$TableGetView?system=system&file=location&__internal=true";
        public static ObservableCollection<Controls.Location> StoreLocations = new ObservableCollection<Controls.Location>();
        public App()
        {
            InitializeComponent();

            
     
            
            if (Application.Current.Properties.ContainsKey("id"))
            {
                // Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                if (Application.Current.Properties["id"] != null && (string)Application.Current.Properties["id"] != "")
                {
                   
                    MainPage = new LoadingPage();
                }

                else
                {
                    MainPage = new RegisterPage();
                }
            }
            else
            {

                MainPage = new RegisterPage();
            }


            try
            {
                Device.InvokeOnMainThreadAsync(() => {
                    string details = DoGetHttp(IPLoc, "", 10000);
                    var ti = JsonConvert.DeserializeObject<List<Loc_pull>>(details);

                    foreach (var obj in ti)
                    {
                        if (String.Equals(obj.longitude, "0") || String.Equals(obj.latitude, "0") || String.Equals(obj.longitude, null) || String.Equals(obj.latitude, null))
                        {
                            StoreLocations.Add(
                                new Controls.Location { address = obj.add1, store_name = obj.name, latitude = Convert.ToDouble(obj.latitude), longitude = Convert.ToDouble(obj.longitude) });
                        }

                        else
                        {
                            lat = obj.latitude.ToString().Replace(",", ".");
                            lon = obj.longitude.ToString().Replace(",", ".");
                            StoreLocations.Add(new Controls.Location { address = obj.add1, store_name = obj.name, latitude = Convert.ToDouble(lat), longitude = Convert.ToDouble(lon) });

                        }
                    }
                    Device.InvokeOnMainThreadAsync(() => GetCurrentLocation());
                });
 

                  
         
                  
         
            }
            catch (Exception e)
            {
             
            }

            OneSignal.Default.Initialize("e1ed2dc4-4a8e-4b6b-af78-a88b12d9511a");
            OneSignal.Default.PromptForPushNotificationsWithUserResponse();
     
        }


        public string IPEmplSel = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=action&__internal=true";

        protected override async void OnStart()
        {
         
            try
            {
               
                if (!Current.Properties.ContainsKey("popup"))
                {
                    await Current.MainPage.Navigation.PushPopupAsync(new DisclaimerPopupPage());
                    Current.Properties["popup"] = false;
                }

               var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
               
                
                if (status != PermissionStatus.Granted)
                {
                     await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                   
                     await Device.InvokeOnMainThreadAsync(() => Permissions.RequestAsync<Permissions.LocationWhenInUse>());
                     status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                     if (status == PermissionStatus.Granted)
                     {
                        await Device.InvokeOnMainThreadAsync(() => GetCurrentLocation());
                    }

                }
                else
            {
                    await Device.InvokeOnMainThreadAsync(() => GetCurrentLocation());
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine("EXP: " + e);
               
            }


            onstart = true;

            

        }

        protected override async void OnSleep()
        {
            await Device.InvokeOnMainThreadAsync(() => GetCurrentLocation());
        }

        protected override async void OnResume()
        {
            onstart = true;

            await Device.InvokeOnMainThreadAsync(() => GetCurrentLocation());
               
        }

       
        public CancellationTokenSource cts;

        public void MakeLongWebServiceCall()                        
        {
             Task.Delay(TimeSpan.FromSeconds(5)).Wait();       
        }

        public async Task GetCurrentLocation()
        {

            try
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

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Console.WriteLine("WEB " + loc.longitude + " " + loc.latitude);
                            Console.WriteLine("hard: " + lat + " " + lon);
                           
                        });
                        double dist = Xamarin.Essentials.Location.CalculateDistance(lat, lon, loc.latitude, loc.longitude, DistanceUnits.Kilometers);
                        Console.WriteLine("distance: " + dist);
                        if (dist <= 0.8)
                        {
                            
                            d = ((int)Math.Round(dist, 0, MidpointRounding.AwayFromZero));
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                st_name = loc.store_name;
                                inloc = true;
                                stname = st_name;
                                if (Application.Current.Properties.ContainsKey("id"))
                                {
                                    // Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                                    if (Application.Current.Properties["id"] != null && (string)Application.Current.Properties["id"] != "")
                                    {
                                        MainPage = new UserAppShell();
                                    }
                                }
                            });
                           
                            break;
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                inloc = false;
                            });
                        }

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Console.WriteLine("is it in loc: " + inloc);
                        });
                       
                    }

                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                
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

                        Console.WriteLine(geocodeAddress);
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
                await Permissions.RequestAsync<Permissions.LocationAlways>(); // Handle permission exception
            }
            catch (Exception ex)
            {
                
                // Unable to get location
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
