using System;
using EmployManager.Services;
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
           
        }
        internal async void OnAppering()
        {
            await Task.Delay(200);
            try
            {
                await RealmService.LoginAsync();
            }catch (Exception ex)
            {
                var i = ex;
            }
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

