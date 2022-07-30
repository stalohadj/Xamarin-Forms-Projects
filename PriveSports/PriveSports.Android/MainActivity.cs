using System;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Xamarin.Forms;
using Android;
using AndroidX.Core.App;
using Plugin.Permissions;
using Plugin.CurrentActivity;

namespace PriveSports.Droid
{
    [Activity(Label = "Privé Sports", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDE1NDE1QDMxMzgyZTM0MmUzMEJnNWRYaTNudkRWSTJlbzdnc1lrSHRZc0lxV0JUWWJoY2F2YU1SZVZ3Yk09");
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            // CrossCurrentActivity.Current.Activity = this;
            Rg.Plugins.Popup.Popup.Init(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            //CrossCurrentActivity.Current.Init(this, savedInstanceState);

            // Remove this method to stop OneSignal Debugging  
            OneSignal.Current.SetLogLevel(LOG_LEVEL.VERBOSE, LOG_LEVEL.NONE);

            OneSignal.Current.StartInit("82809559-a634-4969-9549-b57684145e2c")
            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
            .EndInit();

            CreateNotificationFromIntent(Intent);



        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(AndroidNotificationManager.TitleKey);
                string message = intent.GetStringExtra(AndroidNotificationManager.MessageKey);
                DependencyService.Get<INotificationManager>().ReceiveNotification(title, message);
            }
        }

     

    }
}