﻿using EmployManager.Views;

namespace EmployManager;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new AppShell();

        //MainPage = new NavigationPage(new LoginPage());
    }
}

