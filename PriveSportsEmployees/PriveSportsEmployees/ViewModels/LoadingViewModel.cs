using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
    
        public class LoadingViewModel : ContentPage
        {
            public static string type;
          
            public static string IPEmpl = "http://139.138.223.53:8080/$PayrollClock?employee=";
            public LoadingViewModel()
            {

                Device.InvokeOnMainThreadAsync(() => DelayedShow());

            }

            private async Task DelayedShow()
            {
                await Task.Delay(2400);
                string x = (string)Application.Current.Properties["level"];
           // Debugger.Break();
            switch ((string)Application.Current.Properties["level"])
            {
               
                case "ADMINISTRATOR":
                    type = "admin";
                   
                    Application.Current.MainPage = new AdminAppShell();
                    break;
                case "STORE MANAGER":
                    type = "manager";
                   
                    Application.Current.MainPage = new ManagerAppShell();
                    break;
                default:
                    type = "employee";
                   
                    Application.Current.MainPage = new UserAppShell();
                    break;
            }
           
            }
        }
    
}

