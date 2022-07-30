using LoginKit09.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginKit09
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Device.SetFlags(new[] { "Shapes_Experimental", "MediaElement_Experimental", "Brush_Experimental" });

            MainPage = new NavigationPage(new LandingPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
