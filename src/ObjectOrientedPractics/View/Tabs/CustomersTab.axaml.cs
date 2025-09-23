using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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
        private readonly ObservableCollection<Customer> _customers = new();

        /// <summary>
        /// Выбранный покупатель.
        /// </summary>
        private Customer? _selected;

        /// <summary>
        /// Инициализация вкладки CustomersTab.
        /// </summary>
        public CustomersTab()
        {
            InitializeComponent();
            CustomersList.ItemsSource = _customers;
            CustomersList.SelectionChanged += (_, __) =>
            {
                _selected = CustomersList.SelectedItem as Customer;
                LoadToForm(_selected);
            };
            AddBtn.Click += OnAdd;
            RemoveBtn.Click += OnRemove;
            SaveBtn.Click += OnSave;
        }

        /// <summary>
        /// Загрузка данных покупателя в форму.
        /// </summary>
        private void LoadToForm(Customer? c)
        {
            ErrorText.Text = "";
            IdBox.Text = c?.Id.ToString() ?? "";
            FullnameBox.Text = c?.Fullname ?? "";
            var a = c?.Address;
            PostIndexBox.Text = a?.Index.ToString() ?? "";
            CountryBox.Text = a?.Country ?? "";
            CityBox.Text = a?.City ?? "";
            StreetBox.Text = a?.Street ?? "";
            BuildingBox.Text = a?.Building ?? "";
            ApartmentBox.Text = a?.Apartment ?? "";
        }

        /// <summary>
        /// Добавление нового покупателя.
        /// </summary>
        private void OnAdd(object? sender, RoutedEventArgs e)
        {
            var c = new Customer("New Customer", new Address());
            _customers.Add(c);
            CustomersList.SelectedItem = c;
        }

        /// <summary>
        /// Удаление выбранного покупателя.
        /// </summary>
        private void OnRemove(object? sender, RoutedEventArgs e)
        {
            if (_selected is null) return;
            _customers.Remove(_selected);
            _selected = null;
            LoadToForm(null);
        }

        /// <summary>
        /// Сохранение данных выбранного покупателя.
        /// </summary>
        private void OnSave(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (_selected is null) return;
                _selected.Fullname = FullnameBox.Text ?? "";
                var address = _selected.Address ?? new Address();
                if (!int.TryParse(PostIndexBox.Text, out var index))
                {
                    throw new ArgumentException("Индекс должен быть числом");
                }
                address.Index = index;
                address.Country = CountryBox.Text ?? "";
                address.City = CityBox.Text ?? "";
                address.Street = StreetBox.Text ?? "";
                address.Building = BuildingBox.Text ?? "";
                address.Apartment = ApartmentBox.Text ?? "";
                _selected.Address = address;
                var idx = CustomersList.SelectedIndex;
                CustomersList.ItemsSource = null;
                CustomersList.ItemsSource = _customers;
                CustomersList.SelectedIndex = idx;
                ErrorText.Text = "Сохранено";
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }
    }
}