using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.ViewModels;
using Xamarin.Forms;

namespace PriveSportsEmployees.Views
{
    public partial class AddOrEditEmployeePage : ContentPage
    {
        // 11.0.0.124:8080 139.138.223.53:8080
        private string[] s;
        private string [] x;
        private string[] p;
        private string f;
       
        public string IPFunc = "http://139.138.223.53:8080/$TableModify";

    

       

        public AddOrEditEmployeePage(Reward employee = null)
        {
            InitializeComponent();


            
            if (employee != null)
            {
                ((AddOrEditEmployeeViewModel)BindingContext).Employee = employee;
                //Debugger.Break();
            }
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            DateTime now = DateTime.UtcNow.Date;
            string pp = now.ToString().Split()[0];
            s = pp.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            f = s[2] + "-" + s[1] + "-" + s[0];
          

            Reward employee = ((AddOrEditEmployeeViewModel)BindingContext).Employee;
            //p = employee.Actionpos;
            //x = employee.EmployeeNum;
            p = employee.Designation.Split('.');
            x = employee.EmployeeName.Split(' ');
            MessagingCenter.Send(this, "AddOrEditEmployee", employee);
          
             Run();

            Navigation.PopAsync();


        }

        public async Task Run()
        {
            await Task.Run(async () =>
            {
                try
                {
                    PostModify exf = new PostModify();

                    exf.contents = new List<ModifyField>();
                    exf.contents.Add(new ModifyField() { id = "date", value = f });
                    exf.contents.Add(new ModifyField() { id = "action", value = p[0] });
                    exf.contents.Add(new ModifyField() { id = "employee", value = x[0] });
                    exf.contents.Add(new ModifyField() { id = "employee_from", value = Application.Current.Properties["pos"] });

                   

                    exf.file = "pointspro";
                    exf.system = "payroll";
                    exf.record = 0;

                    string jsonData = JsonConvert.SerializeObject(exf);
                    byte[] array = Encoding.ASCII.GetBytes(jsonData);
                    string response = PostHttp(IPFunc, array);
                 

                    if (response.Contains("record"))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Application.Current.MainPage.DisplayAlert("SENT", "Your suggestion has been sent for approval.", "OK");

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
                } catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                   
                    
                }
            });
        
        }

        public async Task CancelAsync()
        {
            await Navigation.PopAsync();
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
