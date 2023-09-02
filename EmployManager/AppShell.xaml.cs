using EmployManager.Views;

namespace EmployManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(SetingsPage), typeof(SetingsPage));

    }
}

