using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.Views;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace PriveSportsEmployees.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public static string emplname;
        public static int pos;
        public static string id;
        public static string occ;
        public string IPEmpl = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=employee&__internal=true";
        public static string IPGUID = "http://139.138.223.53:8080/$PayrollEmployeeGuid?employee=";
        public string Phone { get; set; }
        public string ID { get; set; }

        public ICommand RegisterCommand => new Command(() => Register());
        //public ObservableCollection<Employee> Employees { get; set; }

        private async void Register()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ID))
                {
                    await Application.Current.MainPage.DisplayAlert("ID", "Provide your Employee ID to continue.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Phone))
                {
                    await Application.Current.MainPage.DisplayAlert("Phone", "Provide your phone number to continue.", "OK");
                    return;
                }

                if (Phone.Length != 8)
                {
                    await Application.Current.MainPage.DisplayAlert("Phone", "Please provide your phone number in the form of {99887766} to continue.", "OK");
                    return;
                }

                string details = DoGetHttp(IPEmpl, "", 10000);
                var ti = JsonConvert.DeserializeObject<List<Employee>>(details);
                
                foreach (var obj in ti)
                {
                  
                    if (obj.id == ID && obj.mobile == Phone)
                    {
                        emplname = obj.name + " " + obj.surname;
                        pos = obj.posnum;
                        occ = obj.position;
                        
                        // deviceIdentifier = DependencyService.Get<IDevice>().GetIdentifier();
                       
                        Application.Current.Properties["id"] = ID;
                        Application.Current.Properties["phone"] = Phone;
                        Application.Current.Properties["name"] = emplname;
                        Application.Current.Properties["pos"] = pos;
                        Application.Current.Properties["level"] = occ;


                        if (obj.GUID == "" || obj.GUID == null)
                        {
                            var id = Preferences.Get("my_id", string.Empty);
                            if (string.IsNullOrWhiteSpace(id))
                            {
                                id = System.Guid.NewGuid().ToString();
                                Preferences.Set("my_id", id);
                            }

                            Console.WriteLine(Preferences.Get("my_id", id));
                            DoGetHttp(IPGUID + pos + "&guid=" + Preferences.Get("my_id", id), "", 10000);
                          //  DoGetHttp("http://11.0.0.122:8080/$PayrollEmployeeGuid?employee=" + Preferences.Get("my_id", id).ToString(), "", 10000);

                            //send IMEI
                        }
                        else
                        {

                            if (obj.GUID != Preferences.Get("my_id", id))
                            {
                                //send wrong phone alert
                                await Application.Current.MainPage.DisplayAlert("Alert", "Your device is not registered on our database, please contact your employer for further support.", "OK");
                                Application.Current.Properties["id"] = "";
                                Application.Current.Properties["phone"] = "";
                                Application.Current.Properties["name"] = "";
                                Application.Current.Properties["pos"] = "";
                                return;
                            }
                        }

                        //MakeLongWebServiceCall();
                        await Application.Current.MainPage.DisplayAlert("Registration", "Your registration was successful.", "OK");
                        Application.Current.MainPage = new LoadingPage();

                       
                       
                        break;
                    }
                  
               
                  //Console.WriteLine(obj.name);
                }

                if (!Application.Current.Properties.ContainsKey("id"))
                {
                   await Application.Current.MainPage.DisplayAlert("NOT FOUND", "Sorry the ID/Phone you entered has not been found, please try a different one.", "OK");
                    Application.Current.Properties["id"] = "";
                    Application.Current.Properties["phone"] = "";
                    Application.Current.Properties["name"] = "";
                    Application.Current.Properties["pos"] = "";
                }


            }
            catch (Exception e)
            {
               await Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED, TRY AGAIN LATER.", "OK");
                Console.WriteLine("EXEPTIONS: " + e.StackTrace);
                
            }

           
        }
        public void MakeLongWebServiceCall()
        {
            Task.Delay(TimeSpan.FromSeconds(3)).Wait();
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


