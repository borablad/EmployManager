﻿using EmployManager.ViewModels;

namespace EmployManager.Views;

public partial class LoginPage : ContentPage
{
    private LoginViewModel vm;
    public LoginPage()
    {
        InitializeComponent();
        vm = new LoginViewModel();
        BindingContext = vm;
        //vm.IsBusy = true;
       

    }

    void SwichPassVisible(System.Object sender, System.EventArgs e)
    {
        PassEntry.IsPassword = !PassEntry.IsPassword;
    }


    protected override async void OnAppearing()
    {

        base.OnAppearing();
       // await vm.OnAppearing();
        Shell.SetTabBarIsVisible(this, false);
    }

}
