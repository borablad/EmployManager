using System;
using CommunityToolkit.Mvvm.Input;
using EmployManager.Views;

namespace EmployManager.ViewModels
{
	public partial class SetitingsViewModel : BaseViewModel
	{
		public SetitingsViewModel()
		{
		}

		[RelayCommand]
		public async void Back()
		{
			await AppShell.Current.GoToAsync($"..");
		}

        [RelayCommand]
        public async void LogOut()
        {
            Token = string.Empty;
            DateLogin = DateTime.MinValue;
            await AppShell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}

