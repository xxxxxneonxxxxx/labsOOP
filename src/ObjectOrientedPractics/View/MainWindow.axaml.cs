using Avalonia.Controls;
using ObjectOrientedPractics.Model;

namespace ObjectOrientedPractics
{
    public partial class MainWindow : Window
    {
        private readonly Store _store;

        public MainWindow()
        {
            InitializeComponent();

            _store = new Store();

            ItemsTab.Items = _store.Items;
            CustomersTab.Customers = _store.Customers;
        }
    }
}