
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployManager.Helpers;
using EmployManager.Models;
using EmployManager.Services;
using EmployManager.Views;
using Microsoft.Maui.Controls.PlatformConfiguration;
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
         IQueryable<Organization> organizations;

		[ObservableProperty]
		private  Member currentUser;


        [ObservableProperty]
        string searchText;

        public static string MemberId { get => Preferences.Get(nameof(MemberId), ""); set => Preferences.Set(nameof(MemberId), value); }

    

        [ObservableProperty]
        private bool sortLowPrice, sortHiPrice;

        [ObservableProperty]
        Departanent currentDepartament;



        public ObservableCollection<Organization> TestOrganizations { get; set; }   =new ObservableCollection<Organization>();
        public ObservableCollection<Departanent> TestDep{ get; set; }   =new ObservableCollection<Departanent>();

		private Realm realm;
        private string organizationID;
		public MainPageViewModel()
		{
			

        }


		internal async void OnAppering()
		{
            SortHiPrice = Preferences.Get($"{nameof(SortHiPrice)}{CurrentDepartamentId}", false);
            realm =  RealmService.GetMainThreadRealm();
            await GetAllOrganizations();//после текущего юзера

            /* 
             CurrentUser = realm.All<Member>().Where(x => x.Username == CurrentLogin).FirstOrDefault();
              await GetAllOrganizations();
            await f();
             CurrentDepartament = GetCount() > 0 ? ReturnFirstDepartament():null;
             CurrentDepartamentId=CurrentDepartament?.Id;
             LoadAllMembers();
 */

            loadDefaultOrganizations();
            LoadDefaultDep();
        }

        private void loadDefaultOrganizations()
        {
          

            // Генерируем и добавляем 5 рандомных объектов Organization в список
            for (int i = 0; i < 5; i++)
            {
                Organization org = new Organization
                {
                    Title = "Organization " + (i + 1),
                    Description = "Description for Organization " + (i + 1),
                    PhotoUrl = "https://example.com/photo" + (i + 1) + ".jpg"
                };

                TestOrganizations.Add(org);
            }
        }
        private void LoadDefaultDep()
        {

            for (int i = 0; i < 5; i++)
            {
             
                Departanent department = new Departanent
                {
                    CreaterId = "CreatorId " + (i + 1),
                    OrganizationId = "OrganizationId " + (i + 1),
                    Title = "Department " + (i + 1),
                    Description = "Description for Department " + (i + 1),
                    PhotoUrl = "https://example.com/photo" + (i + 1) + ".jpg",
                

                };

                TestDep.Add(department);
            }

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
        public async Task SelectOrganization(Organization organization)
        {

            organizationID = organization.Id;
            CurrentOrganizationId= organization.Id;
            GetAllDepartaments();
            LoadAllMembers();
            await Task.CompletedTask;
        }


		[RelayCommand]
		public async void SelectDepartament( Departanent departanent)
		{
            organizationID=string.Empty;

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
        private async Task ImportExcel()
        {


            var tempMembers = GenerateRandomUsers(6);
        
            var file_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "out.xlsx");

            var result = ExcelExportHelper.ImportMembersToExcel(members: tempMembers, filePath: file_path);
           

            if (result)
                await DialogService.ShowError("успех");
            await Task.CompletedTask;
        }

        public static List<Member> GenerateRandomUsers(int count)
        {
            List<Member> randomUsers = new List<Member>();
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                Member user = new Member
                {
                    Username = $"User{i + 1}",
                    Password = "123",
                    Role = GenerateRandomRole(),
                    Salary = random.NextDouble() * 10000,
                    FirstName = $"FirstName{i + 1}",
                    LastName = $"LastName{i + 1}",
                     // Здесь вы можете добавить случайную генерацию контактов
                    DepartamentId = $"Department{i + 1}",
                    RoleName = $"Role{i + 1}",
                    OrganizationId = $"Organization{i + 1}",
                    MiddleName = $"MiddleName{i + 1}",
                    PhotoUrl = $"https://example.com/user{i + 1}.jpg"
                };

                randomUsers.Add(user);
            }

            return randomUsers;
        }

 

        public static MembersRole GenerateRandomRole()
        {
            // Генерация случайной роли из перечисления MembersRole
            Array values = Enum.GetValues(typeof(MembersRole));
            Random random = new Random();
            return (MembersRole)values.GetValue(random.Next(values.Length));
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

        /// <returns></returns>
        private async Task GetAllDepartaments()
        {
            Departanents = realm.All<Departanent>().Where(x => x.OrganizationId ==CurrentOrganizationId);
        }
        private async Task GetAllOrganizations()
        {
            if (CurrentUser is null)
                return;
            if(CurrentUser.Role is not MembersRole.Admin) {
                var deps = realm.All<Departanent>().Where(x => x.Members.Any(y => y.Id == CurrentUser.Id));
                var organizationIds = deps.Select(y => y.OrganizationId).ToList();
                Organizations = realm.All<Organization>().Where(x => organizationIds.Contains(x.Id));
                return;
            }
               
            Organizations = realm.All<Organization>();
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
                case SortMember.NameAbc:
                    _sort = "SORT(last_name ASC)";
                    break;
                case SortMember.NameZxy:
                    _sort = "SORT(last_name DESC)";
                    break;
                
            }

            if (!IsNoEmpty(SearchText) && IsNoEmpty(organizationID))
            {
                filter = $"organization_id == '{CurrentOrganizationId}' AND user_name != '{CurrentLogin}'";
            }
            else if (IsNoEmpty(organizationID))
            {
                filter = $"organization_id == {CurrentOrganizationId} AND user_name != '{CurrentLogin}' AND (user_name CONTAINS[c] '{SearchText}' OR first_name CONTAINS[c] '{SearchText}' OR last_name CONTAINS[c] '{SearchText}' OR ANY(Contacts, title CONTAINS[c] '{SearchText}' OR body CONTAINS[c] '{SearchText}'))";

            }

            else if (!IsNoEmpty(SearchText))
            {
                filter = $"departament_id == '{CurrentDepartamentId}' AND user_name != '{CurrentLogin}'";
            }
            else
            {
               filter= $"departament_id == {CurrentDepartamentId} AND user_name != '{CurrentLogin}' AND (user_name CONTAINS[c] '{SearchText}' OR first_name CONTAINS[c] '{SearchText}' OR last_name CONTAINS[c] '{SearchText}' OR ANY(Contacts, title CONTAINS[c] '{SearchText}' OR body CONTAINS[c] '{SearchText}'))";

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

