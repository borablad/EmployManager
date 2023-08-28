using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployManager.Services
{
    public static class DialogService
    {
        public static Task ShowAlertAsync(string title, string message)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }


        public static  Task ShowError(string message)
        {
            return Application.Current.MainPage.DisplayAlert("Ошибка", message, "OK");
        }



    }
}
