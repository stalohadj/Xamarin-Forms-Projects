using System;
using System.Collections.Generic;
using PriveSports.Models;
using PriveSports.ViewModels;
using Xamarin.Forms;

namespace PriveSports.Views
{
    public partial class TransactionPagev2 : ContentPage
    {
        public TransactionPagev2()
        {
            InitializeComponent();
        }

        private void ListViewItem_Tabbed(object sender, ItemTappedEventArgs e)
        {
            var trans = e.Item as Transaction;
            var vm = BindingContext as Transaction2ViewModel;
            vm?.ShoworHiddenProducts(trans);
        }
    }
}
