
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EmployManager.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    // текуший логин
    public static string CurrentLogin { get => Preferences.Get(nameof(CurrentLogin), ""); set => Preferences.Set(nameof(CurrentLogin), value); }

   
    [ObservableProperty]
    protected bool isBusy;

    [ObservableProperty]
    private string title;

    protected Action currentDismissAction;

    internal IConnectivity _connectivity;


    public BaseViewModel()
    {
        //if (_connectivity == null)
        //    _connectivity = ServiceHelper.GetService<IConnectivity>();
    }

    // Запуск индикатора активности
    partial  void OnIsBusyChanged(bool value)
    {
        try
        {
            if (value)
            {
               // currentDismissAction = Services.DialogService.ShowActivityIndicator();
            }
            else
            {
                currentDismissAction?.Invoke();
                currentDismissAction = null;                
            }
        }
        catch (Exception)
        {

        }

    }

    //Регулярное выражение на проверку почты
    public bool IsValidEmail(string email) => 
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    // Выход    
    public async Task LogoutTapped()
    {
        IsBusy = true;
        //await RealmService.LogoutAsync();

        IsBusy = false;
        //Application.Current.MainPage = new LoginPage();
        //await Shell.Current.GoToAsync($"//LoginPage");
    }

   

}
