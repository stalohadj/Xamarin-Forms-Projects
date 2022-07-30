using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESAOK.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace ESAOK.ViewModels
{
    public class LinksViewModel : INotifyPropertyChanged
    {

        //public ICommand TapCommand { get; private set; }
        public ICommand TapCommand => new Command<string>(OpenBrowser);
        public ICommand WebsiteCommand => new Command(async () => { await OpenBrowser(); });
        // = new AsyncCommand((o) => SaveCommandHandlerAsync (o));
        //TapCommand = new Command();


        public ImageSource PlatformSpecificImage { get; set; }


        public class LinkStructure
        {
            [DataMember]
            public string date { get; set; }

            [DataMember]
            public string name { get; set; }

            [JsonProperty(PropertyName = "*notes")]
            public string notes { get; set; }

            public string url { get; set; }

        }

        public string IPtableview = "http://213.7.221.3:9080/$TableGetView?";


        public event PropertyChangedEventHandler PropertyChanged;

        private LinksModel _links;
        public ObservableCollection<LinksModel> Links { get; set; }

        public LinksViewModel()
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

            Links = new ObservableCollection<LinksModel>();
            Links.Clear();
            AddData();
        }

        DateTime startDate;
        string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                    "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy"};

        public void AddData()
        {
            try
            {
                Links.Clear();
                string linksdata = DoGetHttp(IPtableview + "system=office&file=linkurl&report=links&compact=true", "", 10000);//Getting Links
              
                if (linksdata != "" && linksdata!=null)
                {
                    var Lnks = JsonConvert.DeserializeObject<List<LinkStructure>>(linksdata);
                    var orderedList = Lnks.OrderByDescending(x => DateTime.ParseExact(x.date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None)).ToList();
                  
                    foreach (var x in orderedList)
                    {
                        if (x.notes != null && x.notes != "")
                        {
                            Links.Add(new LinksModel { Category = x.date.ToString(), Isvisible = false, Title = x.name + "\n", Details = x.notes + "\n", link = "\n" + x.url });

                        }
                        else
                        {
                            Links.Add(new LinksModel { Category = x.date.ToString(), Isvisible = false, Title = x.name + "\n", link = "\n" + x.url });
                        }
                    }
                   
                    
                }
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                Console.WriteLine(e.StackTrace + " " + e.GetBaseException());
              
                
            }
        }

        public void Refresh()
        {
            try
            {
                Links.Clear();

                string linksdata = DoGetHttp(IPtableview + "system=office&file=linkurl&report=links&compact=true", "", 10000);//Getting Links
                if (linksdata != "" && linksdata != null)
                {
                    var Lnks = JsonConvert.DeserializeObject<List<LinkStructure>>(linksdata);
                    var orderedList = Lnks.OrderByDescending(x => DateTime.ParseExact(x.date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None)).ToList();

                    foreach (var x in orderedList)
                    {
                        if (x.notes != null && x.notes != "")
                        {
                            Links.Add(new LinksModel { Category = x.date.ToString(), Isvisible = false, Title = x.name + "\n", Details = x.notes + "\n", link = "\n" + x.url });

                        }
                        else
                        {
                            Links.Add(new LinksModel { Category = x.date.ToString(), Isvisible = false, Title = x.name + "\n", link = "\n" + x.url });
                        }
                    }


                }
                _isRefreshing = false;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsRefreshing)));
            }
            catch (Exception e)
            {
                _isRefreshing = false;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsRefreshing)));
                Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.StackTrace + " " + e.GetBaseException());
                
            }
        }

        public void ShoworHiddenProducts(LinksModel links)
        {
            if (_links == links)
            {
                links.Isvisible = !links.Isvisible;
                UpDateProducts(links);
            }
            else
            {
                if (_links != null)
                {
                    _links.Isvisible = false;
                    UpDateProducts(_links);

                }
                links.Isvisible = true;
                UpDateProducts(links);
            }
            _links = links;
        }

        private void UpDateProducts(LinksModel links)
        {

            var Index = Links.IndexOf(links);
            Links.Remove(links);
            Links.Insert(Index, links);

        }




         public async void OpenBrowser(string url)
         {
             try
             {
                 await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
             }
             catch (Exception e)
             {

                 await App.Current.MainPage.DisplayAlert("Alert", "Το Link δεν είναι διαθέσιμο ", "OK");

             }
         }
        public async Task OpenBrowser()
        {
            string url = "http://www.mountaincommissioner.gov.cy/Presidency/CDMC/CDMC/cdmc.nsf/home/home?opendocument";
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            });
        }

        private bool _isRefreshing = false;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = true;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsRefreshing)));
                Refresh();
              
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
