﻿
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
        private string isDepId;

        public SortMember SortMemb1 = SortMember.MemberSalaryMax;
        public SortMember SortMemb2 = SortMember.MemberSalaryMin;
        public SortMember SortMemb3 = SortMember.NameAbc;
        public SortMember SortMemb4 = SortMember.NameZxy;

        [ObservableProperty]
        bool isSortMemb1, isSortMemb2, isSortMemb3, isSortMemb4, isSortVisble;

        private IQueryable<Member> members;

        private string organizationIdTemp;
        private Realm realm;
        private IDisposable subscribeMembers, subscribeDeparaments;
        private string searchText;

        public string OrganizationIdTemp
        {
            get
            {
                return organizationIdTemp;
            }
            set
            {
                organizationIdTemp = value;
                OnPropertyChanged(nameof(OrganizationIdTemp));
                OnPropertyChanged(nameof(IsOrgSelect));
                OnPropertyChanged(nameof(IsDepsSelect));
                OnPropertyChanged(nameof(IsDeleteUser));
            }
        }

        public bool IsOrgSelect { get { return !IsNoEmpty(OrganizationIdTemp); } }
        public bool IsDepsSelect { get { return IsNoEmpty(OrganizationIdTemp); } }

        public bool IsDepNull { get { return IsNoEmpty(IsDepId); } }
        public string IsDepId { get { return isDepId; } set { isDepId = value; OnPropertyChanged(nameof(IsDepId)); OnPropertyChanged(nameof(IsDepNull)); } }

        
        public string SearchText { get { return searchText; } set { searchText = value; OnPropertyChanged(nameof(SearchText)); OnSearchtextChanged(); } }

        public IQueryable<Member> Members
        {
            get { return members; }
            set
            {
                members = value; OnPropertyChanged(nameof(Members)); UpdateCollectinSpan();

            }
        }

        private SortMember sort
        {
            get
            {
                if (SortHiPrice)
                    return SortMember.MemberSalaryMax;

                else if (SortLowPrice)
                    return SortMember.MemberSalaryMin;
                else if (IsSortMemb4)
                    return SortMember.NameZxy;
                return SortMember.NameAbc;
            }
        }

        [ObservableProperty]
        IQueryable departanents;

        [ObservableProperty]
        private bool memberIsNotNull, searchVisible;


        [ObservableProperty]
        IQueryable<Organization> organizations;

        [ObservableProperty]
        private Member currentUser;


        [ObservableProperty]
        int collectinSpan = 1;


        private bool isAdmin, isManager;

        public bool IsAdmin { get => isAdmin; set { isAdmin = value; OnPropertyChanged(nameof(IsAdmin)); OnPropertyChanged(nameof(IsManagerOrAdmin)); OnPropertyChanged(nameof(IsDeleteUser)); } }
        public bool IsManager { get => isManager; set { isManager = value; OnPropertyChanged(nameof(IsManager)); OnPropertyChanged(nameof(IsManagerOrAdmin)); OnPropertyChanged(nameof(IsDeleteUser)); } }

        public bool IsManagerOrAdmin { get => IsAdmin || IsManager; }

        public bool IsDeleteUser { get => IsManagerOrAdmin && IsDepNull; }

        [ObservableProperty]
         string curretnOrgTitle;

        [ObservableProperty]
        private bool sortLowPrice, sortHiPrice;

        [ObservableProperty]
        Departanent currentDepartament;
     

        public MainPageViewModel()
        {


        }

        public void UpdateCollectinSpan()
        {
            if (Members.Count() >= 3)
                CollectinSpan = 3;
            else
            {
                if (Members.Count() <1)
                    CollectinSpan = 1;
                else
                    CollectinSpan = Members.Count();
            }
        }

        internal async void OnAppering()
        {
            MemberIsNotNull = false;
            SortHiPrice = Preferences.Get($"{nameof(SortHiPrice)}{CurrentDepartamentId}", false);
            SortLowPrice = Preferences.Get($"{nameof(SortLowPrice)}{CurrentDepartamentId}", false);
            IsSortMemb4 = Preferences.Get($"{nameof(IsSortMemb4)}{CurrentDepartamentId}", false);
          
            realm = RealmService.GetMainThreadRealm();

            /*   IsOrgSelekt = true;
               IsntDepsSelect = false;*/


            CurrentUser = realm.All<Member>().Where(x => x.Username == CurrentLogin).FirstOrDefault();
            if (CurrentUser is null)
                return;
         
            IsAdmin=CurrentUser.Role == MembersRole.Admin;
            IsManager=CurrentUser.Role == MembersRole.Manager;

            await GetAllOrganizations();

            if (Organizations is null)
                return;
            if (Organizations.Count() <= 0)
                return;

         
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

            subscribeDeparaments = realm.All<Departanent>().Where(x => x.OrganizationId == CurrentOrganizationId)
                  .SubscribeForNotifications((sender, changes) =>
                  {
                      if (changes == null)
                          return;
                      foreach (var i in changes.NewModifiedIndices)
                      {
                          if(IsNoEmpty(organizationIdTemp))
                            GetAllDepartaments();
                      }

                      foreach (var i in changes.InsertedIndices)
                      {
                          if (IsNoEmpty(organizationIdTemp))
                              GetAllDepartaments();
                      }
                      foreach (var i in changes.DeletedIndices)
                      {
                          if (IsNoEmpty(organizationIdTemp))
                              GetAllDepartaments();
                      }
                  });


        }


        [RelayCommand]
        private async Task DeleteDepartament(Departanent dep)
        {
            if (dep is null)
                return;

           await realm.WriteAsync(() =>
            {
                realm.Remove(dep);

            });
        }


        [RelayCommand]
        private async Task DeleteOrganization(Organization org)
        {
            if (org is null)
                return;

            await realm.WriteAsync(() =>
            {
                realm.Remove(org);

            });
        }

        [RelayCommand]
        public async Task SelectOrganization(Organization organization)
        {
            SearchVisible = true;
            if (organization is null)
                return;

            CurrentOrganizationId = IsNoEmpty(organization.Id) ? organization.Id : CurrentOrganizationId;
             OrganizationIdTemp = CurrentOrganizationId;
            CurrentDepartamentId = "";
            CurrentDepartament = null;
             IsDepId = "";
            IsDepId = "";
            Title = "";
            CurretnOrgTitle = organization.Title;
            await GetAllDepartaments();
            await LoadAllMembers();
            await Task.CompletedTask;
        }


        [RelayCommand]
        public async void SelectDepartament(Departanent departanent)
        {
            SearchVisible=true;
            if(CurrentDepartament is not null)
                CurrentDepartament.IsSelect = false;
           // OrganizationIdTemp = "";

            departanent.IsSelect = true;

            CurrentDepartament = departanent;
            CurrentDepartamentId = departanent.Id;
            IsDepId=departanent.Id;
            LoadAllMembers();
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

            if (!IsManagerOrAdmin||(member.Role is MembersRole.Admin&& IsManager))
                return;

            await AppShell.Current.GoToAsync($"{nameof(EmployDetailPage)}?member_id={member.Id}");


        }


        [RelayCommand]
        public async void AddMember()
        {

            if (!IsManagerOrAdmin)
            {
                await DialogService.ShowError("Данная функция вам не доступна");
                return;
            }


            await AppShell.Current.GoToAsync($"{nameof(EmployDetailPage)}");


        }
        [RelayCommand]
        public async void DeleteMember(Member member)
        {

            if (member is null)
                return;

            if (!IsManagerOrAdmin || (member.Role is MembersRole.Admin && IsManager))
                return;

            if (!IsNoEmpty(CurrentOrganizationId) && CurrentDepartament is null)
                return;
            await realm.WriteAsync(() =>
            {
                member.DepartamentId = string.Empty;
                member.OrganizationId = string.Empty;
                realm.Add(member);
                CurrentDepartament.Members.Remove(member);
                realm.Add(CurrentDepartament);
            });
         

        }



        [RelayCommand]
        public async void LogOut()
        {
            Token = string.Empty;
            CurrentLogin = string.Empty;
            DateLogin = DateTime.MinValue;
            await AppShell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        public async void OnSearchtextChanged()
        {
            await LoadAllMembers();

        }

        [RelayCommand]
        private async Task ImportExcel()
        {
            if (!IsManagerOrAdmin)
            {
                await DialogService.ShowError("Данная функция вам не доступна");
                return;
            }

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
                await DialogService.ShowError("Не получилось получить пользователей. Проверьте правильность заполняемых данных");
                return;
            }
            if (result.Count < 0)
            {
                await DialogService.ShowError("Не получилось получить пользователей. Проверьте правильность заполняемых данных");
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
            if (!IsManagerOrAdmin)
            {
                await DialogService.ShowError("Данная функция вам не доступна");
                return;
            }

            if (Members is null)
                return;


            var file_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"out.xlsx");

            var result = ExcelExportHelper.ImportMembersToExcel(members: Members.ToList(), filePath: file_path);


            if (result)
                await AppShell.Current.DisplayAlert("Документ создан","В ваших документа успешно создан экспортированный файл","ок");
            await Task.CompletedTask;
        }




        /// <returns></returns>
        private async Task GetAllDepartaments()
        {
            if (CurrentUser is null)
                CurrentUser = realm.All<Member>().FirstOrDefault(x => x.Username == CurrentLogin);
            if (CurrentUser.Role is not MembersRole.Admin)
            {
                Departanents = realm.All<Departanent>().ToList().Where(x =>  x.OrganizationId == CurrentOrganizationId&&x.Members.Any(y => y.Id == CurrentUser.Id)).AsQueryable();
                return;
            }

            Departanents = realm.All<Departanent>().Where(x => x.OrganizationId == CurrentOrganizationId);
        }
        private async Task GetAllOrganizations()
        {
            if (CurrentUser is null)
                CurrentUser = realm.All<Member>().FirstOrDefault(x => x.Username == CurrentLogin);
            if (CurrentUser.Role is not MembersRole.Admin)
            {
                var deps = realm.All<Departanent>().ToList().Where(x => x.Members.Any(y => y.Id == CurrentUser.Id));
                var organizationIds = deps.ToList().Select(y => y.OrganizationId).ToList();
               Organizations= realm.All<Organization>().ToList().Where(x => organizationIds.ToList().Any(y=>y==x.Id)).ToList().AsQueryable();
                return;
            }

            Organizations = realm.All<Organization>();
        }




        [RelayCommand]
        private async Task SortChanged(string param)
        {
             var _sort = (SortMember)int.Parse(param);
            switch (_sort)
            {
                case SortMember.MemberSalaryMin:

                    SortHiPrice = false;
                    SortLowPrice = true;
                    IsSortMemb4 = false;
                    IsSortMemb3 = false;
                    break;
                case SortMember.MemberSalaryMax:
                    SortHiPrice = true;
                    SortLowPrice = false;
                    IsSortMemb4 = false;
                    IsSortMemb3 = false;
                    break;
                case SortMember.NameAbc:
                    SortHiPrice = false;
                    SortLowPrice = false;
                    IsSortMemb3 = true;
                    IsSortMemb4 = false;
                    break;
                case SortMember.NameZxy:
                    IsSortMemb3 = false;
                    IsSortMemb4 = true;
                    SortHiPrice = false;
                    SortLowPrice = false;
                    break;

            }


            Preferences.Set($"{nameof(SortLowPrice)}{CurrentDepartamentId}", SortLowPrice);
            Preferences.Set($"{nameof(IsSortMemb4)}{CurrentDepartamentId}", IsSortMemb4);
             Preferences.Set($"{nameof(SortHiPrice)}{CurrentDepartamentId}", SortHiPrice);
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
                if (!IsNoEmpty(SearchText) && IsNoEmpty(OrganizationIdTemp) && !IsNoEmpty(CurrentDepartamentId))
                {
                    filter = $"organization_id == '{CurrentOrganizationId}' AND user_name != '{CurrentLogin}' ";
                }
                else if (IsNoEmpty(CurrentOrganizationId) && !IsNoEmpty(CurrentDepartamentId) && IsNoEmpty(SearchText))
                {
                    filter = $"organization_id == '{CurrentOrganizationId}' AND user_name != '{CurrentLogin}'" +
                        $" AND ((user_name CONTAINS[c] '{SearchText}' OR first_name CONTAINS[c] '{SearchText}' OR role_name CONTAINS[c] '{SearchText}' " +
                        $"OR last_name CONTAINS[c] '{SearchText}' OR middle_name CONTAINS[c] '{SearchText}'" +
                        $" OR ANY contacts.title CONTAINS[c] '{SearchText}' OR ANY contacts.body CONTAINS[c] '{SearchText}'))";
                }

                else if (!IsNoEmpty(SearchText) && IsNoEmpty(CurrentOrganizationId) && IsNoEmpty(CurrentDepartamentId))
                {
                    filter = $"departament_id == '{CurrentDepartamentId}' AND organization_id =='{CurrentOrganizationId}' AND user_name != '{CurrentLogin}'";
                }
                else if (IsNoEmpty(SearchText) && IsNoEmpty(CurrentOrganizationId) && IsNoEmpty(CurrentDepartamentId))
                {
                    filter = $"departament_id == '{CurrentDepartamentId}' AND organization_id == '{CurrentOrganizationId}' AND user_name != '{CurrentLogin}'" +
                       $" AND ((user_name CONTAINS[c] '{SearchText}' OR first_name CONTAINS[c] '{SearchText}' OR role_name CONTAINS[c] '{SearchText}' " +
                       $"OR last_name CONTAINS[c] '{SearchText}' OR middle_name CONTAINS[c] '{SearchText}'" +
                       $" OR ANY contacts.title CONTAINS[c] '{SearchText}' OR ANY contacts.body CONTAINS[c] '{SearchText}'))";   
                }
                else
                    filter = "departament_id == '{CurrentDepartamentId}'";

                Members = realm.All<Member>().Filter($"{filter} {_sort}");

                if (Members is not null)
                    MemberIsNotNull = Members.Count() > 0;
            }
            catch (Exception ex)
            {

            }
            await Task.CompletedTask;
        }


   






        [RelayCommand]
        public void BackToOrg()
        {
            SearchVisible = false;

            /* IsOrgSelekt = false;
             IsntDepsSelect = true;*/
            OrganizationIdTemp = string.Empty;
            CurrentOrganizationId = string.Empty;
            CurrentDepartamentId = string.Empty;
            IsDepId = string.Empty;
            CurretnOrgTitle = string.Empty;
            CurrentDepartament = null;
            Title = "";
            LoadAllMembers();
            //GetAllOrganizations();
        }

        [RelayCommand]
        public async void AddOrgOrDep()
        {
            if (IsDepsSelect)
                await AppShell.Current.GoToAsync($"{nameof(DepPage)}?is_dep=true");
            else
                await AppShell.Current.GoToAsync($"{nameof(OrgPage)}?is_dep=false");

        }
       

        [RelayCommand]
        public async void GoToOrgDepDetail(string param)
        {
            if (!IsNoEmpty(param))
                return;
          
            if (IsNoEmpty(OrganizationIdTemp))
            {
               
                await AppShell.Current.GoToAsync($"{nameof(DepPage)}?dep_id={param}");
                return;
            }
           
               await AppShell.Current.GoToAsync($"{nameof(OrgPage)}?org_id={param}");

        }

        internal void OnDissapering()
        {
            subscribeMembers?.Dispose();
            subscribeDeparaments?.Dispose();

            

        }

        [RelayCommand]
        public void ChengeSortVisble()
        {
            IsSortVisble = !IsSortVisble;
        }
    }

}

    