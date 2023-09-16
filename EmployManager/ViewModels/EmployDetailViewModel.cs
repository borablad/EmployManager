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
using DocumentFormat.OpenXml.Drawing.Charts;

namespace EmployManager.ViewModels;

public partial class EmployDetailViewModel : BaseViewModel, IQueryAttributable
{
  
    [ObservableProperty]
    private Member updateMember;

    [ObservableProperty]
    private bool isUpdate;
    [ObservableProperty]
    private string firstName, lastName, password, photoUrl, login, roleName, midleName;

    [ObservableProperty]
    private double salary;

    [ObservableProperty]
     List<string> roles = new List<string>();

    [ObservableProperty]
    private MembersRole memberRole=MembersRole.User;

    [ObservableProperty]
    private int selectedIndex;


    [ObservableProperty]
    ObservableCollection<EmployManager.Models.Contacts> contactsVisual;

    private Member CurrentUser;
    private string MemberId;
    private Realm realm;
    public EmployDetailViewModel()
    {
        Roles.Add("Пользователь");
        Roles.Add("Мэнэджер");
        Roles.Add("Администратор"); 

    }

    internal async Task OnAppering()
    {
                if (realm is null)
            realm = RealmService.GetMainThreadRealm();
        //ContactsVisual.Add(new EmployManager.Models.Contacts());

       
    }


    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.Count > 0 && query.ContainsKey("member_id") && query["member_id"] != null)
        {

            if (realm is null)
                realm = RealmService.GetMainThreadRealm();

            MemberId = query["member_id"].ToString();
            UpdateMember = realm.All<Member>().Where(x => x.Id == MemberId).FirstOrDefault();

            MemberId = "";
            if (UpdateMember is null)
            {
                UpdateMember = new Member();

                IsUpdate = false;
                return;
            }
            FirstName = UpdateMember?.FirstName;
            LastName = UpdateMember?.LastName;
            PhotoUrl = UpdateMember?.PhotoUrl;
            RoleName = UpdateMember?.RoleName;
            MidleName = UpdateMember?.MiddleName;
            MemberRole = UpdateMember.Role;
            Login = UpdateMember?.Username;
            Salary = UpdateMember.Salary;

            SelectedIndex = (int)MemberRole;
            if(UpdateMember.Contacts is not null)
            {
                var i = UpdateMember?.Contacts.ToList();

                ContactsVisual = new ObservableCollection<Models.Contacts>();
                
                i.ForEach(x => ContactsVisual.Add(x));
        
                }   
                
            IsUpdate = true;

        }

    }




    [RelayCommand]
    public async Task SelectRole(MembersRole parametr)
            {
        //if (CurrentUser.Role == MembersRole.Manager && parametr == (MembersRole.Admin))
        //    return;
        MemberRole = parametr;//switch
        //{
        //    nameof(MembersRole.Admin) => MembersRole.Admin,
        //    nameof(MembersRole.Manager)=> MembersRole.Manager,   
        //    _ => MembersRole.User
        //};
         
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





        Departanent currentDep;
        currentDep = realm.All<Departanent>().FirstOrDefault(x => x.Id == CurrentDepartamentId);
        if (currentDep is null)
            currentDep = realm.All<Departanent>().Where(x => x.OrganizationId == CurrentOrganizationId).ToList()[0];

        var result = new List<Models.Contacts>();
        if (ContactsVisual is not null)
        {
            var tempres = ContactsVisual?.Where(x => IsNoEmpty(x.Body) && IsNoEmpty(x.Title)).ToList();
            if (UpdateMember.Contacts != null&&UpdateMember.Contacts.Count()>0)
            {
                result = tempres.Where(x => UpdateMember.Contacts.Any(y => y.Id != x.Id)).ToList();
            }
            else
            {
                result = tempres;
            }
        }
           



        // var result = UpdateMember.Contacts.Where(x => memberscontacts.Any(y => x.Id != y.Id)).ToList();try{
        try
        {
            var i = IsNoEmpty(Password);
              
            await realm.WriteAsync(() =>
            {
                UpdateMember.FirstName = FirstName;
                UpdateMember.LastName = LastName;
                UpdateMember.MiddleName = MidleName;
                UpdateMember.Salary = Salary;
                UpdateMember.PhotoUrl = PhotoUrl;
                UpdateMember.RoleName = RoleName;
                //if (!IsNoEmpty(Password))
                //    UpdateMember.Password = CreateHashPassword(Password);

                UpdateMember.Role = MemberRole;
                if (result?.Count > 0)
                {
                    result?.ForEach(x => UpdateMember.Contacts.Add(x));
                }
                    

                realm.Add(currentDep);
            });
        }catch(Exception ex)
        {
            var i = ex;
            return;
        }
        await AppShell.Current.GoToAsync($"..");
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
        Departanent currentDep;
        currentDep = realm.All<Departanent>().FirstOrDefault(x => x.Id == CurrentDepartamentId);
        if (currentDep is null)
            currentDep = realm.All<Departanent>().Where(x => x.OrganizationId == CurrentOrganizationId).ToList()[0];
        if (ContactsVisual is null)
            ContactsVisual = new ObservableCollection<Models.Contacts>();
        UpdateMember = new Member();
        UpdateMember.FirstName = FirstName;
        UpdateMember.LastName = LastName;
        UpdateMember.MiddleName = MidleName;
        UpdateMember.Username = Login;
        UpdateMember.Password = CreateHashPassword(Password);
        UpdateMember.PhotoUrl = PhotoUrl;
        UpdateMember.Role = MemberRole;
        UpdateMember.DepartamentId = currentDep.Id;
        UpdateMember.OrganizationId = CurrentOrganizationId;
        UpdateMember.RoleName = RoleName;




        
        var result = new List<Models.Contacts>();
        if (ContactsVisual is not null)
            result = ContactsVisual?.Where(x => IsNoEmpty(x.Body) && IsNoEmpty(x.Title)).ToList();
        //var result = UpdateMember.Contacts.Where(x => memberscontacts.Any(y => x.Id != y.Id)).ToList();
        await realm.WriteAsync(() =>
        {
            if(result.Count > 0)
                result.ForEach(x => UpdateMember.Contacts.Add(x));

            currentDep.Members.Add(UpdateMember);
            realm.Add(UpdateMember);
        });
                            await AppShell.Current.GoToAsync($"..");

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

    [RelayCommand]
    void Back()
    {
        AppShell.Current.GoToAsync("..");
    }
}