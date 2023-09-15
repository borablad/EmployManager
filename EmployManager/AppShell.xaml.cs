using EmployManager.Views;

namespace EmployManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(SetingsPage), typeof(SetingsPage));
        Routing.RegisterRoute(nameof(EmployDetailPage), typeof(EmployDetailPage)); 
        Routing.RegisterRoute(nameof(OrgPage), typeof(OrgPage)); 
        Routing.RegisterRoute(nameof(DepPage), typeof(DepPage)); 






    }
}

