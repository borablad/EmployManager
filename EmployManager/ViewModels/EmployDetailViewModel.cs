using CommunityToolkit.Mvvm.ComponentModel;
using EmployManager;
using EmployManager.Models;
using EmployManager.ViewModels;
using Newtonsoft.Json;
using EmployManager.Views;
using Realms;
using EmployManager.Services;
using CommunityToolkit.Mvvm.Input;
using static EmployManager.Models.Enums;
using System.Security.Cryptography;
using System.Text;

using System.Collections.ObjectModel;

namespace EmployManager.ViewModels;

public partial class EmployDetailViewModel : BaseViewModel
{
    public static string MemberId { get => Preferences.Get(nameof(MemberId), ""); set => Preferences.Set(nameof(MemberId), value); }
    [ObservableProperty]
    private Member updateMember;

    [ObservableProperty]
    private bool isUpdate=true;
    [ObservableProperty]
    private string firstName, lastName, password, photoUrl, login, roleName, midleName;
    
 

    [ObservableProperty]
    private MembersRole memberRole=MembersRole.User;


    [ObservableProperty]
    ObservableCollection<EmployManager.Models.Contacts> contactsVisual;

    private Member CurrentUser;

    private Realm realm;
    public EmployDetailViewModel()
    {

        
    }

    internal async Task OnAppering()
    {
        if (realm is null)
            realm = RealmService.GetMainThreadRealm();
        
        if (IsNoEmpty(MemberId))
        {
            UpdateMember = realm.All<Member>().Where(x => x.Id == MemberId).FirstOrDefault();

            MemberId = "";
            if (UpdateMember is null)
            {
                UpdateMember = new Member();

                IsUpdate = false;
                return;
            }
            UpdateMember?.Contacts.ToList().ForEach(x => ContactsVisual.Add(x));

        }
        else
            IsUpdate = false;
    }
    [RelayCommand]
    private async Task SelectRole(string parametr)
    {
        if (CurrentUser.Role == MembersRole.Manager && parametr == nameof(MembersRole.Admin))
            return;
        MemberRole = parametr switch
        {
            nameof(MembersRole.Admin) => MembersRole.Admin,
            nameof(MembersRole.Manager)=> MembersRole.Manager,   
            _ => MembersRole.User
        };
         
    }


    [RelayCommand]
    public async Task AddNewContact()
    {
        ContactsVisual.Add(new EmployManager.Models.Contacts());

    }

  

    private async Task UpdateMembers()
    {
        if (UpdateMember is null)
            return;


        var currentDep = realm.All<Departanent>().FirstOrDefault(x => x.Id == CurrentDepartamentId);
        var result = ContactsVisual.Where(x => IsNoEmpty(x.Body) && IsNoEmpty(x.Title)).ToList();
       // var result = UpdateMember.Contacts.Where(x => memberscontacts.Any(y => x.Id != y.Id)).ToList();
        await realm.WriteAsync(() =>
        {
            if (result.Count > 0)
                result.ForEach(x => UpdateMember.Contacts.Add(x));

            currentDep.Members.Add(UpdateMember);
            realm.Add(currentDep);
        });

        await AppShell.Current.GoToAsync($"{nameof(MainPage)}");
    }


    [RelayCommand]
    private async Task CreateMember()
    {
        if(IsUpdate)
        {
            UpdateMembers();
            return;
        }
        if (!IsNoEmpty(Login) || !IsNoEmpty(Password))
        {
            await DialogService.ShowAlertAsync("Ошибка", "Заполните все обязательные поля!");
            return;
        }
        UpdateMember.FirstName = FirstName;
        UpdateMember.LastName = LastName;
        UpdateMember.MiddleName = MidleName;
        UpdateMember.Username = Login;
        UpdateMember.Password = CreateHashPassword(Password);
        UpdateMember.PhotoUrl = PhotoUrl;
        UpdateMember.Role = MemberRole;
        UpdateMember.DepartamentId = CurrentDepartamentId;
        UpdateMember.OrganizationId = CurrentOrganizationId;
        UpdateMember.RoleName = RoleName;

        var currentDep = realm.All<Departanent>().FirstOrDefault(x => x.Id == CurrentDepartamentId);
        var result = ContactsVisual.Where(x => IsNoEmpty(x.Body) && IsNoEmpty(x.Title)).ToList();
        //var result = UpdateMember.Contacts.Where(x => memberscontacts.Any(y => x.Id != y.Id)).ToList();
        await realm.WriteAsync(() =>
        {
            if(result.Count > 0)
                result.ForEach(x => UpdateMember.Contacts.Add(x));

            currentDep.Members.Add(UpdateMember);
            realm.Add(currentDep);
        });
        await AppShell.Current.GoToAsync($"{nameof(MainPage)}");

    }

    private string CreateHashPassword(string password)
    {
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

    }

}