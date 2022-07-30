 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace PriveSports.Views
{
    public partial class FeedbackForm : PopupPage
    {
        public FeedbackForm()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        private async void OnClose(object sender, EventArgs e)
        {
            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception E)
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
