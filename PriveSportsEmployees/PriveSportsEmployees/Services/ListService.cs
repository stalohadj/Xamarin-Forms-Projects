using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriveSportsEmployees.Controls;
using PriveSportsEmployees.ViewModels;
using Xamarin.Forms;

namespace PriveSportsEmployees.Services
{
    class ListService
    {
        public string[] p; 
        //$action
        public class Action
        {
            [DataMember]
            public string date { get; set; }
            [JsonProperty(PropertyName = "action.name")]
            public string action { get; set; }
            [JsonProperty(PropertyName = "action.*record")]
            public int action_pos { get; set; }
            [JsonProperty(PropertyName = "action.type")]
            public string type { get; set; }
            [DataMember]
            public string employee { get; set; }
            [JsonProperty(PropertyName = "$employee")]
            public int employeenum { get; set; }
            [DataMember]
            public string pointslog { get; set; }
            [JsonProperty(PropertyName = "$employee_from")]
            public int employee_from { get; set; }
            [JsonProperty(PropertyName = "$$record")]
            public int record { get; set; }
            [JsonProperty(PropertyName = "action.points")]
            public int points { get; set; }
            [DataMember]
            public string reject { get; set; }
        }

        public class rd
        {
            [DataMember]
            public string name { get; set; }
            [JsonProperty(PropertyName = "$$position")]
            public int pos { get; set; }
            [DataMember]
            public int points { get; set; }
        }
        public ObservableCollection<Reward> Employees { get; set; }
        public string SuggsIP = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointspro&report=points_pending&__internal=true";
        public string RewardsIP = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=action&report=points_reward&__internal=true";
        public string IPBalance = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointslog&driving=employee&__internal=true&from=";
        public string IPEmpl = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=employee&report=employees_app&__internal=true";
        public string IPpointem = "http://139.138.223.53:8080/$TableGetView?system=payroll&file=pointslog&report=employees_with_points&__internal=true";
        public ListService(string type)
        {
            Employees = new ObservableCollection<Reward>();
            InitializeEmployeeService(type);
        }

