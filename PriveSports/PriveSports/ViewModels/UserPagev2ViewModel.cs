using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PriveSports.Views;
using Xamarin.Forms;

namespace PriveSports.ViewModels
{
    public class UserPagev2ViewModel : BaseViewModel
    {
        public ICommand LogoutCommand => new Command(() => Logout());
        public ICommand EditCommand => new Command(() => Edit());
        protected string p, n, b, g, nd, zp, e;
        public UserPagev2ViewModel()
        {
            if (Application.Current.Properties.ContainsKey("phone"))
            {
                if (Application.Current.Properties["phone"] != null && (string)Application.Current.Properties["phone"] != "" && Application.Current.Properties["birthday"] != null && Application.Current.Properties["nameday"] != null && Application.Current.Properties["gender"] != null && Application.Current.Properties["zipcode"] != null)
                {
                    
                        n = Application.Current.Properties["name"].ToString();
                        p = Application.Current.Properties["phone"].ToString();
                        b = Application.Current.Properties["birthday"].ToString();
                        nd = Application.Current.Properties["nameday"].ToString();
                        g = Application.Current.Properties["gender"].ToString();
                        zp = Application.Current.Properties["zipcode"].ToString();
                    if (Application.Current.Properties["email"] != null)
                    {
                        e = Application.Current.Properties["email"].ToString();
                    }
                    else
                    {
                        e = "";
                    }

                }
            }


        }

        public string name { get { return n; } set { } }
        public string phone { get { return p; } set { } }
        public string birthday { get { return b; } set { } }
        public string nameday { get { return nd; } set { } }
        public string gender { get { return g; } set { } }
        public string zipcode { get { return zp; } set { } }
        public string email { get { return e; } set { } }

        public async void Edit()
        {
            await  Application.Current.MainPage.Navigation.PushAsync(new EditPage());
        }

        public async void Logout()
        {
            Application.Current.Properties["phone"] = null;
            Application.Current.Properties["name"] = null;
            Application.Current.Properties["birthday"] = null;
            Application.Current.Properties["nameday"] = null;
            Application.Current.Properties["gender"] = null;
            Application.Current.Properties["zipcode"] = null;
            Application.Current.Properties["email"] = null;
            await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
            Application.Current.MainPage = new RegisterPage();
            await Application.Current.SavePropertiesAsync();


        }

        
    }
}

