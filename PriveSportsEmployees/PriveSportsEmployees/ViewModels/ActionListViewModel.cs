using System;

using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
    public class ActionListViewModel : ContentPage
    {
        public ActionListViewModel()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}


