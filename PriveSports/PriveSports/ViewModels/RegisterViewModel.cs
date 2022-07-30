using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Input;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Collections;
using PriveSports.Views;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PriveSports.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                }
            }
        }

        public string IPTEST = "http://mmhsys.com:9080/$PosFindPeople?find=";

        public string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public string IPverotp = "http://mmhsys.com/$TableVerifyOtp";
        public string IPsave = "http://139.138.223.53:8080/$PosSavePeople?";
        public string IPsendotp = "http://mmhsys.com/$TableOtp";
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Birthdate { get; set; }
        public string Gender { get; set; }
        public string Nameday { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }

        public class User
        {
            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string bookmark { get; set; }

            [DataMember]
            public string number { get; set; }

            [DataMember]
            public string any { get; set; }

            [DataMember]
            public string other { get; set; }
        }


        public ICommand MainPageCmd => new Command(() => guest());
        public ICommand RegisterCommand => new Command(() => Register()); //Application.Current.MainPage.Navigation.PushAsync(new otpPage()));

        private void guest()
        {
            Application.Current.Properties["phone"] = null;
            Application.Current.Properties["name"] = null;
            Application.Current.Properties["birthday"] = null;
            Application.Current.Properties["nameday"] = null;
            Application.Current.Properties["gender"] = null;
            Application.Current.Properties["zipcode"] = null;
            Application.Current.Properties["email"] = null;
            Application.Current.MainPage = new AppShell();
        }

        private async void Register()
        {

            try
            {
                Hashtable ht = new Hashtable();
                ht.Add("user", "root");
                ht.Add("session-token", "tesT");

              
               

                //PRIVE SPORTS : 139.138.223.53:8080
                //string details = DoGetHttp("http://139.138.223.53/$TableGetView?"+"system=pos&file=transdet&driving=trans&fields=item trans.refer trans.date&record="+ des[0].value, "", 10000);//Getting the Details 
                //var it = JsonConvert.DeserializeObject<List<User>>(details);
                //Validations: Add your own extra validations as required

                if (string.IsNullOrWhiteSpace(Name))
                {
                    await Application.Current.MainPage.DisplayAlert("Name", "Provide your full name to continue.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Phone))
                {
                    await Application.Current.MainPage.DisplayAlert("Phone", "Provide your phone number to continue.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Birthdate))
                {
                    await Application.Current.MainPage.DisplayAlert("Birthday", "Provide your birthday to continue.", "OK");
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(ZipCode))
                {
                    await Application.Current.MainPage.DisplayAlert("Zip Code", "Provide your zip code to continue.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Gender))
                {
                    await Application.Current.MainPage.DisplayAlert("Gender", "Provide your gender to continue.", "OK");
                    return;
                }
                if (isChecked == false)
                {
                    await Application.Current.MainPage.DisplayAlert("Consent Needed", "Please provide consent to store your data.", "OK");
                    return;
                }
                if(Phone.Length != 8)
                {
                    await Application.Current.MainPage.DisplayAlert("Phone", "Please provide your phone number in the form of {99887766} to continue.", "OK");
                    return;
                }
                if (Name.Length < 8)
                {
                    await Application.Current.MainPage.DisplayAlert("Name", "Please provide your name and surname, more than 8 characters are needed.", "OK");
                    return;
                }
                await Task.Run(async () =>
                {
                    string response2 = DoGetHttp(IPfind + Phone, "", 10000);
                    if (response2 != "[]" && response2 != "" && response2 != null)
                    {
                        var des = JsonConvert.DeserializeObject<List<User>>(response2);



                        if (des[0].name == null | des[0].name == "")
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("USER FOUND", Name, "OK");
                                Application.Current.Properties["name"] = Name;
                            });
                        } 
                           
                        

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Application.Current.MainPage.DisplayAlert("USER FOUND", des[0].name, "OK");
                         
                            string result = Birthdate.Split()[0];
                   
                            string res2 = des[0].name.Split()[0];
                            Application.Current.Properties["name"] = des[0].name;
                            Application.Current.Properties["num"] = des[0].number;
                            // Application.Current.Properties["name"] = des[0];
                            Application.Current.Properties["phone"] = Phone;
                            Application.Current.Properties["birthday"] = result;
                            Application.Current.Properties["nameday"] = Nameday;
                            Application.Current.Properties["gender"] = Gender;
                            Application.Current.Properties["zipcode"] = ZipCode;
                            Application.Current.Properties["email"] = Email;
                            await Application.Current.SavePropertiesAsync();
                            string message = "Hey " + Name + " your sign up phone verification OTP is: ";
                            string OTP = DoGetHttp(IPsendotp + "?phone=" + Phone + "&message=" + message, "", 10000);
                             await Application.Current.MainPage.DisplayAlert("OTP", "OTP Sent Succesfully", "OK");
                            Application.Current.MainPage = new otpPage();
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            
                            await Application.Current.MainPage.DisplayAlert("NEW USER", "It seems you are not registered in out customers list!", "OK");
                            string message = "Hey " + Name + " your sign up phone verification OTP is: ";
                            string OTP = DoGetHttp(IPsendotp + "?phone=" + Phone + "&message=" + message, "", 10000);
                            await Application.Current.MainPage.DisplayAlert("OTP", "OTP Sent Succesfully", "OK");
                           
                            string result = Birthdate.Split()[0];
                            string res2 = Name.Split()[0];
                            Application.Current.Properties["name"] = Name;
                            Application.Current.Properties["phone"] = Phone;
                            Application.Current.Properties["birthday"] = result;
                            Application.Current.Properties["nameday"] = Name;
                            Application.Current.Properties["gender"] = Gender;
                            Application.Current.Properties["zipcode"] = ZipCode;
                            Application.Current.Properties["email"] = Email;
                            Application.Current.MainPage = new otpPage();
                        });

                    }
                });
            
            }
            catch (Exception e )
            {
                await Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED, TRY AGAIN LATER.", "OK");
                Console.WriteLine("EXEPTIONS: " + e.StackTrace);
            }

            //All validations passed, you can proceed with registration
           await  Application.Current.MainPage.DisplayAlert("Registration", "Your registration was successful.", "OK");
            //Application.Current.MainPage = new AppShell();
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



