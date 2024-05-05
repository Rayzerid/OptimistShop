using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace OptimistShop.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject, INavigationAware
    {
        private INavigationService? navService;
        private StoreDbContext _dbContext;
        private User? userModel;
        private List<ClothesContain> foodContainItems;
        private MainWindowViewModel _mainWindowViewModel;
        private HomePageViewModel _homePageViewModel;

        [ObservableProperty]
        private string _roleName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _emailUser = string.Empty;

        [ObservableProperty]
        private string _userPassword = string.Empty;

        [ObservableProperty]
        private string _snackbarMessage = string.Empty;

        [ObservableProperty]
        private int _userID = 0;

        [RelayCommand]
        private void GoToRegistration()
        {
            navService = App.GetService<INavigationService>();
            navService.Navigate(typeof(Views.Pages.RegistrationPage));
        }

        [RelayCommand(CanExecute = nameof(CheckFields))]
        private async void Login(Snackbar snackbar)
        {
            try
            {
                _mainWindowViewModel = App.GetService<MainWindowViewModel>();
                _homePageViewModel = App.GetService<HomePageViewModel>();

                navService = App.GetService<INavigationService>();

                _dbContext = new StoreDbContext();
                userModel = await Task.Run(() => _dbContext.User.Include(x => x.Role).FirstOrDefaultAsync(u => u.UserMail == EmailUser && u.UserPassword == UserPassword));

                if (userModel != null)
                {
                    RoleName = userModel.Role.RoleName;
                    EmailUser = userModel.UserMail;

                    switch (userModel.RoleID)
                    {
                        case 1:       
                            _mainWindowViewModel.NavigationItems.Add(new NavigationItem()
                            {
                                Content = "Заказы",
                                PageTag = "orders",
                                Icon = SymbolRegular.Box16,
                                PageType = typeof(Views.Pages.OrdersPage),
                                ToolTip = "Заказы",
                                FontWeight = FontWeights.Black,
                                IconForeground = Brushes.Black
                            });

                            foodContainItems = await Task.Run(() => _dbContext.ClothesContain.Where(x => x.Cart.UserID == userModel.UserID).ToList());
                            foreach (ClothesContain item in foodContainItems)
                            _mainWindowViewModel.BadgeValue += item.Count;

                            UserID = userModel.UserID;
                            _mainWindowViewModel.IsUserAuthorized = true;
                            _mainWindowViewModel.CartVisibility = Visibility.Visible;

                            _mainWindowViewModel.LoginGridVisibility = Visibility.Hidden;

                            _homePageViewModel.TextButton = "Каталог";
                            navService.Navigate(typeof(Views.Pages.HomePage));
                            break;
                        case 2:
                            _mainWindowViewModel.NavigationItems.Clear();

                            UserID = userModel.UserID;
                            _mainWindowViewModel.IsUserAuthorized = true;

                            SnackbarMessage = "Успешный вход!";
                            snackbar.Appearance = ControlAppearance.Success;
                            snackbar.ShowAsync();

                            navService = App.GetService<INavigationService>();
                            navService.Navigate(typeof(Views.Pages.EmployeePage));

                            _mainWindowViewModel.ProgressRingVisibility = Visibility.Hidden;

                            break;
                    }
                }
                else
                {
                    snackbar.Appearance = ControlAppearance.Dark;
                    SnackbarMessage = "Неверный логин или пароль!";
                    snackbar.ShowAsync();
                    return;
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private bool CheckFields()
        {
            return !(Regex.IsMatch(EmailUser, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false || EmailUser == null || EmailUser.Trim() == string.Empty);
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
        }
    }
}
