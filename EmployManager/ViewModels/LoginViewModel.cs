using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using EmployManager.Views;
using EmployManager.Services;

namespace EmployManager.ViewModels
{
	public partial class LoginViewModel:BaseViewModel
	{
		[ObservableProperty]
		string code = "", currentDivaseId,login,password;

        [ObservableProperty]
        bool circle1, circle2, circle3, circle4,authorizationStep;

        public LoginViewModel()
		{
			CurrentDivaseId = Preferences.Get(nameof(CurrentDivaseId),"");
			authorizationStep = string.IsNullOrEmpty(CurrentDivaseId);

			SetUserData();
			AutoLogin();

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
                await AppShell.Current.GoToAsync($"//{nameof(MainPage)}");

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


		private async void DoLogin()
		{
			if (IsNoEmpty(Login) || IsNoEmpty(Password))
			{
                await DialogService.ShowError("Заполните все обязательные поля");
				return;
			}
			 Token = await Rest.Login(Login, Password);
			 DateLogin= DateTime.Now.AddSeconds(TokenAliveSecond);
			 SaveUserData();
             await AppShell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

        private async void AutoLogin()
        {
            if (!IsTokenAlive() || IsUserDataEmpty())
                return;
            await AppShell.Current.GoToAsync($"//{nameof(MainPage)}");
        }


        private void SaveUserData()
		{
			Preferences.Set(nameof(Login), Login);
			Preferences.Set(nameof(Password), Password);
		}

		private void SetUserData()
		{
			Login = Preferences.Get(nameof(Login), "");
			Password = Preferences.Get(nameof(Password), "");
		}

		private bool IsUserDataEmpty()=>
			string.IsNullOrWhiteSpace(Preferences.Get(nameof(Login), "")) 
			|| string.IsNullOrWhiteSpace(Preferences.Get(nameof(Password), "")) ? true : false;
		

		

		
    }
}

