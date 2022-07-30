using Foundation;
using LoginKit09.iOS.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]

namespace LoginKit09.iOS.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            try
            {
                Control.BorderStyle = UITextBorderStyle.None;
                Control.Layer.BorderWidth = 0;
            }
            catch (Exception)
            {
                ;
            }
        }
    }
}