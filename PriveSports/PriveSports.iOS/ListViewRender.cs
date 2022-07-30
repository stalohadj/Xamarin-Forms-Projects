using System;
using PriveSports.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRender))]
namespace PriveSports.iOS
{
    public class ListViewRender : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            ((UITableViewController)ViewController).RefreshControl.TintColor = UIColor.SystemOrangeColor;
        }
    }
}
