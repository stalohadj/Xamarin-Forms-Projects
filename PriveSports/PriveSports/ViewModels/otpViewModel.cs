using PriveSports.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PriveSports.Models;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PriveSports.ViewModels
{
    public class otpViewModel : BaseViewModel
    {

        public string IPTESTfind = "http://mmhsys.com:9080/$PosFindPeople?&__internal=true&find=";
        public string IPTESTF = "http://mmhsys.com:9080/$TableExecFunction";
        public string IPTESTTT = "http://mmhsys.com:9080/$PosFindPeople?find=";

        public string IPsavetest = "http://mmhsys.com:9080/$PosSavePeople?";



        public string IPFunc = "http://139.138.223.53:8080/$TableExecFunction";
        public static string IPtableview = "http://139.138.223.53:8080/$TableGetView?";
        public string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public string IPverotp = "http://mmhsys.com/$TableVerifyOtp";
        public string IPsave = "http://139.138.223.53:8080/$PosSavePeople?";
        public string otp { get; set; }
        public DateTime tdd = DateTime.Today;
        // INotificationManager notificationManager;


        public class ModifyField
        {
            public string id { get; set; }
            public object value { get; set; }            

        }

        class _ExecFunction
        {
            public List<ModifyField> fields;
            public string system;
            public string file;
            public string function;

        }

        [DataContract]
        class NewUser
        {
            [DataMember]
            public string error { get; set; }

            [DataMember]
            public int record { get; set; }

        }

        // public ICommand MainPageCmd => new Command(() => Application.Current.MainPage.Navigation.PushAsync(new MainPage()));
        public ICommand VerifyCommand => new Command(() => OTP()); //Application.Current.MainPage.Navigation.PushAsync(new otpPage()));

        private async void OTP()
        {
   

            try {


            string response2 = DoGetHttp(IPfind + Application.Current.Properties["phone"].ToString(), "", 10000);

               

                //Validations: Add your own extra validations as required
                if (string.IsNullOrWhiteSpace(otp))
                {
                await Application.Current.MainPage.DisplayAlert("OTP", "Provide the password you received to continue.", "OK");
                return;
                }

                if (response2 != "[]" && response2 != "" && response2 != null)
                {
                   
                    DateTime x = DateTime.Now;
                    string verify = DoGetHttp(IPverotp + "?phone=" + Application.Current.Properties["phone"] + "&otp=" + otp, "", 10000);

                    if (verify == "")
                    {

                       string t =  tdd.ToString("dd/MM/yyyy");
                       string [] s = t.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                        string f = s[2] + "-" + s[1] + "-" + s[0];

                        string[] r = Application.Current.Properties["birthday"].ToString().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                        string fs = r[2] + "-" + r[1] + "-" + r[0];
                        Application.Current.Properties["closedate"] = f;

                        _ExecFunction exf = new _ExecFunction();
                        exf.fields = new List<ModifyField>();

                        exf.fields.Add(new ModifyField() { id = "name", value = Application.Current.Properties["name"] });
                        exf.fields.Add(new ModifyField() { id = "oldmobile", value = Application.Current.Properties["phone"] });
                        exf.fields.Add(new ModifyField() { id = "mobile", value = Application.Current.Properties["phone"] });
                        exf.fields.Add(new ModifyField() { id = "birthday", value = fs });
                        exf.fields.Add(new ModifyField() { id = "nameday", value = Application.Current.Properties["nameday"] });
                        exf.fields.Add(new ModifyField() { id = "gender", value = Application.Current.Properties["gender"] });
                        exf.fields.Add(new ModifyField() { id = "zip", value = Application.Current.Properties["zipcode"] });
                        exf.fields.Add(new ModifyField() { id = "email", value = Application.Current.Properties["email"] });
                        exf.fields.Add(new ModifyField() { id = "closedate", value = Application.Current.Properties["closedate"] });
                        exf.fields.Add(new ModifyField() { id = "modify", value = true });

                        exf.file = "people";
                        exf.system = "accounting";
                        exf.function = "CreateCustomer";

                        string jsonData = JsonConvert.SerializeObject(exf);
                        byte[] array = Encoding.ASCII.GetBytes(jsonData);
                        string response = PostHttp(IPFunc, array);

                        if (response == "OK!")
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("ALERT", "Registration Succesful", "OK");

                                Application.Current.MainPage = new AppShell();
                            });

                        }

                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("ALERT", "An error has occured, try again later.", "OK");
                                Application.Current.MainPage = new AppShell();

                            });

                        }

                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("ALERT", "Wrong OTP", "OK");


                    }
                }

                else
                {
                    await Task.Run(async () =>
                    {
                        string verify = DoGetHttp(IPverotp + "?phone=" + Application.Current.Properties["phone"].ToString() + "&otp=" + otp, "", 10000);
                        DateTime x = DateTime.Now;
                        

                        if (verify == "")
                        {
                            object val = Application.Current.Properties["name"];


                            string result = (string)Application.Current.Properties["birthday"];



                            string t = tdd.ToString("dd/MM/yyyy");
                            string[] s = t.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);



                            string f = s[2] + "-" + s[1] + "-" + s[0];
                            Application.Current.Properties["closedate"] = f;

                            // x = Convert.ToDateTime(result);
                            //Debugger.Break();
                            //s = x.ToString().Replace('/', '-');
                            //Debugger.Break();
                            await Application.Current.SavePropertiesAsync();
                            string[] r = Application.Current.Properties["birthday"].ToString().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                            string fs = r[2] + "-" + r[1] + "-" + r[0];

                            _ExecFunction exf = new _ExecFunction();
                            exf.fields = new List<ModifyField>();

                            exf.fields.Add(new ModifyField() { id = "name", value = Application.Current.Properties["name"] });
                            exf.fields.Add(new ModifyField() { id = "oldmobile", value = Application.Current.Properties["phone"] });
                            exf.fields.Add(new ModifyField() { id = "mobile", value = Application.Current.Properties["phone"] });
                            exf.fields.Add(new ModifyField() { id = "birthday", value = fs});
                            exf.fields.Add(new ModifyField() { id = "nameday", value = Application.Current.Properties["nameday"] });
                            exf.fields.Add(new ModifyField() { id = "gender", value = Application.Current.Properties["gender"] });
                            exf.fields.Add(new ModifyField() { id = "zip", value = Application.Current.Properties["zipcode"] });
                            exf.fields.Add(new ModifyField() { id = "email", value = Application.Current.Properties["email"] });
                            exf.fields.Add(new ModifyField() { id = "closedate", value = Application.Current.Properties["closedate"] });
                            exf.fields.Add(new ModifyField() { id = "modify", value = false });

                            exf.file = "people";
                            exf.system = "accounting";
                            exf.function = "CreateCustomer";

                            string jsonData = JsonConvert.SerializeObject(exf);

                            byte[] array = Encoding.ASCII.GetBytes(jsonData);

                            string response = PostHttp(IPFunc, array);



                            if (response == "OK!")
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    await Application.Current.MainPage.DisplayAlert("ALERT", "Registration Successful!", "OK");


                                    Application.Current.MainPage = new AppShell();
                                });

                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    await Application.Current.MainPage.DisplayAlert("ALERT", "An error has occured, try again later.", "OK");
                                    Application.Current.Properties["phone"] = null;
                                    Application.Current.Properties["name"] = null;
                                    Application.Current.Properties["birthday"] = null;
                                    Application.Current.Properties["nameday"] = null;
                                    Application.Current.Properties["gender"] = null;
                                    Application.Current.Properties["zipcode"] = null;
                                    Application.Current.Properties["email"] = null;

                                    Application.Current.MainPage = new RegisterPage();
                                });

                            }

                       
                

                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("ALERT", "Wrong OTP", "OK");
                            });



                        }


                    });



                }
            
               
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("AN ERROR HAS OCCURED", "PLEASE CHECK YOUR NETWORK CONNECTION AND TRY AGAIN LATER.", "OK");
                Console.WriteLine("HERE: "  + e.StackTrace);
                Application.Current.MainPage = new AppShell();
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


        static public Object _GetHttp2(string sUri, Hashtable ht, int nTimeout)
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
        public string PostHttp(string sUri, byte[] buffer, string user = "*", string content = "")
        {

            Object _response = _PostHttp(sUri, buffer, null, user, content, "");

            if (_response == null || _response is string)
                return "";

            HttpWebResponse response = (HttpWebResponse)_response;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string tmp = reader.ReadToEnd();
            response.Close();
            return tmp;
        }

        static public object _PostHttp(string sUri, byte[] buffer, Hashtable ht, string user, string content = "", string accept = "")
        {
            Uri uri = new Uri(sUri);

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                HttpWebRequest request = null;
                try
                {
                    if (uri.Scheme == Uri.UriSchemeHttps)
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Method = "POST"; // WebRequestMethods.Http.Post;
                    request.Timeout = 10 * 1000;

                    if (string.IsNullOrEmpty(content))
                        request.ContentType = "multipart/form-data";
                    else
                        request.ContentType = content;

                    if (!string.IsNullOrEmpty(accept))
                    {
                        request.Accept = accept;
                        request.ContentLength = buffer.Length;
                    }

                    if (string.IsNullOrEmpty(user))
                        user = Environment.UserName;


                    if (string.IsNullOrEmpty(content))
                    {
                        request.Headers.Add("User", user);
                        request.Headers.Add("Session-Token", new Guid().ToString());

                        if (ht != null)
                        {
                            ht.Remove("User");
                            ht.Remove("Session-Token");
                        }
                    }

                    if (ht != null)
                        foreach (DictionaryEntry pair in ht)
                            request.Headers.Add((string)pair.Key, (string)pair.Value);


                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    return response;


                }
                catch (Exception e)
                {
                    string message = "Exception on uri (post) : " + sUri + ", Error: " + e.Message + "\n" + 10 * 1000;


                    return '\\' + e.Message;
                }
            }
            else
            {

                return "Not a uri scheme!";
            }

        }
    }
}


