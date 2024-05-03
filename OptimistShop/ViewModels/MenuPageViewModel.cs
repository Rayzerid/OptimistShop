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
        private LoginPageViewModel _loginViewModel;
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
        private int _indexCategory;

        [ObservableProperty]
        private int _indexType;

        [ObservableProperty]
        private string _selectedCategory;

        [ObservableProperty]
        private string _selectedType;

        [ObservableProperty]
        private string _snackbarMessage;

        [ObservableProperty]
        private ObservableCollection<string> _clothesType;

        [ObservableProperty]
        private ObservableCollection<string> _clothesCategory;

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
                _loginViewModel = App.GetService<LoginPageViewModel>();

                _dbContext = await Task.Run(() => new StoreDbContext());

                CatalogItemsMain = await Task.Run(() => new ObservableCollection<Clothes>(_dbContext.Clothes.ToList()));

                ClothesCategory = new ObservableCollection<string>()
                {
                    "Верхняя одежда",
                    "Нижняя одежда",
                    "Обувь",
                    "Головные уборы"
                };

                ClothesType = new ObservableCollection<string>()
                {
                    "Мужская",
                    "Женская"
                };
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
        private async void ToCartClick(int? parameter)
        {
            try
            {
                if (_mainWindowViewModel.IsUserAuthorized)
                {
                    BtnAddIsHitTestVisible = false;
                    SnackbarMessage = "Товар успешно добавлен в корзину";
                    SnackbarAppearance = "Dark";

                    Clothes SelectedClothesModel = CatalogItemsSecondary.FirstOrDefault(x => x.ClothesID == parameter);

                    //Создание корзины, если ее нет
                    if (await Task.Run(() => _dbContext.Cart.FirstOrDefault(x => x.UserID == _loginViewModel.UserID)) == null)
                    {
                        await _dbContext.Cart.AddAsync(new Cart()
                        {
                            UserID = _loginViewModel.UserID
                        });

                        await _dbContext.SaveChangesAsync();
                    }

                    //Получение корзины пользователя
                    Cart CartModel = await Task.Run(() => _dbContext.Cart.FirstOrDefault(x => x.UserID == _loginViewModel.UserID));

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
                            SnackbarAppearance = "Dark";
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
                    SnackbarAppearance = "Dark";
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
        private void CategoryFilter()
        {
            try
            {
                if (SelectedCategory != string.Empty && SelectedType != string.Empty)
                {
                    ProgressRingVisibility = Visibility.Visible;

                    CatalogItemsSecondary = new ObservableCollection<Clothes>(_dbContext.Clothes.Where(x => x.ClothesCategory == SelectedCategory && x.ClothesType == ParseType(SelectedType)).ToList());

                    ProgressRingVisibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async void ClearFillters()
        {
            CatalogItemsSecondary = new ObservableCollection<Clothes>(_dbContext.Clothes.ToList());
            IndexCategory = -1;
            IndexType = -1;
        }

        

        private string ParseType(string category)
        {
            string result = string.Empty;
            if (category == "Мужская")
                result = "Муж";
            else
            if (category == "Женская")
                result = "Жен";
            else
                result = category;

            return result;
        }
    }
}
