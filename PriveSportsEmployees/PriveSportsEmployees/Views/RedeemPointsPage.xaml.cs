using System;
using System.Collections.Generic;
using System.Linq;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.ViewModels;
using Xamarin.Forms;

namespace PriveSportsEmployees.Views
{
    public partial class RedeemPointsPage : ContentPage
    {
        
        public RedeemPointsPage()
        {
            
            InitializeComponent();
        }

        public async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;

            Reward employee = ((EmployeeListViewModel)BindingContext).Employees.Where(emp => emp.EmployeeId == (int)tappedEventArgs.Parameter).FirstOrDefault();

            if (UserViewModel.Balance >= employee.Points)
            {
                var answer = await DisplayAlert("Confirmation", "Are you sure you want to redeem " + employee.Points + " points?", "Yes", "No");
            }
            else
            {
                await DisplayAlert("Alert", "Sorry! You don't have enough points to redeem this reward.", "OK");
            }
        }
    }
}
