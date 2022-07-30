using LoginKit09.Views;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace LoginKit09.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

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


        public ICommand LoginCommand => new Command(Login);
        public ICommand FingerprintCommand => new Command(FingerprintAuth);
        public ICommand SignUpCommand => new Command(() => Application.Current.MainPage.Navigation.PushAsync(new RegisterPage()));
        public ICommand IsPasswordCommand => new Command(() => IsPassword = !IsPassword);
        public ICommand ForgotPasswordCommand => new Command(() =>
        {
            //Do something here when Forgot Password is tapped
            Application.Current.MainPage.DisplayAlert("Forgot Password", "You tapped on forgot password.", "OK");
        });


        private void Login()
        {
            //Validations: Add your own extra validations as required
            if (string.IsNullOrWhiteSpace(Email))
            {
                Application.Current.MainPage.DisplayAlert("Email Address", "Provide your email address to continue", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                Application.Current.MainPage.DisplayAlert("Password", "Provide your password to continue", "OK");
                return;
            }

            //All validations passed, you can proceed to the app
            Application.Current.MainPage.DisplayAlert("Login", "Your login was successful.", "OK");
        }

        private async void FingerprintAuth()
        {
            var result = await CrossFingerprint.Current.IsAvailableAsync(true);

            if (result)
            {
                var auth = await CrossFingerprint.Current.AuthenticateAsync(new AuthenticationRequestConfiguration("Fingerprint Authentication", "Use your fingerprint for authentication"), new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

                if (auth.Authenticated)
                {
                    //Fingerprint validation passed, you can proceed to the app
                    await Application.Current.MainPage.DisplayAlert("Fingerprint Login", "Your fingerprint login was successful.", "OK");
                }
            }
        }
    }
}
