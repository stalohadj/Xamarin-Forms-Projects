
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;

namespace PriveSports.ViewModels
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
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationAlways>();
            }

           
        }
    }
}