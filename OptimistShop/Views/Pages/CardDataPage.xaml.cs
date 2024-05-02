using OptimistShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
using Wpf.Ui.Mvvm.Interfaces;

namespace OptimistShop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для CardDataPage.xaml
    /// </summary>
    public partial class CardDataPage : INavigableView<ViewModels.CardDataPageViewModel>
    {
        public ViewModels.CardDataPageViewModel ViewModel
        {
            get;
        }
        public CardDataPage(ViewModels.CardDataPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
        private void CheckNums(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.Match(e.Text.ToString(), @"[0-9]").Success)
                e.Handled = true;
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            { ViewModel.Cvv = CvvPB.Password; }
        }
    }
}
