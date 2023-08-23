using System;
using EmployManager.Views;

namespace EmployManager.ViewModels
{
	public partial class SplashViewModel : BaseViewModel
	{
        //authorization parameters
        public static string CurrentLogin { get => Preferences.Get(nameof(CurrentLogin), ""); set => Preferences.Set(nameof(CurrentLogin), value); }
        public static string CurrentDivaseId { get => Preferences.Get(nameof(CurrentDivaseId), ""); set => Preferences.Set(nameof(CurrentDivaseId), value); }

        public SplashViewModel()
		{
            ChekAuthoriztion();
        }

		public async void ChekAuthoriztion()
		{
            if (string.IsNullOrEmpty(CurrentDivaseId) || string.IsNullOrEmpty(CurrentLogin))
            {
				await AppShell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
			else
			{
                await AppShell.Current.GoToAsync($"{nameof(MainPage)}");

            }
        }
	}
}

