
using System;
using Rg.Plugins.Popup.Services;

namespace PriveSports.Views
{
    public partial class FeedbackPopupPage 
    {
        public FeedbackPopupPage()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
