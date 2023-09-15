namespace EmployManager.Views;
using EmployManager.ViewModels;
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

    void Picker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
    {
    }
}
