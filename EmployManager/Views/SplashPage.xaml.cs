﻿using EmployManager.ViewModels;

namespace EmployManager.Views;

public partial class SplashPage : ContentPage
{
    private SplashViewModel vm;
  
        public SplashPage()
	{
		InitializeComponent();
        vm = new SplashViewModel();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        vm.OnAppering();
    }
}
