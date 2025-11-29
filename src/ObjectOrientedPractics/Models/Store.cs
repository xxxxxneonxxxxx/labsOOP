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
        public ObservableCollection<Item> Items { get; } = new();

        /// <summary>
        /// Покупатели магазина.
        /// </summary>
        public ObservableCollection<Customer> Customers { get; } = new();

        /// <summary>
        /// Создаёт магазин с пустыми коллекциями.
        /// </summary>
        public Store()
        {
            // Ничего не делаем: коллекции уже инициализированы выше
        }
    }
}