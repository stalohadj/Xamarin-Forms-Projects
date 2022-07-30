using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ESAOK.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SimplePage : ContentPage
    {
        private NavigationPage MainPage;

        public SimplePage()
        {
            InitializeComponent();
        }
    }
}
