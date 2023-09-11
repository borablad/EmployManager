
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployManager.Models;
using EmployManager.Services;
using EmployManager.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using static EmployManager.Models.Enums;

namespace EmployManager.ViewModels
{
	public partial   class MainPageViewModel : BaseViewModel
	{
        [ObservableProperty]
        IQueryable departanents;

        [ObservableProperty]
         IQueryable<Member> members;

		[ObservableProperty]
		private  Member currentUser;


        [ObservableProperty]
        string searchText;

        public static string MemberId { get => Preferences.Get(nameof(MemberId), ""); set => Preferences.Set(nameof(MemberId), value); }

    

        [ObservableProperty]
        private bool sortLowPrice, sortHiPrice;

        [ObservableProperty]
        Departanent currentDepartament;

		private Realm realm;

		public MainPageViewModel()
		{
			

        }


		internal async void OnAppering()
		{
            SortHiPrice = Preferences.Get($"{nameof(SortHiPrice)}{CurrentDepartamentId}", false);
            realm =  RealmService.GetMainThreadRealm();
            await GetAllDepartaments();
            CurrentUser = realm.All<Member>().Where(x => x.Username == CurrentLogin).FirstOrDefault();
			CurrentDepartament = GetCount() > 0 ? ReturnFirstDepartament():null;
			CurrentDepartamentId=CurrentDepartament?.Id;
			LoadAllMembers();
        }


        private int GetCount()
        {
            return realm.All<Departanent>().Count();
        }

        private Departanent ReturnFirstDepartament()
        {
            return realm.All<Departanent>().ToList()[0];    
        }
	

		[RelayCommand]
		public async void ChangeDepartament( Departanent departanent)
		{
			CurrentDepartament = departanent;
			CurrentDepartamentId = departanent.Id;
            LoadAllMembers();  
			//Members = departanent.Members.ToList();
			//departanent.Members.ToList().ForEach(x=>Members.Add(x));
		}

		[RelayCommand]
		public async void GoToSettings()
		{
			await AppShell.Current.GoToAsync($"{nameof(SetingsPage)}");
		}

        [RelayCommand]
        public async void GoToEmployDetail(Member member)
        {
			
			MemberId = member.Id;
         
            await AppShell.Current.GoToAsync($"{nameof(EmployDetailPage)}");


        }


        [RelayCommand]
        public async void LogOut()
        {
			Token = string.Empty;
			CurrentLogin = string.Empty;
            DateLogin = DateTime.MinValue;
            await AppShell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

         public async void OnSearchtextChanged(string value)
        {
            await LoadAllMembers();
			/*if (string.IsNullOrWhiteSpace(value))
			{
				LoadAllMembers();
                return;
            }


            Members = realm.All<Member>().Where(x => x.DepartamentId==CurrentDepartamentId &&  x.Username != CurrentLogin && x.Username.Contains(value) ||x.FirstName.Contains(value)||x.LastName.Contains(value) || x.Contacts.Where(y => y.Title.Contains(value) || y.Body.Contains(value)).ToList().Count > 0);*/
            //Members.Clear();
            //CurrentDepartament.Members.Where(x => x.Username.Contains(value)|| x.Contacts.Where(y => y.Title.Contains(value) || y.Body.Contains(value)).ToList().Count > 0).ToList().ForEach(x => Members.Add(x));



        }


        [RelayCommand]
        private async Task CreateMember()
        {
            var UpdateMember = new Member();
            var Login = GenerateRandomString(22);
            var Password = "123";
            if (!IsNoEmpty(Login) || !IsNoEmpty(Password))
            {
                await DialogService.ShowAlertAsync("Ошибка", "Заполните все обязательные поля!");
                return;
            }
            var Contact = new Models.Contacts { Title = "123", Body = "test" };
            UpdateMember.FirstName = "!";
            UpdateMember.LastName = "123";
            UpdateMember.Username = Login;
            UpdateMember.Password = CreateHashPassword(Password);
            UpdateMember.Contacts.Add(Contact);
     
            UpdateMember.Role = Enums.MembersRole.Manager;
            UpdateMember.DepartamentId = CurrentDepartamentId;
            var currentDep = realm.All<Departanent>().FirstOrDefault(x => x.Id == CurrentDepartamentId);
           
            await realm.WriteAsync(() =>
            {
                currentDep.Members.Add(UpdateMember);
             /*   realm.Add(Contact);
               realm.Add(UpdateMember);
*/
               
              
                realm.Add(currentDep);
            });

         /*   var i = CurrentDepartament.Members;
            currentDep.Members.ToList().ForEach(x=>Members.Add(x));*/
        }
        private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateRandomString(int length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random random= new Random();
            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(0, characters.Length);
                char randomChar = characters[randomIndex];
                stringBuilder.Append(randomChar);
            }

            return stringBuilder.ToString();
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

        private async Task GetAllDepartaments()
            {
                 Departanents= realm.All<Departanent>();
        }




        [RelayCommand]
        private async Task SortChanged(SortMember sort)
        {
            switch (sort)
            {
                case SortMember.MemberSalaryMin:
               
                    SortHiPrice = false;
                    SortLowPrice = true;
                    break;
                case SortMember.MemberSalaryMax:
                    SortHiPrice = true;
                    SortLowPrice = false;
                    break;
               
            }

         
            Preferences.Set($"{nameof(SortLowPrice)}{CurrentDepartamentId}", SortLowPrice);
           // Preferences.Set($"{nameof(SortHiPrice)}{CurrentShopID}", SortHiPrice);
            await LoadAllMembers();
        }

        private async Task LoadAllMembers()
        {
           /// var members = realm.All<Member>().Filter(rqlQuery).ToList();
            var filter = "";
            var _sort = "";



            switch (sort)
            {
                case SortMember.MemberSalaryMin:

                    _sort = "SORT(salary ASC)";
                    break;
                case SortMember.MemberSalaryMax:

                    _sort = "SORT(salary DESC)";
                    break;
                
            }

            if (!IsNoEmpty(SearchText))
            {
                filter = $"DepartamentId == '{CurrentDepartamentId}' AND Username != '{CurrentLogin}'";
            }
            else
            {
               filter= $"DepartamentId == {CurrentDepartamentId} AND Username != '{CurrentLogin}' AND (Username CONTAINS[c] '{SearchText}' OR FirstName CONTAINS[c] '{SearchText}' OR LastName CONTAINS[c] '{SearchText}' OR ANY(Contacts, Title CONTAINS[c] '{SearchText}' OR Body CONTAINS[c] '{SearchText}'))";

            }


            Members = realm.All<Member>().Filter($"{filter} {_sort}");
            await Task.CompletedTask;
        }


        private SortMember sort
        {
            get
            {
                if (SortHiPrice)
                    return SortMember.MemberSalaryMax;
               

                return SortMember.MemberSalaryMin;
            }
        }
    }
}

