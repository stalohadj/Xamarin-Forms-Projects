using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PriveSports.Models;
using System.Runtime.Serialization;
using Xamarin.Forms.Xaml;
using System.Collections;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Input;
using System.Diagnostics;
using PriveSports.Views;


namespace PriveSports.ViewModels
{
    [DesignTimeVisible(false)]
    public class TransactionPageViewModel : ContentPage
    {
        public string IPfind = "http://139.138.223.53:8080/$PosFindPeople?find=";
        public string IPtableview = "http://139.138.223.53:8080/$TableGetView?";
        public string IP = "11.0.0.99";
        public string IP2 = "11.0.0.138";

        public TransactionPageViewModel()
        {
       
            this.BindingContext = this;

            if (Application.Current.Properties.ContainsKey("phone"))
            {
                if (Application.Current.Properties["phone"] != null && (string)Application.Current.Properties["phone"] != "")
                {
                    Shell.Current.Navigation.PushAsync(new TransactionPagev2());
                    
                }
            }

        }

    }
}
