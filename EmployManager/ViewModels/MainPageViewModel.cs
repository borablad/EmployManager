//using ARKit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployManager.Models;
using EmployManager.Services;
using EmployManager.Views;
using Newtonsoft.Json;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace EmployManager.ViewModels
{
	public partial   class MainPageViewModel : BaseViewModel
	{
		public ObservableCollection<Departanent> Departanents { get; set; }=new ObservableCollection<Departanent>();
		public ObservableCollection<Member> Members { get; set; }=new ObservableCollection<Member>();

		[ObservableProperty]
		private  Member currentUser;

        public static string MemberId { get => Preferences.Get(nameof(MemberId), ""); set => Preferences.Set(nameof(MemberId), value); }

      //  public Organization Organization { get; set; }



		public Departanent CurrentDepartament { get; set; }

		private Realm realm;

		public MainPageViewModel()
		{
			

        }


		internal async void OnAppering()
		{
			realm =  RealmService.GetMainThreadRealm();
            (await GetAllDepartaments()).ForEach(x=>Departanents.Add(x));
            CurrentUser = realm.All<Member>().Where(x => x.Username == CurrentLogin).FirstOrDefault();
			CurrentDepartament= Departanents.Count > 0 ? Departanents[0]:null;
			CurrentDepartamentId=CurrentDepartament?.Id;
			LoadAllMembers();
        }

	

		[RelayCommand]
		public async void ChangeDepartament( Departanent departanent)
		{
			CurrentDepartament = departanent;
			CurrentDepartamentId = departanent.Id;
			//Members = departanent.Members.ToList();
			departanent.Members.ToList().ForEach(x=>Members.Add(x));
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

         public void OnSearchTextChanged(string value)
        {
			if (string.IsNullOrWhiteSpace(value))
			{
				LoadAllMembers();
                return;
            }
			Members.Clear();
			CurrentDepartament.Members.Where(x => x.Username.Contains(value)|| x.Contacts.Where(y => y.Title.Contains(value) || y.Body.Contains(value)).ToList().Count > 0).ToList().ForEach(x => Members.Add(x));
	


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
            UpdateMember.FirstName = "!";
            UpdateMember.LastName = "123";
            UpdateMember.Username = Login;
            UpdateMember.Password = CreateHashPassword(Password);
     
            UpdateMember.Role = Enums.MembersRole.Manager;
            UpdateMember.DepartamentId = CurrentDepartamentId;
            var currentDep = realm.All<Departanent>().FirstOrDefault(x => x.Id == CurrentDepartamentId);
            await realm.WriteAsync(() =>
            {
                realm.Add(UpdateMember);

               
                currentDep.Members.Add(UpdateMember);
                realm.Add(currentDep);
            });

            var i = CurrentDepartament.Members;
            currentDep.Members.ToList().ForEach(x=>Members.Add(x));
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

            private async Task<List<Departanent>> GetAllDepartaments()
        {
            return realm.All<Departanent>().ToList();

        }

		private void LoadAllMembers()
		{
            CurrentDepartament?.Members.ToList().ForEach(x => Members.Add(x));
        }

        private async Task<Organization> GetOrganization()
        {
            var organization = new Organization();

            organization = await Rest.GetOrganization();



            return organization;
        }

    }
}

