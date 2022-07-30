using System;
using System.Collections.Generic;
using System.Text;

namespace PriveSportsEmployees.Controls
{
    public class Reward
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Date { get; set; }
        public int EmployeeNum { get; set; }
        public int Actionpos { get; set; }
        public int Record { get; set; }
        public int Points { get; set; }
        public int Employee_from { get; set; }
        public string Act_Type { get; set; }

        public Reward(int employeeId, string employeeName, string designation, string date, int employeenum, int actionpos, int record, int points, int empl_f, string type)
        {
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            Designation = designation;
            Date = date;
            EmployeeNum = employeenum;
            Actionpos = actionpos;
            Record = record;
            Points = points;
            Employee_from = empl_f;
            Act_Type = type;
     
        }

        public Reward()
        {
        }
    }
}
