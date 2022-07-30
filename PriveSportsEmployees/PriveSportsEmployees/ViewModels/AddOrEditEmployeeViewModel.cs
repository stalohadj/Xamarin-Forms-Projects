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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.Views;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{

    public class AddOrEditEmployeeViewModel : BaseViewModel
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        public string e_act { get; set; }
        public string s;


        public class Action
        {
            [DataMember]
            public string type { get; set; }
            [DataMember]
            public string name { get; set; }
            [JsonProperty(PropertyName = "$type")]
            public int numtype { get; set; }
            [DataMember]
            public string points { get; set; }
            [JsonProperty(PropertyName = "$$position")]
            public int posnum { get; set; }
        }

        public class empl_dts
        {
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string surname { get; set; }
            [DataMember]
            public string id { get; set; }
            [DataMember]
            public string location { get; set; }
            [JsonProperty(PropertyName = "position.authority")]
            public string authority { get; set; }
            [JsonProperty(PropertyName = "$$position")]
            public int posnum { get; set; }
        }
        public class Act
        {
            public string a { get; set; }
        }

        public class Empl
        {
            public string e { get; set; }
        }


        public string IPEmplSel = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=action&__internal=true";
        public string IPGetEmpl = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=employee&report=employees_app&__internal=true";
        
        public string _locations { get; set; }
        public string _secondPickerItemsSource { get; set; }
        public ObservableCollection<string> Actions { get; set; }
        public ObservableCollection<string> Empls { get; set; }
        public List<empl_dts> all_employees = new List<empl_dts>();

        private Reward _employee;
        public Reward Employee
        {
            get { return _employee; }
            set { _employee = value; OnPropertyChanged(); }
        }

        public bool _selectedLocationIndex;
        public bool SelectedLocationIndex
        {
            get { return _selectedLocationIndex; }
            set { _selectedLocationIndex = value; OnPropertyChanged(); }
        }

        public string Locations
        {
            get { return _locations; }
            set { _locations = value; OnPropertyChanged(); }
        }
        public string SecondPickerItemsSource
        {
            get { return _secondPickerItemsSource; }
            set { _secondPickerItemsSource = value; OnPropertyChanged(); }
        }
        public AddOrEditEmployeeViewModel()
        {
        
            Employee = new Reward();
            Actions = new ObservableCollection<string>();
            Empls = new ObservableCollection<string>();
          
            AddPicker();



           // this.WhenAny(SelectLoc, o => o.sel_loc);


        }


        public void AddPicker()
        {
            try
            {
                string response = DoGetHttp(IPEmplSel, "", 10000);
                var des = JsonConvert.DeserializeObject<List<Action>>(response);

                string response2 = DoGetHttp(IPGetEmpl, "", 10000);
                var des2 = JsonConvert.DeserializeObject<List<empl_dts>>(response2);


                string value = "";



                foreach (var obj in des)
                {
                    if (obj.type == "Action")
                    {
                        s = obj.posnum + ". " + obj.name + " " + obj.points + " POINTS";
                        Actions.Add(s);
                    }
                }
                Actions = new ObservableCollection<string>(Actions.OrderBy(i => i));


                foreach (var obj in des2)
                {
                    if (obj.authority != "Administrator")
                    {
                        if (Application.Current.Properties.ContainsKey("selected_loc"))
                        {

                            if (obj.location == (string)Application.Current.Properties["selected_loc"])
                            {

                                s = obj.posnum + " " + obj.name + " " + obj.surname;
                                // all_employees.Add(new empl_dts { name = obj.name, surname = obj.surname, id = obj.id, location = obj.location, authority = obj.authority });
                                Empls.Add(s);

                            }
                        }
                    }
                }




            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("AN ERROR HAS OCCURED", "PLEASE CHECK YOUR NETWORK CONNECTION AND TRY AGAIN LATER.", "OK");
            }
        }



       /* public void SelectLoc(string property)
        {

            if (sel_loc == null || property == null)
            {
                Debugger.Break();
            }
            //Filter the collection of states and set the results     
            var localized = all_employees.Where(a => a.location == property).ToList();
            Empls = new ObservableCollection<string>();
            foreach (var obj in localized)
            {
                Empls.Add(s = obj.name + " " + obj.surname + " " + obj.id);
            }

        }*/

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


 