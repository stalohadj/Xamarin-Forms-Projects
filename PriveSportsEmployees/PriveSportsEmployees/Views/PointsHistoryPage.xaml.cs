using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PriveSportsEmployees.Views
{
    public partial class PointsHistoryPage : ContentPage
    {
        public PointsHistoryPage()
        {
            InitializeComponent();
        }

        private void ListViewItem_Tabbed(object sender, ItemTappedEventArgs e)
        {
            //var link = e.Item as LinksModel;
            //var vm = BindingContext as LinksViewModel;
          //  vm?.ShoworHiddenProducts(link);
        }
    }
}
