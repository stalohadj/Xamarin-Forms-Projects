using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriveSports.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopPage : ContentPage
    {
        public ShopPage()
        {
            InitializeComponent();
            
            this.BindingContext = this;
        }

        void webviewNavigating(object sender, WebNavigatingEventArgs e)
        {
            labelLoading.IsVisible = true;
        }

        void webviewNavigated(object sender, WebNavigatedEventArgs e)
        {
            labelLoading.IsVisible = false;
        }

        async void OnBackButtonClicked(object sender, EventArgs e)
        {
            myWebView.GoBack();
            try
            {
                
                  
               
            }
            catch(Exception)
            {

            }
        }

        async void OnForwardButtonClicked(object sender, EventArgs e)
        {
            myWebView.GoForward();
            try
            {
               
                  
                
            }
            catch(Exception)
            {

            }
        }
    }


}
