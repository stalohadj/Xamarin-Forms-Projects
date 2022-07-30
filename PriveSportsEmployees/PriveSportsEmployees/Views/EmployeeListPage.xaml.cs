using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PriveSportsEmployees.Views
{
    public partial class EmployeeListPage : ContentPage
    {

        private bool flag = false;
        
        public string IPFunc = "http://139.138.223.53:8080/$TableModify";
        public EmployeeListPage()
        {
            InitializeComponent();
        }

 

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddOrEditEmployeePage());
        }

        private async void TapGestureRecognizer_Tapped_Accept(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;

            Reward employee = ((EmployeeListViewModel)BindingContext).Employees.Where(emp => emp.EmployeeId == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var answer = await DisplayAlert("Confirmation", "This suggestion will be accepted. Are you sure you want to proceed?", "Yes", "No");
            if (answer == true)
            {
              await  Run(employee.EmployeeNum, employee.Actionpos, employee.Date, employee.Points, employee.Record, employee.Employee_from, employee.Act_Type);
                if( flag == true)
                {
                    ((EmployeeListViewModel)BindingContext).Employees.Remove(employee);
                    flag = false;
                }
            }
            
        }

     

        private void TapGestureRecognizer_Tapped_Reject(object sender, EventArgs e)
        {
            string rej = "rej";
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Reward employee = ((EmployeeListViewModel)BindingContext).Employees.Where(emp => emp.EmployeeId == (int)tappedEventArgs.Parameter).FirstOrDefault();
            Application.Current.MainPage.Navigation.PushPopupAsync(new LocationPicker(rej, employee.EmployeeNum, employee.Actionpos, employee.Date, employee.Points, employee.Record, employee.Employee_from));
            
            ((EmployeeListViewModel)BindingContext).Employees.Remove(employee);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Navigation.PushAsync(new EmployeeDetailPage((Reward)e.SelectedItem));
        }

        public async Task Run(int empl, int action, string date, int points, int record_n, int empl_f, string type)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (type == "Action")
                    {
                        string[] s = date.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                        string f = s[2] + "-" + s[1] + "-" + s[0];



                        PostModify exf = new PostModify();
                        exf.contents = new List<ModifyField>();
                        exf.contents.Add(new ModifyField() { id = "date", value = f });
                        exf.contents.Add(new ModifyField() { id = "action", value = action });
                        exf.contents.Add(new ModifyField() { id = "employee", value = empl });
                        exf.contents.Add(new ModifyField() { id = "points", value = points });

                        exf.file = "pointslog";
                        exf.system = "payroll";
                        exf.record = 0;

                        string jsonData = JsonConvert.SerializeObject(exf);
                        byte[] array = Encoding.ASCII.GetBytes(jsonData);
                        string response = PostHttp(IPFunc, array);
                        // var des = JsonConvert.DeserializeObject<List<r>>(response);
                        if (response.Contains("record"))
                        {
                            string[] x = response.Split('{', ':', '}');

                            PostModify exf1 = new PostModify();
                            exf1.contents = new List<ModifyField>();
                            exf1.contents.Add(new ModifyField() { id = "date", value = f });
                            exf1.contents.Add(new ModifyField() { id = "pointslog", value = x[2] });
                            exf1.contents.Add(new ModifyField() { id = "action", value = action });
                            exf1.contents.Add(new ModifyField() { id = "employee", value = empl });
                            exf1.contents.Add(new ModifyField() { id = "employee_from", value = empl_f });
                            exf1.file = "pointspro";
                            exf1.system = "payroll";
                            exf1.record = record_n;

                            string jsonData2 = JsonConvert.SerializeObject(exf1);
                            byte[] array2 = Encoding.ASCII.GetBytes(jsonData2);
                            string response2 = PostHttp(IPFunc, array2);

                            Debugger.Break();
                            if (response2.Contains("record"))
                            {
                                flag = true;
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    await Application.Current.MainPage.DisplayAlert("ALERT", "The suggestion has been approved", "OK");

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


                        }


                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("ALERT", "An error has occured, try again later.", "OK");

                            });

                        }
                    }
                    else
                    {
                        string[] s = date.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                        string f = s[2] + "-" + s[1] + "-" + s[0];

                        

                        PostModify exf = new PostModify();
                        exf.contents = new List<ModifyField>();
                        exf.contents.Add(new ModifyField() { id = "date", value = f });
                        exf.contents.Add(new ModifyField() { id = "action", value = action });
                        exf.contents.Add(new ModifyField() { id = "employee", value = empl });
                        exf.contents.Add(new ModifyField() { id = "points", value = -points });

                        exf.file = "pointslog";
                        exf.system = "payroll";
                        exf.record = 0;

                        string jsonData = JsonConvert.SerializeObject(exf);
                        byte[] array = Encoding.ASCII.GetBytes(jsonData);
                        string response = PostHttp(IPFunc, array);
                        // var des = JsonConvert.DeserializeObject<List<r>>(response);
                        if (response.Contains("record"))
                        {
                            string[] x = response.Split('{', ':', '}');

                            PostModify exf1 = new PostModify();
                            exf1.contents = new List<ModifyField>();
                            exf1.contents.Add(new ModifyField() { id = "date", value = f });
                            exf1.contents.Add(new ModifyField() { id = "pointslog", value = x[2] });
                            exf1.contents.Add(new ModifyField() { id = "action", value = action });
                            exf1.contents.Add(new ModifyField() { id = "employee", value = empl });
                            exf1.contents.Add(new ModifyField() { id = "employee_from", value = empl_f });
                            exf1.file = "pointspro";
                            exf1.system = "payroll";
                            exf1.record = record_n;

                            string jsonData2 = JsonConvert.SerializeObject(exf1);
                            byte[] array2 = Encoding.ASCII.GetBytes(jsonData2);
                            string response2 = PostHttp(IPFunc, array2);

        
                            if (response2.Contains("record"))
                            {
                                flag = true;
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    await Application.Current.MainPage.DisplayAlert("ALERT", "The reward has been approved", "OK");

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


                        }


                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("ALERT", "An error has occured, try again later.", "OK");

                            });

                        }
                    }
                }
                catch(Exception e)
                {

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
