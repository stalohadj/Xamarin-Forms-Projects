using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.Services;
using PriveSportsEmployees.Views;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
    public class EmployeePointsViewModel : INotifyPropertyChanged
    {
        public bool flag = false; 
       
        public class Emply
        {
            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Position { get; set; }

            [DataMember]
            public string Location { get; set; }

        }

        public class Emply_p
        {
            [JsonProperty(PropertyName = "$employee")]
            public int ID { get; set; }
            [DataMember]
            public string employee { get; set; }
            
        }
        public string IPBalance = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointslog&driving=employee&__internal=true&from=";
        public string IPEmpl = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=employee&report=employees_app&__internal=true";
        public string IPpointem = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointslog&report=employees_with_points&__internal=true";
        public ICommand Histcmd => new Command<int>((x) => ViewHistory(x));
       
       
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _links;
        public ObservableCollection<Emply> Emp { get; set; }
      
        public EmployeePointsViewModel()
        {
          
            Emp = new ObservableCollection<Emply>();
            Emp.Clear();
            AddData();
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    AddData();

                    IsRefreshing = false;
                });
            }
        }


        public void AddData()
        {
            try
            {
               
                Emp.Clear();
                string response2 = DoGetHttp(IPpointem, "", 10000);
                string response = DoGetHttp(IPEmpl, "", 10000);
                if (response2 != null && response2 != "")
                {
                    var des1 = JsonConvert.DeserializeObject<List<Emply_p>>(response2);
                    if (response != null)
                    {
                        var des = JsonConvert.DeserializeObject<List<Employee>>(response);
                        foreach (var p in des)
                        {
                            foreach(var p1 in des1)
                            {
                                if(p1.ID == p.posnum)
                                {
                                    
                                    if (p.position != "ADMINISTRATOR" && !Emp.Any(x => x.ID == p.posnum))
                                    {
                                        Emp.Add(new Emply { ID = p.posnum, Name = p.posnum + " " + p.name + " " + p.surname, Location = p.location, Position = p.position });
                                        
                                    }
                                } 
                            }
                            
                               
                            
                        }
                    }
                }


                Emp = new ObservableCollection<Emply>(Emp.Distinct());


            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE CHECK YOUR WIFI CONNECTION", "OK");
                Console.WriteLine(e.StackTrace + " " + e.GetBaseException());


            }
        }


        public void ViewHistory(int p)
        {
            string response = DoGetHttp(IPBalance + p, "", 10000);

            if(response!=null & response != "")
            {
                PointsPopupViewModel.emp = p;
                Application.Current.MainPage.Navigation.PushPopupAsync(new PointsPopupPage());
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("No History", "This person has not acquired any points yet.", "OK");
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
}   }



