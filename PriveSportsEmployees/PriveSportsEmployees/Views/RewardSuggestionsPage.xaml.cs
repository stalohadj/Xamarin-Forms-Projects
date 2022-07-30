using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PriveSportsEmployees.Views
{
    public partial class RewardSuggestionsPage : ContentPage
    {

        public RewardSuggestionsPage()
        {
           
            InitializeComponent();
        }
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
             string loc = "loc";
             Application.Current.MainPage.Navigation.PushPopupAsync(new LocationPicker(loc, 0, 0,"",0, 0, 0));

            // Navigation.PushAsync(new AddOrEditEmployeePage());
        }

        private void TapGestureRecognizer_Tapped_Edit(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Reward employee = ((EmployeeListViewModel)BindingContext).Employees.Where(emp => emp.EmployeeId == (int)tappedEventArgs.Parameter).FirstOrDefault();

            Navigation.PushAsync(new AddOrEditEmployeePage(employee));
        }

        private void TapGestureRecognizer_Tapped_Remove(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Reward employee = ((EmployeeListViewModel)BindingContext).Employees.Where(emp => emp.EmployeeId == (int)tappedEventArgs.Parameter).FirstOrDefault();

            ((EmployeeListViewModel)BindingContext).Employees.Remove(employee);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Navigation.PushAsync(new EmployeeDetailPage((Reward)e.SelectedItem));
        }
    }
}
