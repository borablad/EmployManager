using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using EmployManager.Views;
using EmployManager.Services;

using Realms;
using EmployManager.Models;
using System.Security.Cryptography;
using System.Text;

namespace EmployManager.ViewModels
{
	public partial class LoginViewModel:BaseViewModel
	{
		[ObservableProperty]
		string login,password;

    
        private Realm realm;

        public LoginViewModel()
		{
            RealmInit();

			GetUserData();
			AutoLogin();

           // UpdateCircles();

        }


        public async Task RealmInit()
        {
            try
            {
                await RealmService.LoginAsync();
                realm = RealmService.GetMainThreadRealm();
            }
            catch (Exception ex)
            {
                var e = ex;
                //todo
            }
            await Task.CompletedTask;
        }

	
		[RelayCommand]
		public async Task DoLogin()
		{

            if (realm is null)
                realm = RealmService.GetMainThreadRealm();
           
            if (IsUserDataEmpty())
            {
                await DialogService.ShowError("Заполните все обязательные поля");
                return;
            }
            if (realm is null)
            {
                await DialogService.ShowError("Не возможность входа, обратитесь к администратору");
                return;
            }

            try
            {
                var membersCount=realm.All<Member>().Count();
                if (membersCount < 1)
                    await CreateNewUser();

                var member = realm.All<Member>().Where(x=>x.Username==Login).FirstOrDefault();
                if (member is null)
                    throw new Exception("login");
                if (member.Password != CreateHashPassword(Password))
                    throw new Exception("password");

                CurrentLogin = member.Username;
                SaveUserData();
                await AppShell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch(Exception ex)
            {
                if(ex.Message.Contains("login"))
                    await DialogService.ShowError("Не правильный логин , повторите попытку");
                else if (ex.Message.Contains("login"))
                    await DialogService.ShowError("Не правильный  пароль, повторите попытку");
                else
                    await DialogService.ShowError("Введутся технические работы, повторите попытку позже");
                return;
            }


            /*  Token = await Rest.Login(Login, Password);
              DateLogin = DateTime.Now.AddSeconds(TokenAliveSecond);
              SaveUserData();*/

         
          
        }

	


        private async void AutoLogin()
        {
			if ( IsUserDataEmpty() && IsNoEmpty(CurrentLogin))
                return;
			
            await DoLogin();
        }


        private void SaveUserData()
		{
			Preferences.Set(nameof(Login), Login);
			Preferences.Set(nameof(Password), Password);
		}

		private void GetUserData()
		{
			Login = Preferences.Get(nameof(Login), "");
			Password = Preferences.Get(nameof(Password), "");
		}

		private bool IsUserDataEmpty()=>
			string.IsNullOrWhiteSpace(Login) 
			&& string.IsNullOrWhiteSpace(Password) ? true : false;



        private async Task CreateNewUser()
        {
            if (realm is null)
                realm = RealmService.GetMainThreadRealm();




            var adminUser = new Member { Id = Guid.NewGuid().ToString(), Role = Enums.MembersRole.Admin, Username = Login, Password = CreateHashPassword(Password) };
            var firstDepartament = new Departanent {OrganizationId=Guid.NewGuid().ToString(), Id = Guid.NewGuid().ToString() ,CreaterId = adminUser.Id, Title = "default", Description = "defalut" };
            adminUser.DepartamentId = firstDepartament.Id;
            await realm.WriteAsync(() =>
            {
                realm.Add(adminUser);
                realm.Add(firstDepartament);
            });
       
           
            await Task.CompletedTask;
        }


        private string CreateHashPassword(string password)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }

        }



    }
}

