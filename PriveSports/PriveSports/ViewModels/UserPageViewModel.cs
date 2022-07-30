using System;
using PriveSports.Views;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PriveSports.ViewModels
{
    public class UserPageViewModel : BaseViewModel
    {
        public ICommand RegisterCommand => new Command(() => Application.Current.MainPage = new RegisterPage());
        bool flag = true;
        

        public UserPageViewModel()
        {
            //  Application.Current.MainPage.DisplayAlert("ok", Application.Current.Properties["name"].ToString(), "ok");
            if (Application.Current.Properties.ContainsKey("phone"))
            {
                if (Application.Current.Properties["phone"] != null && (string)Application.Current.Properties["phone"] != "")
                {
                    flag = false;
                    Application.Current.MainPage.Navigation.PushAsync(new UserPagev2());
                }

               

            }
           
           
        }

        public bool isVisible { get { return flag; } set { } }
    }
}

