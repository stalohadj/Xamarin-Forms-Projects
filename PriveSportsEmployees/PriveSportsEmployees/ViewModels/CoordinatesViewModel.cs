using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
    public class CoordinatesViewModel : ContentPage
    {
        public string lat { get; set; }
        public string lon { get; set; }
        public static string IPLoc = "http://139.138.223.53:8080/$TableGetView?system=system&file=location&__internal=true";
        public static ObservableCollection<Controls.Location> StoreLocations = new ObservableCollection<Controls.Location>();
        public CoordinatesViewModel()
        {


            try
            {
                Task.Run(async () =>
                {

                    string details = DoGetHttp(IPLoc, "", 10000);
                    var ti = JsonConvert.DeserializeObject<List<Loc_pull>>(details);

                    foreach (var obj in ti)
                    {
                        if (String.Equals(obj.longitude, "0") || String.Equals(obj.latitude, "0") || String.Equals(obj.longitude, null) || String.Equals(obj.latitude, null))
                        {
                            StoreLocations.Add(new Controls.Location { address = obj.add1, store_name = obj.name, latitude = Convert.ToDouble(obj.latitude), longitude = Convert.ToDouble(obj.longitude) });
                        }

                        else
                        {
                            lat = obj.latitude.ToString().Replace(",", ".");
                            lon = obj.longitude.ToString().Replace(",", ".");
                            StoreLocations.Add(new Controls.Location { address = obj.add1, store_name = obj.name, latitude = Convert.ToDouble(lat), longitude = Convert.ToDouble(lon) });

                        }
                    }

                   
                });
            }
            catch (Exception e)
            {

            }

        }

        public ICommand bcmd => new Command(() => Back());
        public ICommand ccmd => new Command(() => Device.InvokeOnMainThreadAsync(() => GetCurrentLocation()));
        public ICommand scmd => new Command(() => Device.InvokeOnMainThreadAsync(() => GetStore()));
        public ICommand dcmd => new Command(() => Device.InvokeOnMainThreadAsync(() => DebugLOC()));

        public void Back()
        {
            Application.Current.MainPage = new ManagerAppShell();

        }

        public CancellationTokenSource cts;


        public async Task GetStore()
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
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("STORE", "STORE NAME: " + loc.store_name + " LATITUDE: " + loc.latitude.ToString() + " LONGITUTDE: " + loc.longitude.ToString(), "OK");
                              
                              
                                
                            });

                            break;
                        }
                        else if (dist > 0.8 && dist < 2 || dist == null)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("STORE", "NO STORE DETECTED WITHIN 800m", "OK");



                            });

                        }
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


        public async Task DebugLOC()
        {
            try
            {



                foreach (var loc in StoreLocations)
                {


                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("STORE", "STORE NAME: " + loc.store_name + " LATITUDE: " + loc.latitude.ToString() + " LONGITUTDE: " + loc.longitude.ToString(), "OK");

                    });


                }
            }

            catch (Exception ex)
            {

                // Unable to get location
            }

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

                    var lati = location.Latitude;
                    var longi = location.Longitude;




                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

                    await Application.Current.MainPage.DisplayAlert("COORDINATES", "LATITUDE: " + location.Latitude.ToString() + " LONGITUTDE: " + location.Longitude.ToString(), "OK");

                    BindingContext = this;
                    var placemarks = await Geocoding.GetPlacemarksAsync(lati, longi);
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