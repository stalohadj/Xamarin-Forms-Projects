using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriveSports.Models;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;


namespace PriveSports.ViewModels
{
    public partial class Newsletter_popup : PopupPage
    {
        public Newsletter_popup()
        {
            InitializeComponent();
        }

        private async void OnClose(object sender, EventArgs e)
        {
            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch(Exception E)
            {
                //Application.Current.MainPage = new AppShell();
            }
            // a
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return Content.FadeTo(1);
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return Content.FadeTo(0.5);
        }

        private void ListViewItem_Tabbed(object sender, ItemTappedEventArgs e)
        {
            var link = e.Item as ListViewModel;
            var vm = BindingContext as Newsletter_PopupViewModel;
            vm?.ShoworHiddenProducts(link);
        }

        public static class PopUpService
        {
            public static async Task PopUpAll()
            {
                if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Any())
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                }
            }
        }
    }

}