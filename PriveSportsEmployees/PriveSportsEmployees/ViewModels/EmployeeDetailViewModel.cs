using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using PriveSportsEmployees.Controls;

namespace PriveSportsEmployees.ViewModels
{

    class EmployeeDetailViewModel : INotifyPropertyChanged
    {
        private Reward _employee;
        public Reward Employee
        {
            get { return _employee; }
            set { _employee = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
