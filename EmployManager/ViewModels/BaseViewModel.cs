
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EmployManager.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    // текуший логин
    public static string CurrentLogin { get => Preferences.Get(nameof(CurrentLogin), ""); set => Preferences.Set(nameof(CurrentLogin), value); }
    public static string Token { get => Preferences.Get(nameof(Token), ""); set => Preferences.Set(nameof(Token), value); }
    public DateTime DateLogin { get => DateTime.Parse(Preferences.Get(nameof(DateLogin), DateTime.MinValue.ToString())); set => Preferences.Set(nameof(DateLogin), value.ToString()); }

    public const int TokenAliveSecond = 60 *60 * 24;

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
    public bool IsNoEmpty(string obj)=>
        string.IsNullOrWhiteSpace(obj) ? false : true;

    public bool IsTokenAlive() =>
        DateLogin < DateTime.Now;
    
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
