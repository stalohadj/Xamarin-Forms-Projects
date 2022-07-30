
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PriveSportsEmployees.Droid;
using PriveSportsEmployees.Controls;
using Xamarin.Forms;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(UniqueIdAndroid))]
namespace PriveSportsEmployees.Droid
{
    public class UniqueIdAndroid : IDevice
    {
          string IDevice.GetIdentifier()
        {
            var context = Android.App.Application.Context;
            string id = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Secure.AndroidId);
            return id;
        }
    }
}
