namespace EmployManager.Views;

public partial class SetingsPage : ContentPage
{
	public SetingsPage()
	{
		InitializeComponent();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

    }
}
