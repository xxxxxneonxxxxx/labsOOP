// Model/Order.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ObjectOrientedPractics.Model
{
    /// <summary>
    /// Заказ покупателя.
    /// </summary>
    public class Order : INotifyPropertyChanged
    {
        private readonly int _id;
        private readonly DateTime _created;
        private readonly ObservableCollection<Item> _items;
        private Address _address;
        private OrderStatus _status;

        public int Id => _id;

        public DateTime Created => _created;

        /// <summary>
        /// Адрес доставки.
        /// </summary>
        public Address Address
        {
            get => _address;
            set => _address = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Товары в заказе.
        /// </summary>
        public ObservableCollection<Item> Items => _items;

        /// <summary>
        /// Количество позиций.
        /// </summary>
        public int ItemsCount => _items.Count;

        /// <summary>
        /// Общая стоимость заказа.
        /// </summary>
        public decimal Amount => _items.Sum(i => i.Cost);

        /// <summary>
        /// Статус заказа.
        /// </summary>
        public OrderStatus Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public Order(int id, Address address, IEnumerable<Item> items)
        {
            _id = id;
            _created = DateTime.Now;
            _address = address ?? throw new ArgumentNullException(nameof(address));
            _items = new ObservableCollection<Item>(items ?? throw new ArgumentNullException(nameof(items)));
            _items.CollectionChanged += OnItemsChanged;
            foreach (var item in _items)
            {
                item.PropertyChanged += OnItemChanged;
            }
            _status = OrderStatus.New;
        }

        private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Item it in e.OldItems)
                    it.PropertyChanged -= OnItemChanged;
            }
            if (e.NewItems != null)
            {
                foreach (Item it in e.NewItems)
                    it.PropertyChanged += OnItemChanged;
            }

            OnPropertyChanged(nameof(ItemsCount));
            OnPropertyChanged(nameof(Amount));
        }

        private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Item.Cost))
            {
                OnPropertyChanged(nameof(Amount));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString()
        {
            return $"#{Id} ({Status}), {Amount:0.00}";
        }
    }
}
