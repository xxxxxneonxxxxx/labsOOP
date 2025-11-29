using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ObjectOrientedPractics.Model
{
    /// <summary>
    /// Корзина покупателя.
    /// </summary>
    /// <remarks>
    /// Хранит выбранные товары и автоматически пересчитывает итоговую сумму
    /// при изменении состава корзины или стоимости товаров.
    /// </remarks>
    public class Cart : INotifyPropertyChanged
    {
        /// <summary>
        /// Внутренняя коллекция товаров корзины.
        /// </summary>
        private readonly ObservableCollection<Item> _items = new();

        /// <summary>
        /// Товары в корзине.
        /// </summary>
        public ObservableCollection<Item> Items => _items;

        /// <summary>
        /// Итоговая стоимость всех товаров в корзине.
        /// </summary>
        public decimal Amount => _items.Sum(i => i.Cost);

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="Cart"/>.
        /// </summary>
        public Cart()
        {
            _items.CollectionChanged += OnItemsChanged;
        }

        /// <summary>
        /// Добавляет товар в корзину.
        /// </summary>
        /// <param name="item">Товар, который нужно добавить.</param>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если <paramref name="item"/> имеет значение <c>null</c>.
        /// </exception>
        public void Add(Item item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        /// <summary>
        /// Удаляет один экземпляр товара из корзины.
        /// </summary>
        /// <param name="item">Товар, который нужно удалить.</param>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если <paramref name="item"/> имеет значение <c>null</c>.
        /// </exception>
        public void Remove(Item item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _items.Remove(item);
        }

        /// <summary>
        /// Полностью очищает корзину.
        /// </summary>
        public void Clear()
        {
            UnsubscribeItems(_items);
            _items.Clear();
        }

        /// <summary>
        /// Обработчик изменения коллекции товаров в корзине.
        /// </summary>
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

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UnsubscribeItems(_items);
            }

            OnPropertyChanged(nameof(Amount));
        }

        /// <summary>
        /// Обработчик изменения свойств отдельного товара в корзине.
        /// </summary>
        /// <remarks>
        /// При изменении стоимости товара пересчитывается <see cref="Amount"/>.
        /// </remarks>
        private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Item.Cost))
                OnPropertyChanged(nameof(Amount));
        }

        /// <summary>
        /// Отписывает все товары в коллекции от обработчика <see cref="OnItemChanged"/>.
        /// </summary>
        /// <param name="items">Коллекция товаров, с которой нужно снять подписку.</param>
        private void UnsubscribeItems(ObservableCollection<Item> items)
        {
            foreach (var it in items)
                it.PropertyChanged -= OnItemChanged;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/> для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}