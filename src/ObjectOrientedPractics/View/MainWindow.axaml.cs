using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ObjectOrientedPractics.Model;

namespace ObjectOrientedPractics
{
    /// <summary>
    /// Главное окно приложения.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Store _store;

        public MainWindow()
        {
            InitializeComponent();

            _store = new Store();

            // Общие коллекции
            ItemsTab.Items         = _store.Items;
            ItemsTab.ItemsChanged  += () =>
            {
                CartsTab.RefreshData();
                OrdersTab.RefreshData();
            };
            CustomersTab.Customers = _store.Customers;
            CustomersTab.CustomersChanged += () =>
            {
                CartsTab.RefreshData();
                OrdersTab.RefreshData();
            };

            CartsTab.Items      = _store.Items;
            CartsTab.Customers  = _store.Customers;

            CartsTab.OrderCreated += (_, __) => OrdersTab.RefreshData();

            OrdersTab.Customers = _store.Customers;

            MainTabControl.SelectionChanged += MainTabControl_OnSelectionChanged;
        }

        private void MainTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem == CartsTabItem)
            {
                CartsTab.RefreshData();
            }
            else if (MainTabControl.SelectedItem == OrdersTabItem)
            {
                OrdersTab.RefreshData();
            }
        }
    }
}
