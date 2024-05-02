using System;
using System.Collections.Generic;
using System.Linq;
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

namespace OptimistShop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : INavigableView<ViewModels.EmployeePageViewModel>
    {
        public ViewModels.EmployeePageViewModel ViewModel
        {
            get;
        }

        private void CheckNums(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.Match(e.Text.ToString(), @"[0-9]").Success)
                e.Handled = true;
        }
        public EmployeePage(ViewModels.EmployeePageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
