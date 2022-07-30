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
using PriveSportsEmployees.Views;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using static PriveSportsEmployees.Services.ListService;

namespace PriveSportsEmployees.ViewModels
{
	public class RedeemPointsViewModel : INotifyPropertyChanged
	{
        public string RewardsIP = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=action&report=points_reward&__internal=true";
        public string[] s;
        public string f;
        public string IPFunc = "http://139.138.223.53:8080/$TableModify";
        public ICommand redeemCommand => new Command<int>((x) => RedeemPoints(x));

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class Rewrds
        {
            [DataMember]
            public int ID { get; set; }

            [DataMember]
            public int Act_Pos { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Points_Disp { get; set; }

            [DataMember]
            public int Points { get; set; }

            [DataMember]
            public int Pos { get; set; }

        }


        public ObservableCollection<Rewrds> Rew { get; set; }
        public RedeemPointsViewModel ()
		{
            Rew = new ObservableCollection<Rewrds>();
            Rew.Clear();
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

               
                int i = 0;
                string response = DoGetHttp(RewardsIP, "", 10000);
               
                if (response != "")
                {

                    var des = JsonConvert.DeserializeObject<List<rd>>(response);
                   
                    int s = (int)Application.Current.Properties["pos"];


                    foreach (var obj in des)
                    {


                        Rew.Add(new Rewrds { ID = i, Act_Pos = (int)Application.Current.Properties["pos"], Name = obj.name, Points_Disp = "REDEEM FOR: " + obj.points + " POINTS",  Pos= obj.pos, Points = obj.points });

                        i++;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);

            }
        }

        public async void RedeemPoints(int id)
        {
           
            Rewrds reward = Rew.Where(r => r.ID == id).FirstOrDefault();
           

            if (UserViewModel.r_balance >= reward.Points)
            {
               
                var answer = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to redeem " + reward.Points + " points?", "Yes", "No");
                if (answer == true)
                {
                    DateTime now = DateTime.UtcNow.Date;
                    string pp = now.ToString().Split()[0];
                    s = pp.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                    f = s[2] + "-" + s[1] + "-" + s[0];

                    await Run(reward.Act_Pos, reward.Pos, f, reward.Points);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Sorry! You don't have enough points to redeem this reward.", "OK");
            }
        }

        public async Task Run(int empl, int action, string date, int points)
        {
            await Task.Run(async () =>
            {
                try
                {

                    string pp = "-" + points;
                    PostModify exf = new PostModify();
                    exf.contents = new List<ModifyField>();
                    exf.contents.Add(new ModifyField() { id = "date", value = date });
                    exf.contents.Add(new ModifyField() { id = "action", value = action });
                    exf.contents.Add(new ModifyField() { id = "employee", value = empl });
                    exf.contents.Add(new ModifyField() { id = "employee_from", value = empl });


                    exf.file = "pointspro";
                    exf.system = "payroll";
                    exf.record = 0;

                    string jsonData = JsonConvert.SerializeObject(exf);
                    byte[] array = System.Text.Encoding.ASCII.GetBytes(jsonData);
                    string response = PostHttp(IPFunc, array);
                    //***** WHEN GOING TO REDEEM THE SAME THING TWICE I GET THE RECORD EXISTS ERROR. ALLOW DUPLICATES!!
                    // Debugger.Break();
                    // var des = JsonConvert.DeserializeObject<List<r>>(response);
                    if (response.Contains("record"))
                    {
                       

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                        
                            await Application.Current.MainPage.DisplayAlert("SUCCESS", "Your redemption request has been sent for approval!", "OK");
                       
                            await Application.Current.MainPage.Navigation.PushAsync(new UserViewpage());
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
                catch (Exception e)
                {

                }

            });
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

    






