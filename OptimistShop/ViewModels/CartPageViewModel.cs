using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace OptimistShop.ViewModels
{
    public partial class CartPageViewModel : ObservableObject, INavigationAware
    {
        private MainWindowViewModel? _mainWindowViewModel;
        private StoreDbContext _dbContext;
        private INavigationService? navService;

        [ObservableProperty]
        private ObservableCollection<ClothesContain> _cartItems;

        [ObservableProperty]
        private Snackbar _snackBar;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InterfaceIsEnabled))]
        private Visibility _progressRingVisibility = Visibility.Hidden;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EmptyCartVisibility))]
        private Visibility _interfaceVisibility = Visibility.Visible;

        [ObservableProperty]
        private string _countText;

        [ObservableProperty]
        private int _productsCount;

        [ObservableProperty]
        private int _productsSummary;

        [ObservableProperty]
        private bool _buttonsCountIsEnabled = true;

        [ObservableProperty]
        private bool _deleteBtnIsEnabled = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PayNowCheckBoxIsEnabled))]
        private bool _cashMethodIsChecked = true;

        [ObservableProperty]
        private bool _cardMethodIsChecked = false;

        [ObservableProperty]
        private bool _payNowCheckBoxIsChecked = false;
        public bool InterfaceIsEnabled => ProgressRingVisibility == Visibility.Hidden ? true : false;
        public bool PayNowCheckBoxIsEnabled => CashMethodIsChecked ? false : true;
        public Visibility EmptyCartVisibility => InterfaceVisibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;

        public void OnNavigatedFrom()
        {
            _mainWindowViewModel.IsCartFilled = false;
        }

        public void OnNavigatedTo()
        {
            InitializeViewModel();
            _mainWindowViewModel.IsCartFilled = true;
        }

        private async void InitializeViewModel()
        {
            try
            {
                _mainWindowViewModel = App.GetService<MainWindowViewModel>();

                if(_mainWindowViewModel.BadgeValue == 0)
                    InterfaceVisibility = Visibility.Hidden;
                else
                {
                    InterfaceVisibility = Visibility.Visible;
                    ProductsCount = 0;
                    ProductsSummary = 0;

                    ProgressRingVisibility = Visibility.Visible;

                    _dbContext = await Task.Run(() => new StoreDbContext());

                    _mainWindowViewModel.IsCartFilled = true;
                    ProductsCount = _mainWindowViewModel.BadgeValue;

                    CartItems = await Task.Run(() => new ObservableCollection<ClothesContain>(_dbContext.ClothesContain.Include(x => x.Clothes).Include(x => x.Cart).Where(x => x.Cart.UserID == _mainWindowViewModel.UserID)));

                    foreach (ClothesContain item in CartItems)
                        ProductsSummary += item.Count * item.Clothes.ClothesPrice;

                    GetNumEnding(ProductsCount);

                    ProgressRingVisibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }          
        }

        [RelayCommand]
        private void GoToMenu()
        {
            navService = App.GetService<INavigationService>();
            navService.Navigate(typeof(Views.Pages.MenuPage));
        }

        [RelayCommand]
        private async void Checkout(Snackbar snackbar)
        {
            SnackBar = snackbar;

            if (PayNowCheckBoxIsChecked && CardMethodIsChecked)
            {
                navService = App.GetService<INavigationService>();
                navService.Navigate(typeof(Views.Pages.CardDataPage));
            }
            else
            {
                try
                {
                    //Создание заказа
                    Order orderObj = new Order
                    {
                        UserID = _mainWindowViewModel.UserID,
                        OrderDate = DateTime.Now,
                        OrderStatus = "Подтвержден",
                        OrderTotal = ProductsSummary,
                        IsPaid = false,
                        PaymentMethod = CardMethodIsChecked ? "Карта" : "Наличные"
                    };
                    await _dbContext.Order.AddAsync(orderObj);
                    await _dbContext.SaveChangesAsync();

                    int OrderObjID = orderObj.OrderID;

                    //Добавление товаров из корзины в контейнер заказа и очистка корзины
                    foreach (ClothesContain item in CartItems)
                    {
                        await _dbContext.OrderContain.AddAsync(new OrderContain()
                        {
                            OrderID = OrderObjID,
                            ClothesID = item.ClothesID,
                            Count = item.Count
                        });
                    }

                    //Удаление элементов корзины
                    await Task.Run(() => _dbContext.ClothesContain.RemoveRange(CartItems));
                    await _dbContext.SaveChangesAsync();

                    _mainWindowViewModel.BadgeValue = 0;
                    InitializeViewModel();

                    SnackBar.Show();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }     
            }
        }

        [RelayCommand]
        private async void DecrementCount(int clothesContainId)
        {
            try
            {
                ButtonsCountIsEnabled = false;
                ClothesContain ClothesContainModel = await Task.Run(() => _dbContext.ClothesContain.FirstOrDefault(x => x.ClothesContainID == clothesContainId));

                if (ClothesContainModel.Count == 1)
                {
                    ButtonsCountIsEnabled = true;
                    return;
                }
                else
                {
                    ClothesContainModel.Count--;

                    _mainWindowViewModel.BadgeValue--;
                    ProductsCount--;

                    await _dbContext.SaveChangesAsync();
                    InitializeViewModel();
                }
                ButtonsCountIsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
           
        }

        [RelayCommand]
        private async void DeleteItem(int clothesContainId)
        {
            try
            {
                DeleteBtnIsEnabled = false;

                ClothesContain clothesContainModel = await Task.Run(() => _dbContext.ClothesContain.FirstOrDefault(x => x.ClothesContainID == clothesContainId));

                _mainWindowViewModel.BadgeValue -= clothesContainModel.Count;
                ProductsCount -= clothesContainModel.Count;

                _dbContext.ClothesContain.Remove(clothesContainModel);
                await _dbContext.SaveChangesAsync();

                InitializeViewModel();

                DeleteBtnIsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        [RelayCommand]
        private async void IncrementCount(int clothesContainId)
        {
            try
            {
                ButtonsCountIsEnabled = false;
                ClothesContain clothesContainModel = await Task.Run(() => _dbContext.ClothesContain.FirstOrDefault(x => x.ClothesContainID == clothesContainId));

                if (clothesContainModel.Count == 5)
                {
                    ButtonsCountIsEnabled = true;
                    return;
                }
                else
                {
                    clothesContainModel.Count++;

                    _mainWindowViewModel.BadgeValue++;
                    ProductsCount++;

                    await _dbContext.SaveChangesAsync();
                    InitializeViewModel();
                }
                ButtonsCountIsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }
            
        }

        public void GetNumEnding(int iNumber)
        {
            iNumber = iNumber % 100;
            if (iNumber >= 11 && iNumber <= 19)
                CountText = "товаров";
            else
            {
                var i = iNumber % 10;
                switch (i)
                {
                    case (1):
                        CountText = "товар";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        CountText = "товара";
                        break;
                    default:
                        CountText = "товаров";
                        break;
                }
            }
        }
    }
}
