using Microsoft.EntityFrameworkCore;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using OptimistShop.ViewModels;
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
using System.Xml;
using Wpf.Ui.Common.Interfaces;

namespace OptimistShop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : INavigableView<ViewModels.HomePageViewModel>
    {
        public ViewModels.HomePageViewModel ViewModel
        {
            get;
        }
        public HomePage(HomePageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
