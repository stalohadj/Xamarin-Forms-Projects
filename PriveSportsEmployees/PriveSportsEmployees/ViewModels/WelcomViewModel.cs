using System;

using Xamarin.Forms;

namespace PriveSportsEmployees.ViewModels
{
	public class WelcomViewModel : ContentPage
	{
		public string name { get; set; }
		public string occupation { get; set; }

		public WelcomViewModel ()
		{
            name = (string)Application.Current.Properties["name"];
            switch ((string)Application.Current.Properties["level"])
            {

                case "ADMINISTRATOR":
                    occupation = "ADMINISTRATOR";
                    break;
                case "STORE MANAGER":
                    occupation = "STORE MANAGER";
                    break;
                default:
                    occupation = "EMPLOYEE";
                    break;
            }

        }
	}
}


