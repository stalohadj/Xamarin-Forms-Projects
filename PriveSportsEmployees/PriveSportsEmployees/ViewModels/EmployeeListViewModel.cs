using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.Services;
using PriveSportsEmployees.Views;
using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
    class EmployeeListViewModel : INotifyPropertyChanged
    {

        ListService employeeService;
        public string[] s;
        public string f;
        public string IPFunc = "http://139.138.223.53:8080/$TableModify";
        public ICommand searchCommand => new Command<string>(LoadEmployees);
       // 

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Reward> _employees;
        public ObservableCollection<Reward> Employees
        {
            get { return _employees; }
            set { _employees = value; OnPropertyChanged(); }
        }
        public string EmployeeName { get; set; }
        public string SelectedEmployee { get; set; }
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    LoadEmployees("");

                    IsRefreshing = false;
                });
            }
        }

        public EmployeeListViewModel()
        {

            employeeService = new ListService(LoadingViewModel.type);
            LoadEmployees(string.Empty);

            MessagingCenter.Subscribe<AddOrEditEmployeePage, Reward>(this, "AddOrEditEmployee",
                (page, employee) =>
                {
                    if (employee.EmployeeId == 0)
                    {

                        employee.EmployeeId = Employees.Count + 1;
                        Employees.Add(employee);

                    }
                    else
                    {
                        Reward employeeToEdit = Employees.Where(emp => emp.EmployeeId == employee.EmployeeId).FirstOrDefault();

                        int newIdex = Employees.IndexOf(employeeToEdit);
                        Employees.Remove(employeeToEdit);

                        Employees.Add(employee);
                        int oldIndex = Employees.IndexOf(employee);

                        Employees.Move(oldIndex, newIdex);
                    }
                }
                );
        }

        public void LoadEmployees(string query)
        {

            IsBusy = true;
            Task.Run(async () =>
            {
                Employees = await employeeService.GetEmployeesAsync(query, LoadingViewModel.type);
                IsBusy = false;
            });
        }

    }
}
