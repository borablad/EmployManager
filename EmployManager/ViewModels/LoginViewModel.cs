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
		string login,password;



        public LoginViewModel()
		{
		

			SetUserData();
			AutoLogin();

           // UpdateCircles();

        }

	
		[RelayCommand]
		public async Task DoLogin()
		{

            if (!IsNoEmpty(Login) || !IsNoEmpty(Password))
            {
                await DialogService.ShowError("Заполните все обязательные поля");
                return;
            }
          /*  Token = await Rest.Login(Login, Password);
            DateLogin = DateTime.Now.AddSeconds(TokenAliveSecond);
            SaveUserData();*/
            await AppShell.Current.GoToAsync($"//{nameof(MainPage)}");
          
        }


	

        private async void AutoLogin()
        {
			if (!IsTokenAlive() || IsUserDataEmpty())
			{
                if (!IsNoEmpty(Login) || !IsNoEmpty(Password))
					return;
                
                await DoLogin();
				return;
			}
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

