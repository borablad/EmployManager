using CommunityToolkit.Mvvm.ComponentModel;
using EmployManager;
using EmployManager.Models;
using EmployManager.ViewModels;
using Newtonsoft.Json;
using EmployManager.Views;

public partial class EmployDetailViewModel : BaseViewModel
{
    public static string CurrentMember { get => Preferences.Get(nameof(CurrentMember), ""); set => Preferences.Set(nameof(CurrentMember), value); }
    [ObservableProperty]
    private Member updateMember;
    public EmployDetailViewModel()
    {

        if (!string.IsNullOrWhiteSpace(CurrentMember))
        {
            UpdateMember = JsonConvert.DeserializeObject<Member>(CurrentMember);
            CurrentMember = "";
        }
    }

    internal void OnAppering()
    {

    }
}