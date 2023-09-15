namespace EmployManager.Views;
using EmployManager.ViewModels;
using Microsoft.Maui.Storage;
using static EmployManager.Models.Enums;

public partial class EmployDetailPage : ContentPage
{
	private EmployDetailViewModel vm;
	public EmployDetailPage()
	{
		InitializeComponent();
		vm= new EmployDetailViewModel();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		vm.OnAppering();
    }

    async void Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
	{
		var role = MembersRole.User;
        if (RolePicker.SelectedIndex == 0)
            await vm.SelectRole(MembersRole.Admin);
        else if (RolePicker.SelectedIndex == 1)
            await vm.SelectRole(MembersRole.Manager);
        else if (RolePicker.SelectedIndex == 2)
           await vm.SelectRole(MembersRole.User);
    }
}
