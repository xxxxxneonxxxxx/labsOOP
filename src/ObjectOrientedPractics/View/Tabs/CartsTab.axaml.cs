using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ObjectOrientedPractics.Model;
using ObjectOrientedPractics.Services;

namespace ObjectOrientedPractics.View.Tabs
{
    /// <summary>
    /// Вкладка с корзинами покупателей.
    /// Позволяет выбирать покупателя, управлять его корзиной и оформлять заказ.
    /// </summary>
    public partial class CartsTab : UserControl
    {
        /// <summary>
        /// Коллекция всех товаров магазина.
        /// </summary>
        private ObservableCollection<Item> _items        = new();

        /// <summary>
        /// Коллекция всех покупателей магазина.
        /// </summary>
        private ObservableCollection<Customer> _customers = new();

        /// <summary>
        /// Текущий выбранный покупатель (для которого показываем корзину).
        /// </summary>
        private Customer? _currentCustomer;

        /// <summary>
        /// Событие: создан новый заказ из корзины.
        /// Подписка в MainWindow: CartsTab.OrderCreated += (_,__) => OrdersTab.RefreshData();
        /// </summary>
        public event EventHandler? OrderCreated;

        /// <summary>
        /// Все товары магазина, используемые во вкладке.
        /// При установке переназначает источник данных списка товаров
        /// и подписывается на изменения коллекции и свойств элементов.
        /// </summary>
        public ObservableCollection<Item> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                {
                    _items.CollectionChanged -= OnItemsCollectionChanged;
                    UnsubscribeItems(_items);
                }

                _items = value ?? new ObservableCollection<Item>();

                _items.CollectionChanged += OnItemsCollectionChanged;
                SubscribeItems(_items);

