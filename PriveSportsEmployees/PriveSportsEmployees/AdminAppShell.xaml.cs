using System;
using System.Collections.Generic;
using PriveSportsEmployees.Views;
using Xamarin.Forms;

namespace PriveSportsEmployees
{
    public partial class AdminAppShell : Shell
    {
        
        public AdminAppShell()
        {
            InitializeComponent();
            //FlyoutBehavior = FlyoutBehavior.Locked;
            // FlyoutIsPresented = true;

           // ((AppShell)Shell.Current).MyTabbar1.Items[0] = new ShellContent { Icon = "StarChecked.png", Title = "test", Content = new PunchPage() };
        }

        
    }
}
