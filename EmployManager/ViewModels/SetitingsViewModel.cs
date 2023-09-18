using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using EmployManager.Views;

namespace EmployManager.ViewModels
{
	public partial class SetitingsViewModel : BaseViewModel
	{
		[ObservableProperty]
		bool isLightTeme, isDarckTeme, isSystemTeme;

        public string userTheme
        {
            get => Preferences.Get("CastTheme", "2");

            set
            {
                Preferences.Set("CastTheme", value);
                OnPropertyChanged(nameof(userTheme));
            }
        }

        public SetitingsViewModel()
		{
            TemeChenges(userTheme);

        }

		[RelayCommand]
		public async void Back()
		{
			await AppShell.Current.GoToAsync($"..");
		}

        [RelayCommand]
        public async void LogOut()
        {
            CurrentLogin = string.Empty;
            Token = string.Empty;
            DateLogin = DateTime.MinValue;
            await AppShell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

		[RelayCommand]
		public async void TemeChenges(string parm)
		{
			IsLightTeme = parm == "0";
			IsDarckTeme = parm == "1";
			IsSystemTeme = parm == "2";
            userTheme = parm;
            if (IsLightTeme)
			{
                Application.Current.UserAppTheme = AppTheme.Light;
            }
			else if(IsDarckTeme) 
			{
                Application.Current.UserAppTheme = AppTheme.Dark;

            }
			else
			{
                Application.Current.UserAppTheme = AppTheme.Unspecified;

            }
        }
    }
}

