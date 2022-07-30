using System;
using System.Collections.Generic;

using Xamarin.Forms;



namespace PriveSports.Views
{
    public partial class TransactionPage : ContentPage
    {
        //
        public TransactionPage()
        {
            /**
            SfDataGrid dataGrid;

            dataGrid = new SfDataGrid();
            TransactionInfoRepository viewModel = new TransactionInfoRepository();
            dataGrid.ItemsSource = viewModel.TransInfoCollection;
            dataGrid.AllowPullToRefresh = true;
            dataGrid.PullToRefreshCommand = new Command(ExecutePullToRefreshCommand);
            // Application.Current.MainPage = new ContentPage { Content = dataGrid };
            */

            InitializeComponent();

           
            
        }
        /**
        private TransactionInfoRepository trans;
        private async void ExecutePullToRefreshCommand()
        {
            this.dataGrid.IsBusy = true;
            await Task.Delay(new TimeSpan(0, 0, 5));
            trans.Refresh();
            this.dataGrid.IsBusy = false;
        }

        private ObservableCollection<Transaction> transInfo;

        public ObservableCollection<Transaction> TransInfo
        {
            get { return transInfo; }
            set { this.transInfo = value; }
        }

        
        */
        }
}
