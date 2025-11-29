using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

using Avalonia.Controls;
using Avalonia.Interactivity;

using ObjectOrientedPractics.Model;

namespace ObjectOrientedPractics.View.Tabs
{
    /// <summary>
    /// Вкладка для работы с покупателями.
    /// </summary>
    public partial class CustomersTab : UserControl
    {
        /// <summary>
        /// Коллекция покупателей.
        /// </summary>
        private ObservableCollection<Customer>? _customers;

        /// <summary>
        /// Текущий выбранный покупатель.
        /// </summary>
        private Customer? _selected;

        /// <summary>
        /// Событие, возникающее при изменении данных покупателей
        /// (добавление, удаление, изменение свойств).
        /// </summary>
        public event Action? CustomersChanged;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CustomersTab"/>.
        /// </summary>
        public CustomersTab()
        {
            InitializeComponent();

            CustomersList.SelectionChanged += (_, __) =>
            {
                _selected = CustomersList.SelectedItem as Customer;
                LoadToForm(_selected);
            };

            FullnameBox.PropertyChanged += (_, a) =>
            {
                if (a.Property == TextBox.TextProperty)
                    ValidateAll();
            };
            PostIndexBox.PropertyChanged += (_, a) =>
            {
                if (a.Property == TextBox.TextProperty)
                    ValidateAll();
            };
            CountryBox.PropertyChanged += (_, a) =>
            {
                if (a.Property == TextBox.TextProperty)
                    ValidateAll();
            };
            CityBox.PropertyChanged += (_, a) =>
            {
                if (a.Property == TextBox.TextProperty)
                    ValidateAll();
            };
            StreetBox.PropertyChanged += (_, a) =>
            {
                if (a.Property == TextBox.TextProperty)
                    ValidateAll();
            };
            BuildingBox.PropertyChanged += (_, a) =>
            {
                if (a.Property == TextBox.TextProperty)
                    ValidateAll();
            };
            ApartmentBox.PropertyChanged += (_, a) =>
            {
                if (a.Property == TextBox.TextProperty)
                    ValidateAll();
            };

            AddBtn.Click    += OnAdd;
            RemoveBtn.Click += OnRemove;
            SaveBtn.Click   += OnSave;
        }

        /// <summary>
        /// Внешняя коллекция покупателей (назначить до использования).
        /// </summary>
        public ObservableCollection<Customer> Customers
        {
            get => _customers ?? throw new InvalidOperationException(
                "Коллекция покупателей не установлена. Назначьте CustomersTab.Customers = store.Customers перед использованием.");
            set
            {
                if (_customers != null)
                {
                    _customers.CollectionChanged -= OnCustomersCollectionChanged;
                    UnsubscribeCustomers(_customers);
                }

                _customers = value ?? throw new ArgumentNullException(nameof(value));
                CustomersList.ItemsSource = _customers;

                _customers.CollectionChanged += OnCustomersCollectionChanged;
                SubscribeCustomers(_customers);
            }
        }

