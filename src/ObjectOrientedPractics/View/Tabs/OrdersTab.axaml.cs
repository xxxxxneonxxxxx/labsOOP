using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using ObjectOrientedPractics.Model;

namespace ObjectOrientedPractics.View.Tabs
{
    /// <summary>
    /// Вкладка с заказами.
    /// </summary>
    public partial class OrdersTab : UserControl
    {
        private ObservableCollection<Customer> _customers = new();
        private ObservableCollection<Order> _orders      = new();
        private Order? _currentOrder;

        private bool _isRefreshing;
        private bool _isStatusUpdating;

        /// <summary>
        /// Покупатели, из которых собираем заказы.
        /// </summary>
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value ?? new ObservableCollection<Customer>();
                Console.WriteLine($"[OrdersTab] Customers set: count={_customers.Count}");
                RefreshData();
            }
        }

        public OrdersTab()
        {
            InitializeComponent();

            ConfigureColumns();

            // список статусов в комбобоксе
            StatusComboBox.ItemsSource = Enum.GetValues(typeof(OrderStatus));

            // события
            OrdersDataGrid.SelectionChanged += OrdersDataGrid_OnSelectionChanged;
            StatusComboBox.SelectionChanged += StatusComboBox_OnSelectionChanged;
        }

        /// <summary>
        /// Создаём колонки грида вручную.
        /// </summary>
        private void ConfigureColumns()
        {
            OrdersDataGrid.Columns.Clear();

            OrdersDataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header  = "Id",
                    Binding = new Binding(nameof(Order.Id))
                });

            OrdersDataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header  = "Created",
                    Binding = new Binding(nameof(Order.Created)) { StringFormat = "G" }
                });

            OrdersDataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header  = "Items",
                    Binding = new Binding(nameof(Order.ItemsCount))
                });

            OrdersDataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header  = "Amount",
                    Binding = new Binding(nameof(Order.Amount)) { StringFormat = "0.00" }
                });

            OrdersDataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header  = "Status",
                    Binding = new Binding(nameof(Order.Status))
                });

            Console.WriteLine($"[OrdersTab] Columns configured: {OrdersDataGrid.Columns.Count}");
        }

        /// <summary>
        /// Пересобрать список заказов из всех покупателей.
        /// </summary>
        public void RefreshData()
        {
            var selectedId = _currentOrder?.Id;
            _isRefreshing = true;

            Console.WriteLine($"[OrdersTab] RefreshData: customers={_customers.Count}");

            var newOrders = new ObservableCollection<Order>();

            foreach (var customer in _customers)
            {
                Console.WriteLine($"[OrdersTab]   customer {customer.Id}: orders={customer.Orders.Count}");
                foreach (var order in customer.Orders)
                {
                    Console.WriteLine($"[OrdersTab]      order {order.Id}");
                    newOrders.Add(order);
                }
            }

            _orders = newOrders;

            OrdersDataGrid.ItemsSource = null;
            OrdersDataGrid.ItemsSource = _orders;

            Console.WriteLine($"[OrdersTab] Total orders in grid: {_orders.Count}, Columns={OrdersDataGrid.Columns.Count}");

            var toSelect = _orders.FirstOrDefault(o => o.Id == selectedId)
                           ?? _orders.FirstOrDefault();

            OrdersDataGrid.SelectedItem = toSelect;
            ShowOrder(toSelect);

            _isRefreshing = false;
        }

        private void OrdersDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing) return;

            _currentOrder = OrdersDataGrid.SelectedItem as Order;
            ShowOrder(_currentOrder);
        }

        /// <summary>
        /// Смена статуса по выбору в ComboBox.
        /// </summary>
        private void StatusComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing || _isStatusUpdating) return;
            if (_currentOrder is null) return;
            if (StatusComboBox.SelectedItem is not OrderStatus status) return;

            ApplyStatus(status);
        }

        private void ApplyStatus(OrderStatus status)
        {
            if (_currentOrder is null) return;
            if (_currentOrder.Status == status) return;

            _isStatusUpdating = true;

            _currentOrder.Status = status;
            Console.WriteLine($"[OrdersTab] Status changed: order={_currentOrder.Id}, status={status}");

            ShowOrder(_currentOrder);

            _isStatusUpdating = false;
        }

        /// <summary>
        /// Отобразить детали заказа справа.
        /// </summary>
        private void ShowOrder(Order? order)
        {
            _currentOrder = order;

            if (order is null)
            {
                OrderIdTextBlock.Text      = string.Empty;
                OrderCreatedTextBlock.Text = string.Empty;

                IndexTextBox.Text      = string.Empty;
                CountryTextBox.Text    = string.Empty;
                CityTextBox.Text       = string.Empty;
                StreetTextBox.Text     = string.Empty;
                BuildingTextBox.Text   = string.Empty;
                ApartmentTextBox.Text  = string.Empty;

                OrderItemsListBox.ItemsSource = null;
                StatusComboBox.SelectedItem   = null;
                AmountTextBlock.Text          = "0";
                return;
            }

            // ID и дата
            OrderIdTextBlock.Text      = order.Id.ToString();
            OrderCreatedTextBlock.Text = order.Created.ToString("G");

            // Адрес доставки
            var addr = order.Address;
            IndexTextBox.Text      = addr.Index.ToString();
            CountryTextBox.Text    = addr.Country;
            CityTextBox.Text       = addr.City;
            StreetTextBox.Text     = addr.Street;
            BuildingTextBox.Text   = addr.Building;
            ApartmentTextBox.Text  = addr.Apartment;

            // Товары
            OrderItemsListBox.ItemsSource = null;
            OrderItemsListBox.ItemsSource = order.Items;

            // Статус
            StatusComboBox.SelectedItem = order.Status;

            // Сумма заказа
            AmountTextBlock.Text = order.Amount.ToString("N2");
        }
    }
}