using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSports.Models;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;

namespace PriveSports.ViewModels
{
    public class Newsletter_PopupViewModel : PopupPage
    {
        private ImageSource src = null;
        public ICommand TapCommand => new Command<string>(OpenBrowser);
        public System.Drawing.Image icon;
        public string IPnews = "http://139.138.223.53:8080/$TableGetView?system=office&file=newsletter&report=newsletter&__internal=true";
        public string IPimg = "http://139.138.223.53:8080/$TableAttachments?system=office&file=newsletter&__internal=true&search=";


        public async void OpenBrowser(string url)
        {
            try
            {
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Το Link δεν είναι διαθέσιμο ", "OK");

            }
        }


        

        private ListViewModel _links;
        public ObservableCollection<ListViewModel> Links { get; set; }

        public Newsletter_PopupViewModel()
        {
            try
            {
                Links = new ObservableCollection<ListViewModel>();
                AddData();
            }
            catch (Exception e)
            {
                 Application.Current.MainPage.DisplayAlert("Alert", "Το Link δεν είναι διαθέσιμο ", "OK");
            }
            

        }

     
        public void AddData()
        {
            try
            {
                bool isEmpty = !Links.Any();
                if (isEmpty==false)
                {
                    Links.Clear();
                }

                string newslttrs = DoGetHttp(IPnews, "", 10000);//Getting the Details
               // string img = DoGetHttp(IPimg, "", 10000);//Getting the img
                var nl = JsonConvert.DeserializeObject<List<Newsletter>>(newslttrs);
                //var im = JsonConvert.DeserializeObject<List<Newsletter>>(newslttrs);
                int sz = nl.Count();
               
                foreach (var obj in nl)
                {

                    src = "http://139.138.223.53:8080/$TableImage?system=office&__internal=true&file=newsletter&record=" + obj.position;
                    
                        Links.Add(new ListViewModel { Category = obj.date + " " + obj.name, image = src, Isvisible = false, link = obj.link });
                   
                   
                }

                /** if (sz > 2)
                 {
                     
                 }
                 else
                 {
                     Links.Add(new ListViewModel { Category = nl[sz - 1].name + " " + nl[sz - 1].date, Isvisible = false, link = nl[sz - 1].link });
                 }*/

                // 
                // Link = nl[sz - 1].link;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException + " " + e.StackTrace + " " + e.GetBaseException());
            }
        }

    
        public void ShoworHiddenProducts(ListViewModel links)
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

        private void UpDateProducts(ListViewModel links)
        {

            var Index = Links.IndexOf(links);
            Links.Remove(links);
            Links.Insert(Index, links);

        }


        public async void RefreshCmd()
        {
           
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

