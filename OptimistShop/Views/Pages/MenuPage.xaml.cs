using Microsoft.Win32;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using OptimistShop.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Common.Interfaces;

namespace OptimistShop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MenuPage.xaml
    /// </summary>
    public partial class MenuPage : INavigableView<ViewModels.MenuPageViewModel>
    {
        private MainWindowViewModel? _mainWindowViewModel;
        public ViewModels.MenuPageViewModel ViewModel
        {
            get;
        }
        public MenuPage(MenuPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            _mainWindowViewModel = App.GetService<MainWindowViewModel>();
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            AddSnackbar.Show();
        }
    }
}
