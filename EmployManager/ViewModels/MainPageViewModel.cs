//using ARKit;
using CommunityToolkit.Mvvm.Input;
using EmployManager.Models;
using EmployManager.Services;
using EmployManager.Views;
using System;
using System.Collections.ObjectModel;

namespace EmployManager.ViewModels
{
	public partial   class MainPageViewModel : BaseViewModel
	{
		public ObservableCollection<Departanent> Departanents { get; set; }=new ObservableCollection<Departanent>();

		public Organization Organization { get; set; }

		public MainPageViewModel()
		{
		}


		internal async void OnAppering()
		{
			Organization = await  GetOrganization();

           Organization.Departanents.ForEach(d => Departanents.Add(d));
        }


		private async Task <Organization> GetOrganization()
		{
			var organization = new Organization();

			organization = await Rest.GetOrganization();


			return organization;
		}

		[RelayCommand]
		public async void GoToSettings()
		{
			await AppShell.Current.GoToAsync($"{nameof(SetingsPage)}");
		}
        [RelayCommand]
        public async void LogOut()
        {
			Token = string.Empty;
			DateLogin = DateTime.MinValue;
            await AppShell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}

