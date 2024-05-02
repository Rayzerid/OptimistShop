using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace OptimistShop.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private StoreDbContext _dbContext;
        private MenuPageViewModel _menuPageViewModel;
        private HomePageViewModel _homePageViewModel;
        private LoginPageViewModel _loginPageViewModel;
        private bool _isInitialized = false;
        private INavigationService? navService;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(IsLoginFilled))]
        private Visibility _loginGridVisibility = Visibility.Hidden;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LoginInterfaceVisibility))]
        private Visibility _progressRingVisibility = Visibility.Hidden;

        [ObservableProperty]
        private Visibility _cartVisibility = Visibility.Hidden;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLoginBtnVisible), nameof(IsLogoutBtnVisible), nameof(LoginStateVisibility))]
        private bool _isUserAuthorized = false;

        [ObservableProperty]
        private bool _isCartFilled = false;

        [ObservableProperty]
        private bool _rememberMeIsChecked = true;

        [ObservableProperty]
        private int _userID = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BadgeVisibility))]
        private int _badgeValue;
        public Visibility BadgeVisibility => BadgeValue > 0 ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsLoginBtnVisible => IsUserAuthorized == true ? Visibility.Hidden : Visibility.Visible;
        public Visibility IsLogoutBtnVisible => IsUserAuthorized == true ? Visibility.Visible : Visibility.Hidden;
        public Visibility LoginInterfaceVisibility => ProgressRingVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        public Visibility LoginStateVisibility => IsUserAuthorized == true ? Visibility.Visible : Visibility.Hidden;
        public bool IsLoginFilled => LoginGridVisibility == Visibility.Visible ? true : false;

        public MainWindowViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
                InitializeViewModel();
        }
        private async void InitializeViewModel()
        {
            try
            {
                ApplicationTitle = "Optimist";
                Accent.Apply(Color.FromRgb(0, 0, 0));

                _menuPageViewModel = App.GetService<MenuPageViewModel>();
                _dbContext = await Task.Run(() => new StoreDbContext());

                NavigationItems = new ObservableCollection<INavigationControl>
                {
                    new NavigationItem()
                    {
                        Content = "Главная",
                        PageTag = "home",
                        Icon = SymbolRegular.Home20,
                        PageType = typeof(Views.Pages.HomePage),
                        ToolTip = "Главная",
                        IconForeground = Brushes.Black,
                        IsActive = true,
                        FontWeight = FontWeights.Black
                    },

                    new NavigationItem()
                    {
                        Content = "Каталог",
                        PageTag = "catalog",
                        Icon = SymbolRegular.Album24,
                        PageType = typeof(Views.Pages.MenuPage),
                        ToolTip = "Каталог",
                        IconForeground = Brushes.Black,
                        FontWeight = FontWeights.Black
                    }
                };

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }


        [RelayCommand]
        private void OpenLoginGrid()
        {
            navService = App.GetService<INavigationService>();
            navService.Navigate(typeof(Views.Pages.LoginPage));
        }


        [RelayCommand]
        private void Logout()
        {
            NavigationItems.Clear();

            _homePageViewModel = App.GetService<HomePageViewModel>();
            _loginPageViewModel = App.GetService<LoginPageViewModel>();

            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Главная",
                    PageTag = "home",
                    Icon = SymbolRegular.Home20,
                    PageType = typeof(Views.Pages.HomePage),
                    ToolTip = "Главная",
                    IconForeground = Brushes.Black,
                    IsActive = true,
                    FontWeight = FontWeights.Black
                },

                new NavigationItem()
                {
                    Content = "Каталог",
                    PageTag = "catalog",
                    Icon = SymbolRegular.Album24,
                    PageType = typeof(Views.Pages.MenuPage),
                    ToolTip = "Каталог",
                    IconForeground = Brushes.Black,
                    FontWeight = FontWeights.Black
                }
            };

            CartVisibility = Visibility.Hidden;
            IsUserAuthorized = false;
            BadgeValue = 0;

            navService = App.GetService<INavigationService>();
            navService.Navigate(typeof(Views.Pages.HomePage));

            _homePageViewModel.TextButton = "Войти";

            _loginPageViewModel.EmailUser = string.Empty;
            _loginPageViewModel.UserPassword = string.Empty;
        }


        [RelayCommand]
        private void OpenCart()
        {
            IsCartFilled = true;
            navService = App.GetService<INavigationService>();
            navService.Navigate(typeof(Views.Pages.CartPage));
        }


        [RelayCommand]
        private void GoToSignUp()
        {
            LoginGridVisibility = Visibility.Hidden;

            navService = App.GetService<INavigationService>();
            navService.Navigate(typeof(Views.Pages.RegistrationPage));
        }
    }
}
