using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : INavigableView<ViewModels.CartPageViewModel>
    {
        public ViewModels.CartPageViewModel ViewModel
        {
            get;
        }
        public CartPage(ViewModels.CartPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
