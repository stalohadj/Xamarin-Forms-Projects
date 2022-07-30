using System;
using PriveSports.Views;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using static Xamarin.Essentials.Permissions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace PriveSports.ViewModels
{
    public class LandingViewModel : BaseViewModel
    {
        public ICommand SignInCommand => new Command(() => Dec());

        public LandingViewModel()
        {

           /*Task.Run(async () =>
            {
                await GetLocPerm();
            });*/
        }

        public async Task GetLocPerm()
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
                    if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                        {
                            await Application.Current.MainPage.DisplayAlert("Need location", "Gunna need that location", "OK");
                        }
                        status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
                    }
                    if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        //Query permission
                    }
                    else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
                    {
                        //location denied
                    }
                });
                
            }
            catch (Exception ex)
            {
                //Something went wrong
            }
        }

        public void Dec()
        {
            Application.Current.MainPage.Navigation.PushAsync(new LoginPage());

        }
    }
    
}

