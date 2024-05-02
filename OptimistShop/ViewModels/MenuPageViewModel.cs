using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptimistShop.Core;
using OptimistShop.Models.DbTables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Common.Interfaces;

namespace OptimistShop.ViewModels
{
    public partial class MenuPageViewModel : ObservableObject, INavigationAware
    {
        private StoreDbContext _dbContext;
        private MainWindowViewModel _mainWindowViewModel;
        private bool _isInitialized = false;

        [ObservableProperty]
        private ObservableCollection<Clothes> _catalogItemsMain;

        [ObservableProperty]
        private ObservableCollection<Clothes> _catalogItemsSecondary;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MenuVisibility), nameof(InterfaceIsHitTestVisible))]
        private Visibility _progressRingVisibility = Visibility.Visible;

        [ObservableProperty]
        private string? _searchText;

        [ObservableProperty]
        private string _snackbarMessage;

        [ObservableProperty]
        private string _snackbarAppearance;

        [ObservableProperty]
        private bool _interfaceIsEnabled = false;

        [ObservableProperty]
        private bool _btnAddIsHitTestVisible = true;
        public Visibility MenuVisibility => ProgressRingVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        public bool InterfaceIsHitTestVisible => ProgressRingVisibility == Visibility.Visible ? false : true;

        public void OnNavigatedFrom()
        {
        }
        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }
        private async void InitializeViewModel()
        {
            try
            {
                _mainWindowViewModel = App.GetService<MainWindowViewModel>();

                _dbContext = await Task.Run(() => new StoreDbContext());

                CatalogItemsMain = await Task.Run(() => new ObservableCollection<Clothes>(_dbContext.Clothes.ToList()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CatalogItemsSecondary = await Task.Run(()=> CatalogItemsMain);

            _isInitialized = true;
            ProgressRingVisibility = Visibility.Hidden;
            InterfaceIsEnabled = true;
        }


        [RelayCommand]
        private async void TextChanged()
        {
            ProgressRingVisibility = Visibility.Visible;

            CatalogItemsSecondary = await Task.Run(() => new ObservableCollection<Clothes>(CatalogItemsMain.Where(u => u.ClothesName.ToLower().Contains(SearchText.ToLower())).ToList()));

            ProgressRingVisibility = Visibility.Hidden;
        }


        [RelayCommand]
        private async void ToCartClick(string? parameter)
        {
            try
            {
                if (_mainWindowViewModel.IsUserAuthorized)
                {
                    BtnAddIsHitTestVisible = false;
                    SnackbarMessage = "Товар успешно добавлен в корзину";
                    SnackbarAppearance = "Success";

                    Clothes SelectedClothesModel = CatalogItemsSecondary.FirstOrDefault(x => x.ClothesName == parameter);

                    //Создание корзины, если ее нет
                    if (await Task.Run(() => _dbContext.Cart.FirstOrDefault(x => x.UserID == _mainWindowViewModel.UserID)) == null)
                    {
                        await _dbContext.Cart.AddAsync(new Cart()
                        {
                            UserID = _mainWindowViewModel.UserID
                        });

                        await _dbContext.SaveChangesAsync();
                    }

                    //Получение корзины пользователя
                    Cart CartModel = await Task.Run(() => _dbContext.Cart.FirstOrDefault(x => x.UserID == _mainWindowViewModel.UserID));

                    //Получение контейнера еды для корзины пользователя
                    ClothesContain foodContain = await Task.Run(() => _dbContext.ClothesContain.FirstOrDefault(x => x.ClothesID == SelectedClothesModel.ClothesID && x.CartID == CartModel.CartID));

                    //Инкремент количества выбранной еды, если она уже добавлена, если нет то добавление
                    if (foodContain == null)
                    {
                        await _dbContext.ClothesContain.AddAsync(new ClothesContain()
                        {
                            CartID = CartModel.CartID,
                            ClothesID = SelectedClothesModel.ClothesID,
                            Count = 1
                        });
                        _mainWindowViewModel.BadgeValue++;
                    }
                    else
                    {
                        if(foodContain.Count == 5)
                        {
                            SnackbarMessage = "Достигнут лимит количества в корзине для этого товара";
                            SnackbarAppearance = "Danger";
                        }
                        else
                        {
                            foodContain.Count++;
                            _mainWindowViewModel.BadgeValue++;
                        }      
                    }

                    await _dbContext.SaveChangesAsync();

                    BtnAddIsHitTestVisible = true;
                }
                else
                {
                    SnackbarMessage = "Войдите в аккаунт";
                    SnackbarAppearance = "Danger";
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }


        [RelayCommand]
        private async void CategoryFilterButtonClick(string? parameter)
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;
                CatalogItemsSecondary = await Task.Run(() => new ObservableCollection<Clothes>(_dbContext.Clothes.Where(x => x.ClothesCategory == parameter)));
                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        [RelayCommand]
        private async void StatusFilterButtonClick(string? parameter)
        {
            try
            {
                /*ProgressRingVisibility = Visibility.Visible;
                MenuItemsSecondary = await Task.Run(() => new ObservableCollection<Clothes>(_dbContext.Clothes.Where(x => x.ClothesStatus == parameter)));
                ProgressRingVisibility = Visibility.Hidden;*/
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        [RelayCommand]
        private async void PriceFilterButtonClick(string? parameter)
        {
            try
            {
                parameter = parameter.Replace(" ₽", "");
                string[] strSplit = parameter.Split(" - ");

                ProgressRingVisibility = Visibility.Visible;
                CatalogItemsSecondary = await Task.Run(() => new ObservableCollection<Clothes>(_dbContext.Clothes.Where(x => x.ClothesPrice >= int.Parse(strSplit[0]) && x.ClothesPrice <= int.Parse(strSplit[1]))));
                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }      
        }

        [RelayCommand]
        private async void TypeFilterButtonClick(string? parameter)
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;
                CatalogItemsSecondary = await Task.Run(() => new ObservableCollection<Clothes>(_dbContext.Clothes.Where(x => x.ClothesType == parameter)));
                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        [RelayCommand]
        private async void ClearFilters(string? parameter)
        {
            try
            {
                ProgressRingVisibility = Visibility.Visible;
                CatalogItemsSecondary = await Task.Run(() => new ObservableCollection<Clothes>(_dbContext.Clothes));
                ProgressRingVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }
    }
}
