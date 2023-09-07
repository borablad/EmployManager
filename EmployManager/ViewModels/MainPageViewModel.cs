//using ARKit;
using CommunityToolkit.Mvvm.Input;
using EmployManager.Models;
using EmployManager.Services;
using EmployManager.Views;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace EmployManager.ViewModels
{
	public partial   class MainPageViewModel : BaseViewModel
	{
		public ObservableCollection<Departanent> Departanents { get; set; }=new ObservableCollection<Departanent>();
		public ObservableCollection<Member> Members { get; set; }=new ObservableCollection<Member>();

        public static string CurrentMember { get => Preferences.Get(nameof(CurrentMember), ""); set => Preferences.Set(nameof(CurrentMember), value); }

        public Organization Organization { get; set; }


		public Departanent CurrentDepartament { get; set; }


		public MainPageViewModel()
		{
			

        }


		internal async void OnAppering()
		{
			Organization =  await GetOrganization();

           Organization.Departanents.ForEach(d => Departanents.Add(d));
        }


		private async Task <Organization> GetOrganization()
		{
			var organization = new Organization();

			organization = await Rest.GetOrganization();

			

			return organization;
		}

		[RelayCommand]
		public async void ChangeDepartament( Departanent departanent)
		{
			CurrentDepartament = departanent;
			//Members = departanent.Members.ToList();
			departanent.Members.ForEach(x=>Members.Add(x));
		}

		[RelayCommand]
		public async void GoToSettings()
		{
			await AppShell.Current.GoToAsync($"{nameof(SetingsPage)}");
		}

        [RelayCommand]
        public async void GoToEmployDetail(Member member)
        {

            // await AppShell.Current.GoToAsync($"{nameof(EmployDetailPage)}?item={member}");

			CurrentMember= JsonConvert.SerializeObject(member);
         
            await AppShell.Current.GoToAsync($"{nameof(EmployDetailPage)}");


        }


        [RelayCommand]
        public async void LogOut()
        {
			Token = string.Empty;
			DateLogin = DateTime.MinValue;
            await AppShell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

         public void OnSearchtOextChanged(string value)
        {
			if (string.IsNullOrWhiteSpace(value))
			{
                CurrentDepartament?.Members.ForEach(x => Members.Add(x));
				return;
            }
			Members.Clear();
			CurrentDepartament.Members.Where(x => x.Username.Contains(value)|| x.Contacts.Where(y => y.Title.Contains(value) || y.Body.Contains(value)).ToList().Count > 0).ToList().ForEach(x => Members.Add(x));
	


        }

    }
}

