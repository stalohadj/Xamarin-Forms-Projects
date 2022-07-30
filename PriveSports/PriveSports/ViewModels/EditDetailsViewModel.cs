using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSports.Views;
using Xamarin.Forms;

namespace PriveSports.ViewModels
{
    public class EditDetailsViewModel : BaseViewModel
    {
        public string IPFunc = "http://139.138.223.53:8080/$TableExecFunction";

     //   public string IPTESTF = "http://mmhsys.com:9080/$TableExecFunction";

        public ICommand SaveCmd => new Command(async () => await SaveAsync());
        public ICommand CancelCmd => new Command(async () => await CancelAsync());
        public string N { get; set; }
        public string P { get; set; }
        public string B { get; set; }
        public string G { get; set; }
        public string Z { get; set; }
        public string E { get; set; }
        public string flag { get; set; }
        string old_n;
        string old_p;
        string old_b;
        string old_g;
        string old_z;
        string old_e;
        string [] s;
        string f; 
        public DateTime x = new DateTime();

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

        public EditDetailsViewModel()
        {
             old_n = Application.Current.Properties["name"].ToString();
             old_p = Application.Current.Properties["phone"].ToString();
             old_b = Application.Current.Properties["birthday"].ToString();
             old_g = Application.Current.Properties["gender"].ToString();
             old_z = Application.Current.Properties["zipcode"].ToString();


            N = Application.Current.Properties["name"].ToString();
            P = Application.Current.Properties["phone"].ToString();
            B = Application.Current.Properties["birthday"].ToString();
      
            G = Application.Current.Properties["gender"].ToString();
            Z = Application.Current.Properties["zipcode"].ToString();
            if (Application.Current.Properties["email"] != null && Application.Current.Properties["email"] != "")
            {
                E = Application.Current.Properties["email"].ToString();
                old_e = Application.Current.Properties["email"].ToString();

            }
            else
            {

                E = "";
            }


        }


        public async Task SaveAsync()
        {
       
            if (string.IsNullOrWhiteSpace(N))
            {
                await Application.Current.MainPage.DisplayAlert("NAME", "This field cannot be blank, please provide your name to continue.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(P))
            {
                await Application.Current.MainPage.DisplayAlert("PHONE", "This field cannot be blank, please provide your phone number to continue.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(B))
            {
                await Application.Current.MainPage.DisplayAlert("BIRTHDAY", "This field cannot be blank, please provide your birthday to continue.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(G))
            {
                await Application.Current.MainPage.DisplayAlert("GENDER", "This field cannot be blank, please provide your gender to continue.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Z))
            {
                await Application.Current.MainPage.DisplayAlert("ZIPCODE", "This field cannot be blank, please provide your zipcode to continue.", "OK");
                return;
            }

            try
            {
              
                string oldphone = (string)Application.Current.Properties["phone"];
                string result = B.Split()[0];
               
                if (P!="" & P!=null)
                {
                   // oldphone = (string)Application.Current.Properties["phone"];
                    Application.Current.Properties["phone"] = P;
                    await Application.Current.SavePropertiesAsync();
                }
               

                if (N!= "" & N!= null)
                {
                    Application.Current.Properties["name"] = N;
                    await Application.Current.SavePropertiesAsync();
                }

                if (G != "" & G != null)
                {
                    Application.Current.Properties["gender"] = G;
                    await Application.Current.SavePropertiesAsync();
                }

                if (Z != "" & Z != null)
                {
                    Application.Current.Properties["zipcode"] = Z;
                    await Application.Current.SavePropertiesAsync();
                }

                if (B != "" & B != null)
                {
                    Application.Current.Properties["birthday"] = result;
                    s = result.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    //x = DateTime.ParseExact(result.ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                    //Debugger.Break();
                    f = s[2] + "-" + s[0] + "-" + s[1];
                   // x = Convert.ToDateTime(result);
                    //Debugger.Break();
                    //s = x.ToString().Replace('/', '-');
                    //Debugger.Break();
                    await Application.Current.SavePropertiesAsync();
                }

                if (E != "" & E != null)
                {
                    Application.Current.Properties["email"] = E;
                    await Application.Current.SavePropertiesAsync();
                }

                await Application.Current.SavePropertiesAsync();

                await Task.Run(async () =>
                {
                    _ExecFunction exf = new _ExecFunction();

                    Console.WriteLine("HERE: " + Application.Current.Properties["birthday"]);


                    exf.fields = new List<ModifyField>();
                    exf.fields.Add(new ModifyField() { id = "name", value = Application.Current.Properties["name"] });
                    exf.fields.Add(new ModifyField() { id = "oldmobile", value = oldphone });
                    exf.fields.Add(new ModifyField() { id = "mobile", value = Application.Current.Properties["phone"] });
                    exf.fields.Add(new ModifyField() { id = "birthday", value = f });
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
                            await Application.Current.MainPage.DisplayAlert("SAVED", "Your changes have been saved", "OK");
                            await Application.Current.MainPage.Navigation.PushAsync(new UserPagev2());
                        });

                    }

                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Application.Current.MainPage.DisplayAlert("ALERT", "An error has occured, try again later.", "OK");
                            Application.Current.Properties["phone"] = old_p;
                            Application.Current.Properties["name"] = old_n;
                            Application.Current.Properties["birthday"] = old_b;
                            Application.Current.Properties["gender"] = old_g;
                            Application.Current.Properties["zipcode"] = old_z;
                            Application.Current.Properties["email"] = old_e;
                            await Application.Current.SavePropertiesAsync();
                            Application.Current.MainPage = new UserPagev2();
                        });

                    }

                });


               
            }
            catch (Exception e)
            {
                Console.WriteLine("ERRORS: " + e.StackTrace);
                await Application.Current.MainPage.DisplayAlert("ERROR", "AN ERROR HAS OCCURED PLEASE TRY AGAIN LATER", "OK");
               
            }

        }

        public async Task CancelAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new UserPagev2());
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

