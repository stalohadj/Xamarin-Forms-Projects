using Xamarin.Plugin.Calendar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using ESAOK.Models;
using System.Globalization;
using ESAOK.Views;
using System.Collections;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Plugin.Calendar.Interfaces;
using Xamarin.Plugin.Calendar.Controls;
using System.Diagnostics;

namespace ESAOK.ViewModels
{
    public class SimplePageViewModel : BasePageViewModel, INotifyPropertyChanged
    {

        public static int events_token;//get ev_number fromadding in events and use it as token to create notifications
        public Color events_color = Color.Green;

        public bool refreshed = false;
        public string IPtableview = "http://213.7.221.3:9080/$TableGetView?";

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

            [JsonProperty(PropertyName = "*notes")]
            public string notes { get; set; }

            [DataMember]
            public string duration { get; set; }

            [JsonProperty(PropertyName = "$$hint")]
            public string hint { get; set; }
        }


        public ICommand RefreshCommand => new Command(async () => { await Refresh(); });
        public ICommand TodayCommand => new Command(() => { Year = DateTime.Today.Year; Month = DateTime.Today.Month; SelectedDate = DateTime.Today; });

        public ICommand LinksCommand => new Command(() => {
            if (App.Current.MainPage is NavigationPage)
                (App.Current.MainPage as NavigationPage).PushAsync(new LinksListView());
        });
        public ICommand WebsiteCommand => new Command(async () => { await OpenBrowser(); });
        public ICommand EventSelectedCommand => new Command(async (item) => await ExecuteEventSelectedCommand(item));

        public async Task OpenBrowser()
        {
            string url = "http://www.mountaincommissioner.gov.cy/Presidency/CDMC/CDMC/cdmc.nsf/home/home?opendocument";
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }); 
        }



        public ImageSource PlatformSpecificImage { get; set; }
        public static IEnumerable<events> list;
        static IEnumerable<IGrouping<string, events>> query;
        public bool selected = false;

        public SimplePageViewModel() : base()
        {


            if (Device.RuntimePlatform == Device.Android)
            {
                PlatformSpecificImage = "@drawable/icon.png"
;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                PlatformSpecificImage = "logo.png";
            }

            try
            {
                Events = new EventCollection();
                string jsondata = DoGetHttp(IPtableview + "system=office&file=event&report=events&compact=true", "", 10000);
                var Evnts = JsonConvert.DeserializeObject<List<events>>(jsondata);
                if (Evnts != null)
                {
                    events_token = Evnts.Count();

                    var query = Evnts.GroupBy(x => x.date)
                               .Select(g => new
                               {
                                   Date = g.Key,
                                   Details = g.Select(x => new
                                   {
                                       Name = x.name,
                                       Place = x.place,
                                       Time = x.time,
                                       Duration = x.duration,
                                       Notes = x.notes

                                   })
                                 .ToList()
                               })
                       .ToList();


                    foreach (var x in query)
                    {

                        var todayEvents = new DayEventCollection<EventModel>() { EventIndicatorColor = events_color };//, EventIndicatorSelectedColor = Color.Red};


                        Events[DateTime.ParseExact(x.Date, "d/M/yyyy", CultureInfo.CurrentCulture)] = todayEvents;

                        foreach (var x1 in x.Details)
                        {
                            todayEvents.Add(new EventModel { Name = x1.Name, Details = x1.Notes + "\n" + "\n" + x1.Time + "\n" + x1.Place + "\n" + "Διάρκεια: " + x1.Duration + "\n" });
                        }

                    }





                }
                else
                {
                    
                }
            }


            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                Console.WriteLine("ERRORS: " + e.StackTrace);
            }



        }

       

        public async Task Refresh()
        {
            refreshed = true;

            try
            {
                //Events = new EventCollection();
                string jsondata = DoGetHttp(IPtableview + "system=office&file=event&report=events&compact=true", "", 10000);
                var Evnts = JsonConvert.DeserializeObject<List<events>>(jsondata);
                int new_token;
                if (Evnts != null)
                {
                    new_token = Evnts.Count();

                    if (new_token > events_token)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.Current.MainPage.DisplayAlert("Καινούρια καταχώριση", "Καινούρια εκδήλωση για τον μήνα" + " " + Month + "/" + Year + "\n" + "Αριθμός καινούριων εδκηλώσεων: " + (new_token - events_token).ToString(), "Ok");
                        });

                        events_token = new_token;
                    }

                    var query = Evnts.GroupBy(x => x.date)
                               .Select(g => new
                               {
                                   Date = g.Key,
                                   Details = g.Select(x => new
                                   {
                                       Name = x.name,
                                       Place = x.place,
                                       Time = x.time,
                                       Duration = x.duration,
                                       Notes = x.notes
                                   })
                                 .ToList()
                               })
                       .ToList();


                    foreach (var x in query)
                    {

                        var todayEvents = new DayEventCollection<EventModel>() { EventIndicatorColor = Color.Red };//, EventIndicatorSelectedColor= Color.Red};

                        if (!Events.ContainsKey(DateTime.ParseExact(x.Date, "d/M/yyyy", CultureInfo.CurrentCulture)))
                        {
                            todayEvents = new DayEventCollection<EventModel>() { EventIndicatorColor = Color.Red };
                            Events[DateTime.ParseExact(x.Date, "d/M/yyyy", CultureInfo.CurrentCulture)] = todayEvents;
                        }
                        else
                        {
                            todayEvents = new DayEventCollection<EventModel>() { EventIndicatorColor = Color.Green };
                            Events[DateTime.ParseExact(x.Date, "d/M/yyyy", CultureInfo.CurrentCulture)] = todayEvents;

                        }

                        Events[DateTime.ParseExact(x.Date, "d/M/yyyy", CultureInfo.CurrentCulture)] = todayEvents;

                        foreach (var x1 in x.Details)
                        {
                            todayEvents.Add(new EventModel { Name = x1.Name, Details = x1.Notes + "\n" + "\n"  + x1.Time + "\n" + x1.Place + "\n" + "Διάρκεια: " + x1.Duration });
                        }




                    }
                    

                }
                else
                {
                    Events.Clear();
                }
            }

            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                Console.WriteLine("ERRORS: " + e.StackTrace);
            }

        }


        private IEnumerable<EventModel> GenerateEvents(string name, string det)
        {

            return Enumerable.Range(0, 1).Select(x => new EventModel
            {
                Name = name,
                Details = det
            });
        }


        public EventCollection Events { get; }

        private int _month = DateTime.Today.Month;
        public int Month
        {
            get => _month;
            set => SetProperty(ref _month, value);
        }

        public int _year = DateTime.Today.Year;
        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        private CultureInfo _culture = CultureInfo.CreateSpecificCulture("el-GR");
        public CultureInfo Culture
        {
            get => _culture;
            set => SetProperty(ref _culture, value);
        }
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private DateTime _minimumDate = new DateTime(2019, 4, 29);
        public DateTime MinimumDate
        {
            get => _minimumDate;
            set => SetProperty(ref _minimumDate, value);
        }

        private DateTime _maximumDate = DateTime.Today.AddMonths(48);
        public DateTime MaximumDate
        {
            get => _maximumDate;
            set => SetProperty(ref _maximumDate, value);
        }
        public object Message { get; internal set; }


        private async Task ExecuteEventSelectedCommand(object item)
        {
            if (item is EventModel eventModel)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert(eventModel.Name, eventModel.Details, "Ok");
                });
              
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
