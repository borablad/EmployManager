using EmployManager.ViewModels;

namespace EmployManager.Views;

public partial class MainPage : ContentPage
{
	private MainPageViewModel vm;

	public MainPage()
	{
		InitializeComponent();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        vm = new MainPageViewModel();
		BindingContext = vm;	
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		vm.OnAppering();
		
    }

}


