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
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace OptimistShop.ViewModels
{
    public partial class OrderDetailsPageViewModel : ObservableObject, INavigationAware
    {
        private StoreDbContext _dbContext;
        private INavigationService? navService;

        [ObservableProperty]
        private ObservableCollection<OrderContain> _orderDetailItems;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DetailsVisibility))]
        private Visibility _progressRingVisibility = Visibility.Visible;

        [ObservableProperty]
        private int _orderID;
        public Visibility DetailsVisibility => ProgressRingVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

        private async void InitializeViewModel()
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;

                _dbContext = await Task.Run(() => new StoreDbContext());
                OrderDetailItems = await Task.Run(() => new ObservableCollection<OrderContain>(_dbContext.OrderContain.Include(x => x.Clothes).Where(x => x.OrderID == OrderID)));

                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        [RelayCommand]
        private void GoToOrders()
        {
            navService = App.GetService<INavigationService>();
            navService.Navigate(typeof(Views.Pages.OrdersPage));
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
