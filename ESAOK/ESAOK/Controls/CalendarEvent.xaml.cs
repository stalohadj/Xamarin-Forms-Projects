using System;
using System.Collections.Generic;
using ESAOK.Models;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ESAOK.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarEvent : ContentView
    {
        public static BindableProperty CalendarEventCommandProperty =
            BindableProperty.Create(nameof(CalendarEventCommand), typeof(ICommand), typeof(CalendarEvent), null);

        public CalendarEvent()
        {
            InitializeComponent();
        }

        public ICommand CalendarEventCommand
        {

            get => (ICommand)GetValue(CalendarEventCommandProperty);
            set => SetValue(CalendarEventCommandProperty, value);
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            
        }
    }
}
