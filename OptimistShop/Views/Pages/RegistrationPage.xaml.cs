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
using Wpf.Ui.Mvvm.Interfaces;

namespace OptimistShop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : INavigableView<ViewModels.RegistrationPageViewModel>
    {
        public ViewModels.RegistrationPageViewModel ViewModel
        {
            get;
        }

        public RegistrationPage(ViewModels.RegistrationPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            { ViewModel.UserPassword = PasswordPB.Password; }
        }
    }
}
