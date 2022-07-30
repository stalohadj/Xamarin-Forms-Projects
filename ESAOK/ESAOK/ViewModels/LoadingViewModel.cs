using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ESAOK.ViewModels
{
   
        public class LoadingViewModel : ContentPage
        {
            public LoadingViewModel()
            {

                Device.InvokeOnMainThreadAsync(() => DelayedShow());

            }

            private async Task DelayedShow()
            {
                await Task.Delay(1800);
                Application.Current.MainPage = new AppShell();
            }
        }
}


