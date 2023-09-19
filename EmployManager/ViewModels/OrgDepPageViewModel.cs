using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using EmployManager.Services;
using EmployManager.Models;
using EmployManager.Views;

namespace EmployManager.ViewModels
{
	public partial class OrgDepPageViewModel:BaseViewModel, IQueryAttributable
    {
        private Realm realm;

        private bool IsDep;

        private string CurrentMemberId, DepartamentId, OrganizationId;


        private Departanent dep;
        private Organization org;

        [ObservableProperty]
        private string name, description,photoUrl;

		public OrgDepPageViewModel()
		{
		}

		internal void OnAppearing()
		{
            if (realm is null)
                realm = RealmService.GetMainThreadRealm();

            CurrentMemberId = realm.All<Member>().FirstOrDefault(x => x.Username == CurrentLogin)?.Id;
		}


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (realm is null)
                realm = RealmService.GetMainThreadRealm();
            if (query.Count > 0 && query.ContainsKey("is_dep") && query["is_dep"] != null)
            {
                IsDep = bool.Parse(query["is_dep"].ToString());
            }
            if (query.Count > 0 && query.ContainsKey("org_id") && query["org_id"] != null)
            {
                OrganizationId=query["org_id"].ToString();
                 org = realm.All<Organization>().FirstOrDefault(x => x.Id == OrganizationId);
                Name = org?.Title;
                Description = org?.Title;
            }
            else if (query.Count > 0 && query.ContainsKey("dep_id") && query["dep_id"] != null)
            {
                DepartamentId=query["dep_id"].ToString();
                dep = realm.All<Departanent>().FirstOrDefault(x => x.Id == DepartamentId);

                Name = dep?.Title;
                Description = dep?.Description;
            }

        }


        [RelayCommand]
		private async void SaveOrgDep()
		{


            if (IsNoEmpty(DepartamentId) || IsNoEmpty(OrganizationId))
            {
                UpdateOrgDep();
                return;
            }





            if (!IsNoEmpty(Name))
            {
                await DialogService.ShowError($"Наименование {(IsDep?"отдела":"организации")} обязательное поле");
                return;
            }
            if (IsDep)
            {
                if (!IsNoEmpty(CurrentMemberId))
                    return;
                if (!IsNoEmpty(CurrentOrganizationId))
                {
                    await DialogService.ShowError("Не выбранна организация");
                    return;
                }

                var depar = new Departanent { CreaterId = CurrentMemberId, OrganizationId = CurrentOrganizationId, Description = Description, Title = Name, PhotoUrl = PhotoUrl };

             await WriteToRealm(depar);

                await AppShell.Current.GoToAsync($"..");
                return;
            }

            var org = new Organization { Description = Description, PhotoUrl = PhotoUrl, Title = Name };
            await WriteToRealm(org);
            await AppShell.Current.GoToAsync($"..");


        }

        private async void UpdateOrgDep()
        {
            if (IsNoEmpty(DepartamentId))
            {
                

                if (dep is null)
                    return;
                if (!IsNoEmpty(CurrentOrganizationId))
                {
                    await DialogService.ShowError("Не выбранна организация");
                    return;
                }
                await realm.WriteAsync(() => {
                    dep.Description = Description;
                    dep.Title = Name;
                    dep.PhotoUrl = PhotoUrl;
                    if (!IsNoEmpty(dep.OrganizationId))
                        dep.OrganizationId = CurrentOrganizationId;
                    if (!IsNoEmpty(dep.CreaterId))
                        dep.CreaterId = CurrentMemberId;
                    realm.Add(dep);
                });
              
                await AppShell.Current.GoToAsync($"..");
                return;
            }


            if (org is null)
                return;
            await realm.WriteAsync(() => {

                org.Description = Description;
                org.Title = Name;
                org.PhotoUrl = PhotoUrl;
                realm.Add(org);

            });
          
            await AppShell.Current.GoToAsync($"..");

        }


        [RelayCommand]
        private async void Back()
        {
            await AppShell.Current.GoToAsync($"..");
        }

        private async Task WriteToRealm(params RealmObject[] parametrs)
        {
           await realm.WriteAsync(() => {
               foreach (var parametr in parametrs)
               {
                   realm.Add(parametr);
               }

           });
            await Task.CompletedTask;
           
        }



	}
}




