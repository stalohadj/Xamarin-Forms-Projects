using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriveSportsEmployees.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PunchPage : ContentPage
    {

        public PunchPage()
        {
            InitializeComponent();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                clock.Text = DateTime.Now.ToString("HH:mm")
                ) ;

                return true;
            });
        }
       
    }
}
