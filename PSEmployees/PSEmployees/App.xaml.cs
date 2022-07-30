using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PSEmployees
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ContainerPage();
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
