using EmployManager.ViewModels;

namespace EmployManager.Views;

public partial class MainPage : ContentPage
{
	private MainPageViewModel vm;

	public MainPage()
	{
		InitializeComponent();
		vm= new MainPageViewModel();
		BindingContext = vm;	
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		vm.OnAppering();
		
    }

}


