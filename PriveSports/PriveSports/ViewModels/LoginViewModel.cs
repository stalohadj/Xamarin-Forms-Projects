using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;
using PriveSports.Views;
using Xamarin.Forms;

namespace PriveSports.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public ICommand RegisterCommand => new Command(() => RegisterCmdAsync());
        public ICommand MainPageCmd => new Command(() => CntGuest());// Navigation.PushAsync(new AppShell()));

        public LoginViewModel()
        {
            
        }

        public async void RegisterCmdAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }

        public async void CntGuest()
        {
            Application.Current.Properties["phone"] = null;
            Application.Current.Properties["name"] = null;
            Application.Current.MainPage = new AppShell();
            await Application.Current.SavePropertiesAsync();
        }
    }
}

