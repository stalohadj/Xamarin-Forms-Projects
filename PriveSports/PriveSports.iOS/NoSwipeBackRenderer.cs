using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(PriveSports.Views.TransactionPagev2),typeof(PriveSports.iOS.NoSwipeBackRenderer))]
namespace PriveSports.iOS
{
    public class NoSwipeBackRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            var navctrl = this.ViewController.NavigationController;
            navctrl.InteractivePopGestureRecognizer.Enabled = false;
        }
    }
}