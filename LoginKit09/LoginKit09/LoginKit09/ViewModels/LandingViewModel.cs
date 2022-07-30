using LoginKit09.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LoginKit09.ViewModels
{
    public class LandingViewModel : BaseViewModel
    {
        public ICommand SignInCommand => new Command(() => Application.Current.MainPage.Navigation.PushAsync(new LoginPage()));
        public ICommand RegisterCommand => new Command(() => Application.Current.MainPage.Navigation.PushAsync(new RegisterPage()));
    }
}
