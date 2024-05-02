using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.Serialization;
using Wpf.Ui.Mvvm.Contracts;

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
