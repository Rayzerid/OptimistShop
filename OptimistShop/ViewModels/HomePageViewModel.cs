using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;
using OptimistShop.Core;
using System.Linq;
using System.IO;
using OptimistShop.Models.DbTables;
using Wpf.Ui.Common.Interfaces;

namespace OptimistShop.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private INavigationService? navService;

        [ObservableProperty]
        private string _textButton = "Войти";

        [RelayCommand]
        private void GoToMenu()
        {
            navService = App.GetService<INavigationService>();
            if (TextButton == "Войти")
                navService.Navigate(typeof(Views.Pages.LoginPage));
            else
                navService.Navigate(typeof(Views.Pages.MenuPage));
        }
    }
}
