using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{

    public class DisclaimerPopupViewModel : PopupPage
    {
        public ICommand GoHomeCommand { get; set; }
        public DisclaimerPopupViewModel()
        {
            GoHomeCommand = new Command(async () => await RequestPermissionAsync());
        }

        async Task RequestPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
           
            if (status != PermissionStatus.Granted)
            {
                // await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                await Device.InvokeOnMainThreadAsync(() => Permissions.RequestAsync<Permissions.LocationWhenInUse>());

            }
        }
    }
}

