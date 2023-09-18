using EmployManager.ViewModels;

namespace EmployManager.Views;

public partial class DepPage : ContentPage
{
	private OrgDepPageViewModel vm;
	public DepPage()
	{
		InitializeComponent();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        vm = new OrgDepPageViewModel();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		vm.OnAppearing();
    }
}
