
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
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
    public partial class MainPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        IQueryable departanents;


        private IQueryable<Member> members;

        public IQueryable<Member> Members
        {
            get { return members; }
            set
            {
                members = value; OnPropertyChanged(nameof(Members)); UpdateCollectinSpan();
               
            }
        }

        [ObservableProperty]
        IQueryable<Organization> organizations;

        [ObservableProperty]
        private Member currentUser;



        [ObservableProperty]
        int collectinSpan = 1;

        private string organizationIdTemp;

        public string OrganizationIdTemp
        {
            get
            {
                return organizationIdTemp;
            }
            set
            {
                organizationIdTemp = value;
                OnPropertyChanged(nameof(organizationIdTemp));
                OnPropertyChanged(nameof(IsOrgSelect));
                OnPropertyChanged(nameof(IsDepsSelect));
            }
        }

        public bool IsOrgSelect { get { return !IsNoEmpty(OrganizationIdTemp); } }
        public bool IsDepsSelect { get { return IsNoEmpty(OrganizationIdTemp); } }

        [ObservableProperty]
        string searchText, curretnOrgTitle;



        [ObservableProperty]
        private bool sortLowPrice, sortHiPrice;

        [ObservableProperty]
        Departanent currentDepartament;



        public ObservableCollection<Organization> TestOrganizations { get; set; } = new ObservableCollection<Organization>();
        public ObservableCollection<Departanent> TestDep { get; set; } = new ObservableCollection<Departanent>();

        private Realm realm;
        private IDisposable subscribeMembers;

        public MainPageViewModel()
        {


        }

        public void UpdateCollectinSpan()
        {
            if (Members.Count() >= 4)
                CollectinSpan = 4;
            else CollectinSpan = Members.Count();
        }

        internal async void OnAppering()
        {
            SortHiPrice = Preferences.Get($"{nameof(SortHiPrice)}{CurrentDepartamentId}", false);
            realm = RealmService.GetMainThreadRealm();

            /*   IsOrgSelekt = true;
               IsntDepsSelect = false;*/


            CurrentUser = realm.All<Member>().Where(x => x.Username == CurrentLogin).FirstOrDefault();
            await GetAllOrganizations();

            if (Organizations is null)
                return;
            if (Organizations.Count() <= 0)
                return;

            /* CurrentDepartament = GetCount() > 0 ? ReturnFirstDepartament():null;
             CurrentDepartamentId=CurrentDepartament?.Id;*/
            LoadAllMembers();

            subscribeMembers = realm.All<Member>().Where(x => x.OrganizationId == CurrentOrganizationId)
                  .SubscribeForNotifications((sender, changes) =>
                  {
                      if (changes == null)
                          return;
                      foreach (var i in changes.NewModifiedIndices)
                      {
                          LoadAllMembers();
                      }

                      foreach (var i in changes.InsertedIndices)
                      {
                          LoadAllMembers();
                      }
                      foreach (var i in changes.DeletedIndices)
                      {
                          LoadAllMembers();
                      }
                  });


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
            if (organization is null)
                return;

            CurrentOrganizationId = IsNoEmpty(organization.Id) ? organization.Id : CurrentOrganizationId;
            OrganizationIdTemp = CurrentOrganizationId;
            CurrentDepartamentId = "";
            Title = "";

            /*IsOrgSelekt = true;
            IsntDepsSelect = false;*/
            CurretnOrgTitle = organization.Title;
            await GetAllDepartaments();
            await LoadAllMembers();
            await Task.CompletedTask;
        }


        [RelayCommand]
        public async void SelectDepartament(Departanent departanent)
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
            if (member is null)
                return;



            await AppShell.Current.GoToAsync($"{nameof(EmployDetailPage)}?member_id={member.Id}");


        }


        [RelayCommand]
        public async void AddMember()
        {
            



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

            FileResult file;
            try
            {
                var options = new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" } },
                    { DevicePlatform.iOS, new string[] { "com.microsoft.excel.xlsx", "com.microsoft.excel.xls" } },
                    { DevicePlatform.WinUI, new string[] { ".xlsx", ".xls" } },
                     { DevicePlatform.macOS, new string[] { "com.microsoft.excel.xlsx", "com.microsoft.excel.xls" } },

                })
                };

                file = await FilePicker.PickAsync(options);
                if (file is null)
                    return;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError("Не удалось открыть файл");
                return;

            }
            if (file is null)
                return;

            var file_path = file.FullPath;

            var result = ExcelExportHelper.ExportMembersFromExcel(filePath: file_path);

            if (result is null)
            {
                await DialogService.ShowError("Не получилось пользователей. Проверьте правильность заполняемых данных");
                return;
            }
            if (result.Count < 0)
            {
                await DialogService.ShowError("Не получилось пользователей. Проверьте правильность заполняемых данных");
                return;
            }

            await realm.WriteAsync(() =>
            {
                realm.Add(result);
            });

            await LoadAllMembers();


            await Task.CompletedTask;

        }

        [RelayCommand]
        private async Task ExportExcel()
        {

            if (Members is null)
                return;

            // var tempMembers = GenerateRandomUsers(6);

            var file_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"out.xlsx");

            var result = ExcelExportHelper.ImportMembersToExcel(members: Members.ToList(), filePath: file_path);


            if (result)
                await AppShell.Current.DisplayAlert("Документ создан","В ваших документа успешно создан экспортированый фаил","ок");
                //await DialogService.ShowError("успех");
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




        /// <returns></returns>
        private async Task GetAllDepartaments()
        {
            Departanents = realm.All<Departanent>().Where(x => x.OrganizationId == CurrentOrganizationId);
        }
        private async Task GetAllOrganizations()
        {
            if (CurrentUser is null)
                CurrentUser = realm.All<Member>().FirstOrDefault(x => x.Username == CurrentLogin);
            if (CurrentUser.Role is not MembersRole.Admin)
            {
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
            try
            {
                if (!IsNoEmpty(SearchText) && IsNoEmpty(CurrentOrganizationId) && !IsNoEmpty(CurrentDepartamentId))
                {
                    filter = $"organization_id == '{CurrentOrganizationId}' AND user_name != '{CurrentLogin}' ";
                }
                else if (IsNoEmpty(CurrentOrganizationId) && !IsNoEmpty(CurrentDepartamentId))
                {
                    filter = $"organization_id == '{CurrentOrganizationId}' AND user_name != '{CurrentLogin}' AND (user_name CONTAINS[c] '{SearchText}' OR first_name CONTAINS[c] '{SearchText}' OR last_name CONTAINS[c] '{SearchText}' OR ANY(Contacts, title CONTAINS[c] '{SearchText}' OR body CONTAINS[c] '{SearchText}'))";

                }

                else if (!IsNoEmpty(SearchText))
                {
                    filter = $"departament_id == '{CurrentDepartamentId}' AND organization_id =='{CurrentOrganizationId}' AND user_name != '{CurrentLogin}'";
                }
                else
                {
                    filter = $"departament_id == '{CurrentDepartamentId}' AND  organization_id == '{CurrentOrganizationId}'  AND user_name != '{CurrentLogin}' AND (user_name CONTAINS[c] '{SearchText}' OR first_name CONTAINS[c] '{SearchText}' OR last_name CONTAINS[c] '{SearchText}' OR ANY(Contacts, title CONTAINS[c] '{SearchText}' OR body CONTAINS[c] '{SearchText}'))";

                }

                //var i = realm.All<Member>().Filter($"{filter} {_sort}");
                Members = realm.All<Member>().Filter($"{filter} {_sort}");
                //   Members = realm.All<Member>().Where(x => x.OrganizationId == CurrentOrganizationId);
            }
            catch (Exception ex)
            {

            }
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






        [RelayCommand]
        public void BackToOrg()
        {

            /* IsOrgSelekt = false;
             IsntDepsSelect = true;*/
            OrganizationIdTemp = string.Empty;
            CurrentOrganizationId = string.Empty;
            CurrentDepartamentId = string.Empty;
            curretnOrgTitle = string.Empty;
            //GetAllOrganizations();
        }

        [RelayCommand]
        public async void AddOrgOrDep()
        {
            if (IsDepsSelect)
                await AppShell.Current.GoToAsync($"{nameof(DepPage)}");
            else
                await AppShell.Current.GoToAsync($"{nameof(OrgPage)}");

        }

    }

}

    