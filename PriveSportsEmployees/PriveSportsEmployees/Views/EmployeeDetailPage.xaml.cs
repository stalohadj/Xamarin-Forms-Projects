using System;
using System.Collections.Generic;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.ViewModels;
using Xamarin.Forms;

namespace PriveSportsEmployees.Views
{
    public partial class EmployeeDetailPage : ContentPage
    {
        public EmployeeDetailPage(Reward employee = null)
        {
            InitializeComponent();

            if (employee != null)
                ((EmployeeDetailViewModel)BindingContext).Employee = employee;
        }
    }
}
