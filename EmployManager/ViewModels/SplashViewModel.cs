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

       
        bool IsLightTeme, IsDarckTeme, IsSystemTeme;

        public string userTheme
        {
            get => Preferences.Get("CastTheme", "2");

            set
            {
                Preferences.Set("CastTheme", value);
                OnPropertyChanged(nameof(userTheme));
            }
        }

      

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
            else if (IsDarckTeme)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;

            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Unspecified;

            }
        }

        public SplashViewModel()
		{
            TemeChenges(userTheme);

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

