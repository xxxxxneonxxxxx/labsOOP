using System.Collections.ObjectModel;

namespace ObjectOrientedPractics.Model
{
    /// <summary>
    /// Представляет магазин с товарами и покупателями.
    /// </summary>
    public class Store
    {
        /// <summary>
        /// Товары магазина.
        /// </summary>
        public ObservableCollection<Item> Items { get; }

        /// <summary>
        /// Покупатели магазина.
        /// </summary>
        public ObservableCollection<Customer> Customers { get; }

        /// <summary>
        /// Создаёт магазин с пустыми коллекциями.
        /// </summary>
        public Store()
        {
            Items = new ObservableCollection<Item>();
            Customers = new ObservableCollection<Customer>();
        }
    }
}