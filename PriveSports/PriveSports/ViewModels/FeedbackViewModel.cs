using System;
using System.Windows.Input;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace PriveSports.ViewModels
{
    public class FeedbackViewModel : PopupPage
    {
        public string Feedback { get; set; }

        public ICommand sendfeedback => new Command(() => Send());

        public void Send()
        {

        }
    }
}

