using System;

using Xamarin.Forms;

namespace ESAOK
{
    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}

