using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PriveSports.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriveSports
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        string shop;

        public AppShell()
        {
            InitializeComponent();

            shop = "DataTemplate views:PointsPage";
        }

        protected override bool OnBackButtonPressed()
        {
            // true or false to disable or enable the action
            return true;
        }
    }

}
