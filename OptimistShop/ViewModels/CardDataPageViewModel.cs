using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using System;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace OptimistShop.ViewModels
{
    public partial class CardDataPageViewModel : ObservableObject, INavigationAware
    {
        private MainWindowViewModel? _mainWindowViewModel;
        private CartPageViewModel? _cartPageViewModel;
        private INavigationService? navService;
        private StoreDbContext _dbContext;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
        private string _cardNumber;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
        private string _month;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
        private string _year;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
        private string _cvv;

        private async void InitializeViewModel()
        {
            try
            {
                _mainWindowViewModel = App.GetService<MainWindowViewModel>();
                _cartPageViewModel = App.GetService<CartPageViewModel>();
                _dbContext = await Task.Run(() => new StoreDbContext());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        [RelayCommand(CanExecute = nameof(CheckFields))]
        private async void Confirm(PasswordBox passwordBox)
        {
            try
            {
                Order orderObj = new Order
                {
                    UserID = _mainWindowViewModel.UserID,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Подтвержден",
                    OrderTotal = _cartPageViewModel.ProductsSummary,
                    IsPaid = true,
                    PaymentMethod = "Карта"
                };
                await _dbContext.Order.AddAsync(orderObj);
                await _dbContext.SaveChangesAsync();

                int OrderObjID = orderObj.OrderID;

                //Добавление товаров из корзины в контейнер заказа и очистка корзины
                foreach (ClothesContain item in _cartPageViewModel.CartItems)
                {
                    await _dbContext.OrderContain.AddAsync(new OrderContain()
                    {
                        OrderID = OrderObjID,
                        ClothesID = item.ClothesID,
                        Count = item.Count
                    });
                }

                //Удаление элементов корзины
                await Task.Run(() => _dbContext.ClothesContain.RemoveRange(_cartPageViewModel.CartItems));
                await _dbContext.SaveChangesAsync();

                _mainWindowViewModel.BadgeValue = 0;

                CardNumber = string.Empty;
                Month = string.Empty;
                Year = string.Empty;
                passwordBox.Password = string.Empty;

                navService = App.GetService<INavigationService>();
                navService.Navigate(typeof(Views.Pages.CartPage));

                _cartPageViewModel.SnackBar.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }
        private bool CheckFields()
        {
            return !(CardNumber == null || CardNumber.Trim() == string.Empty ||
                   Cvv == null || Cvv.Trim() == string.Empty ||
                   Year == null || Year.Trim() == string.Empty ||
                   Month == null || Month.Trim() == string.Empty ||
                   Cvv.Length < 3 || Year.Length < 2 || Month.Length < 2 || CardNumber.Contains('_'));
        }

        public void OnNavigatedTo()
        {
            InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