                ItemsListBox.ItemsSource = _items;
            }
        }

        /// <summary>
        /// Все покупатели магазина, используемые во вкладке.
        /// При установке переназначает источник данных комбобокса
        /// и подписывается на изменения коллекции и самих покупателей.
        /// </summary>
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                if (_customers != null)
                {
                    _customers.CollectionChanged -= OnCustomersCollectionChanged;
                    UnsubscribeCustomers(_customers);
                }

                _customers = value ?? new ObservableCollection<Customer>();

                _customers.CollectionChanged += OnCustomersCollectionChanged;
                SubscribeCustomers(_customers);

                CustomersComboBox.ItemsSource = _customers;

                if (_customers.Count > 0 && CustomersComboBox.SelectedItem is null)
                {
                    CustomersComboBox.SelectedIndex = 0;
                    _currentCustomer = CustomersComboBox.SelectedItem as Customer;
                    RefreshCartView();
                }
            }
        }

        /// <summary>
        /// Инициализация вкладки корзин.
        /// Привязка обработчиков событий UI.
        /// </summary>
        public CartsTab()
        {
            InitializeComponent();

            CustomersComboBox.SelectionChanged += CustomersComboBox_OnSelectionChanged;

            AddToCartButton.Click   += AddToCartButton_OnClick;
            RemoveItemButton.Click  += RemoveItemButton_OnClick;
            ClearCartButton.Click   += ClearCartButton_OnClick;
            CreateOrderButton.Click += CreateOrderButton_OnClick;
        }

        // ================== Обработчики UI ==================

        /// <summary>
        /// Обработка смены выбранного покупателя в комбобоксе.
        /// </summary>
        private void CustomersComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            _currentCustomer = CustomersComboBox.SelectedItem as Customer;
            RefreshCartView();
        }

        /// <summary>
        /// Добавление выбранного товара из списка товаров в корзину текущего покупателя.
        /// </summary>
        private void AddToCartButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentCustomer is null) return;
            if (ItemsListBox.SelectedItem is not Item item) return;

            _currentCustomer.Cart.Add(item);
            RefreshCartView();
        }

        /// <summary>
        /// Удаление выбранного товара из корзины текущего покупателя.
        /// </summary>
        private void RemoveItemButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentCustomer is null) return;
            if (CartItemsListBox.SelectedItem is not Item item) return;

            // удаляем ровно один выбранный товар
            _currentCustomer.Cart.Remove(item);
            RefreshCartView();
        }

        /// <summary>
        /// Полная очистка корзины текущего покупателя.
        /// </summary>
        private void ClearCartButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentCustomer is null) return;

            _currentCustomer.Cart.Clear();
            RefreshCartView();
        }

        /// <summary>
        /// Создание заказа из текущей корзины и её очистка.
        /// Генерирует событие <see cref="OrderCreated"/>.
        /// </summary>
        private void CreateOrderButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentCustomer is null) return;
            if (_currentCustomer.Cart.Items.Count == 0) return;

            var order = new Order(
                IdGenerator.GetNextId(),
                _currentCustomer.Address,
                _currentCustomer.Cart.Items
            );

            _currentCustomer.Orders.Add(order);
            _currentCustomer.Cart.Clear();
            UpdateAmount();

            OrderCreated?.Invoke(this, EventArgs.Empty);
        }

        // ================== События коллекций / моделей ==================

        /// <summary>
        /// Обработчик изменения коллекции товаров (добавление/удаление).
        /// Перекидывает подписки на новые элементы и обновляет UI.
        /// </summary>
        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Item it in e.OldItems)
                    it.PropertyChanged -= OnItemPropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (Item it in e.NewItems)
                    it.PropertyChanged += OnItemPropertyChanged;
            }

            // Перерисовать список товаров
            ItemsListBox.ItemsSource = null;
            ItemsListBox.ItemsSource = _items;

            RefreshCartView();
        }

        /// <summary>
        /// Обработчик изменения свойств товара (например, Name или Cost).
        /// Обновляет список товаров и корзину.
        /// </summary>
        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Item.Cost) ||
                e.PropertyName == nameof(Item.Name))
            {
                // Обновляем список товаров + корзину (если этот товар в корзине)
                ItemsListBox.ItemsSource = null;
                ItemsListBox.ItemsSource = _items;
                RefreshCartView();
            }
        }

        /// <summary>
        /// Обработчик изменения коллекции покупателей.
        /// Переназначает подписки и обновляет комбобокс.
        /// </summary>
        private void OnCustomersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Customer c in e.OldItems)
                    UnsubscribeCustomer(c);
            }
            if (e.NewItems != null)
            {
                foreach (Customer c in e.NewItems)
                    SubscribeCustomer(c);
            }

            CustomersComboBox.ItemsSource = null;
            CustomersComboBox.ItemsSource = _customers;

            RefreshCartView();
        }

        /// <summary>
        /// Обработчик изменения свойств покупателя (например, имени).
        /// Обновляет список покупателей и, при необходимости, корзину.
        /// </summary>
        private void OnCustomerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // обновляем список покупателей (например, поменяли имя)
            CustomersComboBox.ItemsSource = null;
            CustomersComboBox.ItemsSource = _customers;

            if (ReferenceEquals(sender, _currentCustomer))
                RefreshCartView();
        }

        /// <summary>
        /// Обработчик изменения коллекции товаров в корзине конкретного покупателя.
        /// </summary>
        private void OnCustomerCartChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_currentCustomer is not null &&
                ReferenceEquals(sender, _currentCustomer.Cart.Items))
            {
                RefreshCartView();
            }
        }

        /// <summary>
        /// Обработчик изменения свойств товара, находящегося в корзине (влияет на сумму).
        /// </summary>
        private void OnCartItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Item.Cost))
                RefreshCartView();
        }

        // ================== Подписки / отписки ==================

        /// <summary>
        /// Подписывает все товары в коллекции на общий обработчик изменений.
        /// Нужен, чтобы при изменении Name/Cost любого товара обновлялся UI (список товаров и корзина).
        /// </summary>
        private void SubscribeItems(ObservableCollection<Item> items)
        {
            foreach (var it in items)
                it.PropertyChanged += OnItemPropertyChanged;
        }

        /// <summary>
        /// Отписывает все товары в коллекции от обработчика изменений.
        /// Вызывается перед заменой коллекции, чтобы не висели лишние подписки и утечки.
        /// </summary>
        private void UnsubscribeItems(ObservableCollection<Item> items)
        {
            foreach (var it in items)
                it.PropertyChanged -= OnItemPropertyChanged;
        }

        /// <summary>
        /// Подписывает всех покупателей в коллекции:
        ///  - на изменение их свойств (имя и т.п.)
        ///  - на изменения их корзин.
        /// </summary>
        private void SubscribeCustomers(ObservableCollection<Customer> customers)
        {
            foreach (var c in customers)
                SubscribeCustomer(c);
        }

        /// <summary>
        /// Снимает все подписки со всех покупателей в коллекции.
        /// Используется перед заменой коллекции покупателей.
        /// </summary>
        private void UnsubscribeCustomers(ObservableCollection<Customer> customers)
        {
            foreach (var c in customers)
                UnsubscribeCustomer(c);
        }

        /// <summary>
        /// Подписывает одного покупателя:
        ///  - на изменение его свойств (PropertyChanged),
        ///  - на изменения списка товаров в корзине (CollectionChanged),
        ///  - на изменение свойств каждого товара в корзине (например, Cost).
        /// </summary>
        private void SubscribeCustomer(Customer c)
        {
            c.PropertyChanged += OnCustomerPropertyChanged;
            c.Cart.Items.CollectionChanged += OnCustomerCartChanged;

            foreach (var it in c.Cart.Items)
                it.PropertyChanged += OnCartItemPropertyChanged;
        }

        /// <summary>
        /// Полностью снимает подписки с одного покупателя и всех его товаров в корзине.
        /// Вызывается при удалении покупателя или при перестроении коллекции.
        /// </summary>
        private void UnsubscribeCustomer(Customer c)
        {
            c.PropertyChanged -= OnCustomerPropertyChanged;
            c.Cart.Items.CollectionChanged -= OnCustomerCartChanged;

            foreach (var it in c.Cart.Items)
                it.PropertyChanged -= OnCartItemPropertyChanged;
        }

        // ================== Обновление UI ==================

        /// <summary>
        /// Обновить текст с суммой корзины.
        /// </summary>
        private void UpdateAmount()
        {
            if (_currentCustomer is null)
            {
                AmountTextBlock.Text = "0";
                return;
            }

            AmountTextBlock.Text = _currentCustomer.Cart.Amount.ToString("0.00");
        }

        /// <summary>
        /// Полностью обновить представление корзины и сумму.
        /// </summary>
        private void RefreshCartView()
        {
            if (_currentCustomer is null)
            {
                CartItemsListBox.ItemsSource = null;
                AmountTextBlock.Text = "0";
                return;
            }

            if (!ReferenceEquals(CartItemsListBox.ItemsSource, _currentCustomer.Cart.Items))
                CartItemsListBox.ItemsSource = _currentCustomer.Cart.Items;

            UpdateAmount();
        }

        /// <summary>
        /// Внешний вызов из MainWindow, когда меняются Items/Customers.
        /// </summary>
        public void RefreshData()
        {
            ItemsListBox.ItemsSource      = _items;
            CustomersComboBox.ItemsSource = _customers;

            if (_currentCustomer is null && _customers.Count > 0)
            {
                CustomersComboBox.SelectedIndex = 0;
                _currentCustomer = CustomersComboBox.SelectedItem as Customer;
            }

            if (_currentCustomer is not null)
                CartItemsListBox.ItemsSource = _currentCustomer.Cart.Items;
            else
                CartItemsListBox.ItemsSource = null;

            UpdateAmount();
        }
    }
}