using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using EmployManager.Views;

namespace EmployManager.ViewModels
{
	public partial class LoginViewModel:BaseViewModel
	{
		[ObservableProperty]
		string code = "", currentDivaseId;

        [ObservableProperty]
        bool circle1, circle2, circle3, circle4,authorizationStep;

        public LoginViewModel()
		{
			CurrentDivaseId = Preferences.Get(nameof(CurrentDivaseId),"");
			authorizationStep = string.IsNullOrEmpty(CurrentDivaseId);


            UpdateCircles();

        }

		[RelayCommand]
		public void KeyInput(string parm)
		{
			if(parm == "back" && Code.Length != 0)
			{
                Code = Code.Substring(0, Code.Length - 1);
				UpdateCircles();
                return;
			}
			if(parm == "=")
			{
				Authorizatin();
				return;
			}

			if (parm.Length != 1)
				return;
			

            if (Code.Length < 4)
			{
				//var charParm = parm;
				// write Code 
				Code += parm;
			}
			else
			{
				// write Code complit
				//string result = new string(Code.ToArray());
			}
			UpdateCircles();

            return;
		}

        public void UpdateCircles()
        {
			Circle1 = (Code.Length > 0);
			Circle2 = (Code.Length > 1);
			Circle3 = (Code.Length > 2);
			Circle4 = (Code.Length > 3);
			if (Circle4)
				Authorizatin();
        }

		[RelayCommand]
		public async Task Authorizatin()
		{
			if (AuthorizationStep)
			{
				if(string.IsNullOrEmpty(CurrentDivaseId))
                Preferences.Set(nameof(CurrentDivaseId), CurrentDivaseId.Replace(" ", ""));
				AuthorizationStep = !AuthorizationStep;
            }
			else
			{
				if(Code.Length == 4)
				{
					//Complit authorization
					await AppShell.Current.GoToAsync($"//{nameof(MainPage)}");
				}
			}
		}

    }
}

