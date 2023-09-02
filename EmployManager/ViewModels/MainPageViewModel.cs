using ARKit;
using EmployManager.Models;
using EmployManager.Services;
using System;
using System.Collections.ObjectModel;

namespace EmployManager.ViewModels
{
	public class MainPageViewModel
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


	}
}