        /// <summary>
        /// Обработчик изменения коллекции покупателей.
        /// </summary>
        private void OnCustomersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Customer c in e.OldItems)
                    c.PropertyChanged -= OnCustomerPropertyChanged;
            }

            if (e.NewItems != null)
            {
                foreach (Customer c in e.NewItems)
                    c.PropertyChanged += OnCustomerPropertyChanged;
            }

            CustomersChanged?.Invoke();
        }

        /// <summary>
        /// Обработчик изменения свойств покупателя.
        /// </summary>
        private void OnCustomerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            CustomersChanged?.Invoke();
        }

        /// <summary>
        /// Подписка на события PropertyChanged для всех покупателей.
        /// </summary>
        private void SubscribeCustomers(ObservableCollection<Customer> customers)
        {
            foreach (var c in customers)
                c.PropertyChanged += OnCustomerPropertyChanged;
        }

        /// <summary>
        /// Отписка от событий PropertyChanged для всех покупателей.
        /// </summary>
        private void UnsubscribeCustomers(ObservableCollection<Customer> customers)
        {
            foreach (var c in customers)
                c.PropertyChanged -= OnCustomerPropertyChanged;
        }

        /// <summary>
        /// Загрузка данных покупателя в форму.
        /// </summary>
        private void LoadToForm(Customer? c)
        {
            ErrorText.Text = string.Empty;

            IdBox.Text        = c?.Id.ToString() ?? string.Empty;
            FullnameBox.Text  = c?.Fullname ?? string.Empty;

            var a = c?.Address;
            PostIndexBox.Text = a?.Index.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            CountryBox.Text   = a?.Country ?? string.Empty;
            CityBox.Text      = a?.City ?? string.Empty;
            StreetBox.Text    = a?.Street ?? string.Empty;
            BuildingBox.Text  = a?.Building ?? string.Empty;
            ApartmentBox.Text = a?.Apartment ?? string.Empty;
        }

        /// <summary>
        /// Добавление нового покупателя.
        /// </summary>
        private void OnAdd(object? sender, RoutedEventArgs e)
        {
            var c = new Customer("New Customer");
            _customers!.Add(c);
            CustomersList.SelectedItem = c;

            CustomersChanged?.Invoke();
        }

        /// <summary>
        /// Удаление выбранного покупателя.
        /// </summary>
        private void OnRemove(object? sender, RoutedEventArgs e)
        {
            if (_selected is null)
                return;

            _customers!.Remove(_selected);
            _selected = null;
            LoadToForm(null);

            CustomersChanged?.Invoke();
        }

        /// <summary>
        /// Сохранение данных выбранного покупателя.
        /// </summary>
        private void OnSave(object? sender, RoutedEventArgs e)
        {
            if (_selected is null)
                return;

            if (!ValidateAll())
                return;

            try
            {
                _selected.Fullname = FullnameBox.Text?.Trim() ?? string.Empty;

                var address = _selected.Address;

                if (!int.TryParse(
                        PostIndexBox.Text,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out var index))
                {
                    throw new ArgumentException("Индекс: введите число.");
                }

                // Нормальный диапазон: 100000–999999
                if (index < 100000 || index > 999999)
                    throw new ArgumentException("Индекс: число от 100000 до 999999.");

                address.Index     = index;
                address.Country   = CountryBox.Text?.Trim() ?? string.Empty;
                address.City      = CityBox.Text?.Trim() ?? string.Empty;
                address.Street    = StreetBox.Text?.Trim() ?? string.Empty;
                address.Building  = BuildingBox.Text?.Trim() ?? string.Empty;
                address.Apartment = ApartmentBox.Text?.Trim() ?? string.Empty;

                var idx = CustomersList.SelectedIndex;
                CustomersList.ItemsSource = null;
                CustomersList.ItemsSource = _customers;
                CustomersList.SelectedIndex = idx;

                ErrorText.Text = "Сохранено";
                CustomersChanged?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }

        // ===== Валидация =====

        /// <summary>
        /// Проверяет все поля формы и выводит текст ошибки.
        /// </summary>
        private bool ValidateAll()
        {
            var msg = string.Empty;

            if (!ValidateFullname(out var fullErr))
                msg += fullErr + Environment.NewLine;

            if (!ValidatePostIndex(out var idxErr))
                msg += idxErr + Environment.NewLine;

            if (!ValidateRequiredLen(CountryBox.Text, 100, "Страна", out var countryErr))
                msg += countryErr + Environment.NewLine;

            if (!ValidateRequiredLen(CityBox.Text, 100, "Город", out var cityErr))
                msg += cityErr + Environment.NewLine;

            if (!ValidateRequiredLen(StreetBox.Text, 200, "Улица", out var streetErr))
                msg += streetErr + Environment.NewLine;

            if (!ValidateRequiredLen(BuildingBox.Text, 50, "Дом", out var buildErr))
                msg += buildErr + Environment.NewLine;

            if (!ValidateRequiredLen(ApartmentBox.Text, 50, "Квартира", out var aptErr))
                msg += aptErr + Environment.NewLine;

            ErrorText.Text = msg.TrimEnd();
            return string.IsNullOrEmpty(msg);
        }

        /// <summary>
        /// Валидация ФИО.
        /// </summary>
        private bool ValidateFullname(out string error)
        {
            var s = FullnameBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(s))
            {
                error = "ФИО не может быть пустым.";
                return false;
            }

            if (s.Length > 200)
            {
                error = "ФИО: максимум 200 символов.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        /// <summary>
        /// Валидация индекса.
        /// </summary>
        private bool ValidatePostIndex(out string error)
        {
            var s = PostIndexBox.Text?.Trim() ?? string.Empty;

            if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var idx))
            {
                error = "Индекс должен быть числом.";
                return false;
            }

            if (idx < 100000 || idx > 999999)
            {
                error = "Индекс: число от 100000 до 999999.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        /// <summary>
        /// Валидация обязательного строкового поля с ограничением длины.
        /// </summary>
        private static bool ValidateRequiredLen(string? s, int max, string field, out string error)
        {
            s = s?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(s))
            {
                error = $"{field} не может быть пустым.";
                return false;
            }

            if (s.Length > max)
            {
                error = $"{field}: максимум {max} символов.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}