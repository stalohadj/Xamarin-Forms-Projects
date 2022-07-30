using LoginKit09.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LoginKit09.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        private bool isPassword = true;
        public bool IsPassword
        {
            get { return isPassword; }
            set
            {
                isPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand => new Command(Register);
        public ICommand SignInCommand => new Command(() => Application.Current.MainPage.Navigation.PushAsync(new LoginPage()));
        public ICommand IsPasswordCommand => new Command(() => IsPassword = !IsPassword);

        private void Register()
        {
            //Validations: Add your own extra validations as required
            if (string.IsNullOrWhiteSpace(Email))
            {
                Application.Current.MainPage.DisplayAlert("Email Address", "Provide your email address to continue.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                Application.Current.MainPage.DisplayAlert("Password", "Provide your password to continue.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                Application.Current.MainPage.DisplayAlert("Password", "Your password does not match.", "OK");
                return;
            }

            //All validations passed, you can proceed with registration
            Application.Current.MainPage.DisplayAlert("Registration", "Your registration was successful.", "OK");
        }
    }
}