        public void InitializeEmployeeService(string t)
        {
           


            if (t == "employee")
            {

                try
                {


                    int i = 0;
                    string response = DoGetHttp(RewardsIP, "", 10000);
                    if (response != "")
                    {

                        var des = JsonConvert.DeserializeObject<List<rd>>(response);

                        int s = (int)Application.Current.Properties["pos"];


                        foreach (var obj in des)
                        {


                                    Employees.Add(new Reward(i, obj.name, "REDEEM FOR: " + obj.points + " POINTS", "", (int)Application.Current.Properties["pos"], obj.pos, 0, obj.points, 0,""));
                            //1. int employeeId, 2. string employeeName, 3. string designation, 4. string date, 5. int employeenum, 6. int actionpos, 7.  int record, 8. int points, 9. int empl_f
                            i++;
                                
                            

                        }
                    }
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }
            }

            if (t == "manager")
            {
              
                try
                {


                    int i = 0;
                    string response = DoGetHttp(SuggsIP, "", 10000);
                    if (response != "")
                    {
                      
                        var des = JsonConvert.DeserializeObject<List<Action>>(response);

                        int s = (int)Application.Current.Properties["pos"];


                        foreach (var obj in des)
                        {
                            if (obj.employee_from == (int)Application.Current.Properties["pos"])
                            {

                                if (obj.pointslog == null && obj.reject == null)
                                {

                                    p = obj.action.Split(' ');

                                    Employees.Add(new Reward(i, obj.employee, obj.action, obj.date, obj.employeenum, obj.action_pos, obj.record, obj.points, obj.employee_from, ""));
                                
                                    i++;
                                }
                            }

                        }
                    }

                  
                    else
                    {
                         Application.Current.MainPage.DisplayAlert("ALERT", "You have not suggested any rewards yet. Click Add to do that!", "OK");
               
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }
               
            }
            if (t == "admin")
            {
              
                try
                {

                   
                    int i = 0;
                    string response = DoGetHttp(SuggsIP, "", 10000);
                    if (response != "")
                    {
                        
                        var des = JsonConvert.DeserializeObject<List<Action>>(response);

                        foreach (var obj in des)
                        {


                            if (obj.pointslog == null && obj.reject == null)
                            {
                                Employees.Add(new Reward(i, obj.employee, obj.action, obj.date, obj.employeenum, obj.action_pos, obj.record, obj.points, obj.employee_from, obj.type));
                                i++;
                            }

                        }
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("ALERT", "No suggestions have been sent for approval yet.", "OK");
                      
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }
            }
           
        }



        public async Task<ObservableCollection<Reward>> GetEmployeesAsync(string query, string type)
        {
            //Thread.Sleep(2000);

            if (query != string.Empty)
            {
                Employees.Clear();
                InitializeEmployeeService(type);
                List<Reward> llstEmployees = Employees.Where(emp => (emp.Designation.ToLower().Contains(query.ToLower())
                                                                    || emp.EmployeeName.ToLower().Contains(query.ToLower()))).ToList();
               
                Employees.Clear();
                foreach (Reward employee in llstEmployees)
                {
                    Employees.Add(employee);
                    
                }

            }
            else
            {
                Employees.Clear();
                InitializeEmployeeService(type);
                List<Reward> llstEmployees = Employees.Where(emp => (emp.Designation.ToLower().Contains(query.ToLower())
                                                                    || emp.EmployeeName.ToLower().Contains(query.ToLower()))).ToList();
               
                Employees.Clear();
                foreach (Reward employee in llstEmployees)
                {
                    
                    Employees.Add(employee);

                }

                //InitializeEmployeeService(type);
            }
            return await Task.FromResult(Employees);
        }
        
        public async Task<bool> AddEmployeeAsync(Reward employee)
        {

            Employees.Add(employee);
            return await Task.FromResult(true);
        }
        
        public async Task<bool> UpdateEmployeeAsync(Reward employee)
        {
            Reward employeeToEdit = Employees.Where(emp => emp.EmployeeId == employee.EmployeeId).FirstOrDefault();

            int newIdex = Employees.IndexOf(employeeToEdit);
            Employees.Remove(employeeToEdit);

            Employees.Add(employee);
            int oldIndex = Employees.IndexOf(employee);

            Employees.Move(oldIndex, newIdex);

            return await Task.FromResult(true);
        }

        public string DoGetHttp(string sUri, string sHeaders, int nTimeout)
        {
            Hashtable ht = new Hashtable();

            if (!string.IsNullOrEmpty(sHeaders))
            {
                string[] headers = sHeaders.Split('\n');
                foreach (string header in headers)
                {
                    string[] parts = header.Split('\t');

                    ht.Add(parts[0], parts[1]);
                }
            }

            string post_guid = new Guid().ToString();


            ht.Add("User", "root");
            ht.Add("Session-Token", post_guid);


            WebResponse response = (WebResponse)_GetHttp(sUri, ht, nTimeout);
            if (response == null)
                return "";

            try
            {
                Stream s = response.GetResponseStream();
                StreamReader reader = new StreamReader(s);

                string tmp = reader.ReadToEnd();
                response.Close();
                return tmp;
            }
            catch (Exception e)
            {

            }

            return "";

        }

        static public Object _GetHttp(string sUri, Hashtable ht, int nTimeout)
        {
            Uri uri = new Uri(sUri);

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                HttpWebRequest request = null;
                try
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Method = WebRequestMethods.Http.Get;
                    request.Timeout = nTimeout;

                    if (ht != null)
                        foreach (DictionaryEntry pair in ht)
                            request.Headers.Add((string)pair.Key, (string)pair.Value);

                    return request.GetResponse();


                }
                catch (Exception e)
                {
                    string message = "Exception on uri: " + sUri + ", Error: " + e.Message + " Timeout: " + nTimeout;
                    Console.WriteLine(message);

                    return null;
                }
            }
            else


                return null;

        }
    }


}
