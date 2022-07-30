using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace PriveSportsEmployees.Views
{
    public partial class LocationPicker : PopupPage
    {
        public string sel_loc { get; set; }
        public string title { get; set; }
        public string title2 { get; set; }
        public string tt;
        public string IPFunc = "http://139.138.223.53:8080/$TableModify";
        public static string IPLoc = "http://139.138.223.53:8080/$TableGetView?system=system&file=location&__internal=true";
        public static string IPRej = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=reject&__internal=true";
        public int Employee_n;
        public int Action_n;
        public string Date;
        public int Points;
        public int Record_n;
        public int Employee_from;

        public ObservableCollection<string> Locs { get; set; }

        public class Rejects
        {
            [DataMember]
            public string name;
            [JsonProperty(PropertyName = "$$record")]
            public string record;

        }

        public LocationPicker(string t, int empl, int action, string date, int points, int record_n, int empl_f)
        {
            tt = t;
            Locs = new ObservableCollection<string>();

            Employee_n = empl;
            Action_n = action;
            Date = date;
            Points = points;
            Record_n = record_n;
            Employee_from = empl_f;
            BindingContext = this;
            AddPicker(t);
            InitializeComponent();

          
        }


        public void AddPicker(string type)
        {
            try
            {
                if (type == "loc")
                {
                    title = "Location";
                    title2 = "Select Location";
                    string s;
                    string response3 = DoGetHttp(IPLoc, "", 10000);
                    var des3 = JsonConvert.DeserializeObject<List<Loc_pull>>(response3);
                    foreach (var obj in des3)
                    {

                        s = obj.name;
                        if (s != "ALL STORES" && s!="DEFECTIVES")
                        {
                            Locs.Add(s);
                        }

                    }
                }
                else
                {
                    title = "Rejection Reason";
                    title2 = "Select Rejection Reason";
                    string s;
                    string response3 = DoGetHttp(IPRej, "", 10000);
                    var des3 = JsonConvert.DeserializeObject<List<Rejects>>(response3);
                    foreach (var obj in des3)
                    {

                        s = obj.record + " " + obj.name;
                        
                        Locs.Add(s);
                        

                    }
                }
            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("AN ERROR HAS OCCURED", "PLEASE CHECK YOUR NETWORK CONNECTION AND TRY AGAIN LATER.", "OK");
            }
        }

        private void CloseBtn_Clicked(object sender, EventArgs e)
        {
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        private void ConfirmBtn_Clicked(object sender, EventArgs e)
        {
            if (tt == "loc")
            {
                Application.Current.Properties["selected_loc"] = sel_loc;
                Navigation.PushAsync(new AddOrEditEmployeePage());
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
            else
            {
                string[] s = Date.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                string[] p = sel_loc.Split(' ');
                string f = s[2] + "-" + s[1] + "-" + s[0];
                PostModify exf1 = new PostModify();
                exf1.contents = new List<ModifyField>();
                exf1.contents.Add(new ModifyField() { id = "date", value = f });
                exf1.contents.Add(new ModifyField() { id = "reject", value = p[0] });
                exf1.contents.Add(new ModifyField() { id = "pointslog", value = null });
                exf1.contents.Add(new ModifyField() { id = "action", value = Action_n });
                exf1.contents.Add(new ModifyField() { id = "employee", value = Employee_n });
                exf1.contents.Add(new ModifyField() { id = "employee_from", value = Employee_from });
                exf1.file = "pointspro";
                exf1.system = "payroll";
                exf1.record = Record_n;

                string jsonData2 = JsonConvert.SerializeObject(exf1);
                byte[] array2 = Encoding.ASCII.GetBytes(jsonData2);
                string response2 = PostHttp(IPFunc, array2);
                Debugger.Break();

                if (response2.Contains("record"))
                {
                    
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("ALERT", "The suggestion has been Rejected", "OK");

                        await Navigation.PopAsync();
                    });

                }
                else
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("ALERT", "An error has occured, try again later.", "OK");

                    });

                }

                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
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
