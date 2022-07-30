using System;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using PriveSports.ViewModels;

namespace PriveSports.ViewModels
{
    public class TransPopupViewModel : PopupPage
    {
        public string TransDate { get; set; }
        public string TransDeets { get; set; }
        public string TransP { get; set; }

        public TransPopupViewModel()
        {
            try
            {
                TransDate = Transaction2ViewModel.dt;
                TransDeets = Transaction2ViewModel.trs;
                TransP = Transaction2ViewModel.tp;
            }
            catch(Exception e)
            {
                Application.Current.MainPage.DisplayAlert("AN ERROR HAS OCCURED", "PLEASE CHECK YOUR NETWORK CONNECTION AND TRY AGAIN LATER.", "OK");
            }
        }
    }
}

