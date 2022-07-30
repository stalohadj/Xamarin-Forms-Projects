using System;
using System.Collections.Generic;
using System.Linq;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Foundation;
using UIKit;
using Xamarin.Forms;
using UserNotifications;


namespace PriveSports.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            global::Xamarin.Forms.Forms.Init();
            Rg.Plugins.Popup.Popup.Init();
            Xamarin.FormsMaps.Init();
            UNUserNotificationCenter.Current.Delegate = new iOSNotificationReceiver();
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDE1NDE1QDMxMzgyZTM0MmUzMEJnNWRYaTNudkRWSTJlbzdnc1lrSHRZc0lxV0JUWWJoY2F2YU1SZVZ3Yk09");
           // Syncfusion.SfDataGrid.XForms.iOS.SfDataGridRenderer.Init();
            Device.SetFlags(new string[] { "Expander_Experimental" });
            LoadApplication(new App());

            // Remove this method to stop OneSignal Debugging  
            OneSignal.Current.SetLogLevel(LOG_LEVEL.VERBOSE, LOG_LEVEL.NONE);

            OneSignal.Current.StartInit("82809559-a634-4969-9549-b57684145e2c")
            .Settings(new Dictionary<string, bool>() {
            { IOSSettings.kOSSettingsKeyAutoPrompt, false },
            { IOSSettings.kOSSettingsKeyInAppLaunchURL, false } })
            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
            .EndInit();

            // The promptForPushNotificationsWithUserResponse function will show the iOS push notification prompt. We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 7)
            OneSignal.Current.RegisterForPushNotifications();

            return base.FinishedLaunching(app, options);

        }
    }
}
