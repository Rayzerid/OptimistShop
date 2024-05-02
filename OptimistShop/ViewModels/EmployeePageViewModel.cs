using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace OptimistShop.ViewModels
{
    public partial class EmployeePageViewModel : ObservableObject, INavigationAware
    {
        private INavigationService? navService;
        private StoreDbContext _dbContext;
        private MainWindowViewModel _mainWindowViewModel;

        [ObservableProperty]
        private string _userIDText = string.Empty;

        [ObservableProperty]
        private string _emptyText = "Заказов пока что нет";

        [ObservableProperty]
        private ObservableCollection<Order> _orderItems;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InterfaceIsEnabled))]
        private Visibility _progressRingVisibility = Visibility.Visible;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(InterfaceIsEnabled))]
        private Visibility _emptyOrderVisibility = Visibility.Hidden;

        public bool InterfaceIsEnabled => ProgressRingVisibility == Visibility.Visible ? false : true;

        private async void InitializeViewModel()
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;

                _mainWindowViewModel = await Task.Run(() => App.GetService<MainWindowViewModel>());
                _dbContext = await Task.Run(() => new StoreDbContext());

                OrderItems = await Task.Run(() => new ObservableCollection<Order>(_dbContext.Order.Include(x => x.User).Where(x => x.OrderStatus != "Получен")));


                if (OrderItems.Count == 0)
                {
                    EmptyText = "Заказов пока что нет";
                    EmptyOrderVisibility = Visibility.Visible;
                }
                else
                    EmptyOrderVisibility = Visibility.Hidden;

                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        [RelayCommand]
        private async void IsPaidChanged(object parameter)
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;

                var values = (object[])parameter;

                int OrderID = (int)values[0];
                ComboBoxItem SelectedItem = (ComboBoxItem)values[1];
                string value = SelectedItem.Content.ToString();

                Order orderModel = await Task.Run(() => _dbContext.Order.FirstOrDefault(x => x.OrderID == OrderID));

                if (value == "Нет")
                    orderModel.IsPaid = false;
                else
                    orderModel.IsPaid = true;

                await _dbContext.SaveChangesAsync();
                InitializeViewModel();
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); 
                return; 
            }
            
        }

        [RelayCommand]
        private async void StatusChanged(object parameter)
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;

                var values = (object[])parameter;

                int OrderID = (int)values[0];
                ComboBoxItem SelectedItem = (ComboBoxItem)values[1];
                string status = SelectedItem.Content.ToString();

                Order orderModel = await Task.Run(() => _dbContext.Order.FirstOrDefault(x => x.OrderID == OrderID));
                if (status == "Получен")
                {
                    orderModel.OrderStatus = status;
                    orderModel.IsPaid = true;
                }
                else
                    orderModel.OrderStatus = status;

                await _dbContext.SaveChangesAsync();
                InitializeViewModel();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        [RelayCommand]
        private async void GetAllOrders()
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;

                OrderItems = await Task.Run(() => new ObservableCollection<Order>(_dbContext.Order.Include(x => x.User).Where(x => x.OrderStatus != "Получен")));

                if (OrderItems.Count == 0)
                {
                    EmptyText = "Заказов пока что нет";
                    EmptyOrderVisibility = Visibility.Visible;
                }
                else
                    EmptyOrderVisibility = Visibility.Hidden;

                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        [RelayCommand]
        private async void GetTodayOrders()
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;

                OrderItems = await Task.Run(() => new ObservableCollection<Order>(_dbContext.Order.Include(x => x.User).Where(x => x.OrderStatus != "Получен" && x.OrderDate == DateTime.Today)));

                if (OrderItems.Count == 0)
                {
                    EmptyText = "Сегодня пока нет заказов";
                    EmptyOrderVisibility = Visibility.Visible;
                }
                else
                    EmptyOrderVisibility = Visibility.Hidden;

                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        [RelayCommand]
        private async void IDTextChanged()
        {
            try
            {
                if (Regex.Match(UserIDText, @"[0-9]").Success)
                {
                    ProgressRingVisibility = Visibility.Visible;

                    OrderItems = await Task.Run(() => new ObservableCollection<Order>(_dbContext.Order.Include(x => x.User).Where(x => x.OrderStatus != "Получен")));
                    OrderItems = await Task.Run(() => new ObservableCollection<Order>(OrderItems.Where(x => x.User.UserID == Convert.ToInt32(UserIDText))));

                    ProgressRingVisibility = Visibility.Hidden;
                }
                else if (UserIDText == string.Empty)
                {
                    ProgressRingVisibility = Visibility.Visible;

                    OrderItems = await Task.Run(() => new ObservableCollection<Order>(_dbContext.Order.Include(x => x.User).Where(x => x.OrderStatus != "Получен")));

                    ProgressRingVisibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }   
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
            InitializeViewModel();
        }
    }
}
