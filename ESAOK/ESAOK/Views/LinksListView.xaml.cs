using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESAOK.Models;
using ESAOK.ViewModels;
using Xamarin.Forms;

namespace ESAOK.Views
{
    public partial class LinksListView : ContentPage
    {

        public LinksListView()
        {
            InitializeComponent();


        }

        private void ListViewItem_Tabbed(object sender, ItemTappedEventArgs e)
        {
            var link = e.Item as LinksModel;
            var vm = BindingContext as LinksViewModel;
            vm?.ShoworHiddenProducts(link);
        }

    }
}
