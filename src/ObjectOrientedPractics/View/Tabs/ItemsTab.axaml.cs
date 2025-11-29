using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using Avalonia.Controls;
using Avalonia.Interactivity;

using ObjectOrientedPractics.Model;

namespace ObjectOrientedPractics.View.Tabs
{
    /// <summary>
    /// Вкладка для работы с товарами.
    /// </summary>
    public partial class ItemsTab : UserControl
    {
        /// <summary>
        /// Коллекция товаров.
        /// </summary>
        private ObservableCollection<Item>? _items;

        /// <summary>
        /// Текущий выбранный товар.
        /// </summary>
        private Item? _selected;

        /// <summary>
        /// Сигнал о том, что товары изменились (для обновления других вкладок).
        /// </summary>
        public event Action? ItemsChanged;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ItemsTab"/>.
        /// </summary>
        public ItemsTab()
        {
            InitializeComponent();

            // Подкидываем в комбобокс все значения enum Category.
            CategoryBox.ItemsSource = Enum.GetValues(typeof(Category));

            // Выбор элемента в списке.
            ItemsList.SelectionChanged += (_, __) =>
            {
                _selected = ItemsList.SelectedItem as Item;
                LoadToForm(_selected);
            };

            // Валидация при изменении полей.
            NameBox.PropertyChanged += (_, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                    ValidateAll();
            };

            InfoBox.PropertyChanged += (_, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                    ValidateAll();
            };

            CostBox.PropertyChanged += (_, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                    ValidateAll();
            };

            CategoryBox.SelectionChanged += (_, __) => ValidateAll();

            // Кнопки.
            AddBtn.Click    += OnAdd;
            RemoveBtn.Click += OnRemove;
            SaveBtn.Click   += OnSave;
        }

        /// <summary>
        /// Коллекция товаров вкладки (назначить до использования).
        /// </summary>
        public ObservableCollection<Item> Items
        {
            get => _items ?? throw new InvalidOperationException(
                "Коллекция товаров не установлена. Назначьте ItemsTab.Items = store.Items перед использованием.");
            set
            {
                if (_items != null)
                {
                    _items.CollectionChanged -= OnItemsCollectionChanged;
                    UnsubscribeItems(_items);
                }

                _items = value ?? throw new ArgumentNullException(nameof(value));

                ItemsList.ItemsSource = _items;

                _items.CollectionChanged += OnItemsCollectionChanged;
                SubscribeItems(_items);

                ItemsChanged?.Invoke();
            }
        }

        /// <summary>
        /// Загружает данные товара в форму.
        /// </summary>
        /// <param name="item">Товар или <c>null</c>, чтобы очистить форму.</param>
        private void LoadToForm(Item? item)
        {
            ErrorText.Text = string.Empty;

            IdBox.Text   = item?.Id.ToString()   ?? string.Empty;
            NameBox.Text = item?.Name            ?? string.Empty;
            InfoBox.Text = item?.Info            ?? string.Empty;
            CostBox.Text = item?.Cost.ToString() ?? string.Empty;

            CategoryBox.SelectedItem = item?.Category;
        }

        /// <summary>
        /// Обработчик добавления нового товара.
        /// </summary>
        private void OnAdd(object? sender, RoutedEventArgs e)
        {
            var item = new Item("New item", string.Empty, 0m, Category.Other);

            _items!.Add(item);
            ItemsList.SelectedItem = item;

            ItemsChanged?.Invoke();
        }

        /// <summary>
        /// Обработчик удаления выбранного товара.
        /// </summary>
        private void OnRemove(object? sender, RoutedEventArgs e)
        {
            if (_selected == null)
                return;

            _items!.Remove(_selected);
            _selected = null;

            LoadToForm(null);
            ItemsChanged?.Invoke();
        }

        /// <summary>
        /// Обработчик сохранения изменений товара.
        /// </summary>
        private void OnSave(object? sender, RoutedEventArgs e)
        {
            if (_selected == null)
                return;

            if (!ValidateAll())
                return;

            try
            {
                _selected.Name = NameBox.Text ?? string.Empty;
                _selected.Info = InfoBox.Text ?? string.Empty;

                if (CategoryBox.SelectedItem is Category category)
                    _selected.Category = category;
                else
                    throw new ArgumentException("Выберите категорию.");

                if (!decimal.TryParse(CostBox.Text, out var cost))
                    throw new ArgumentException("Стоимость: введите число.");

                if (cost < 0m || cost > 100_000m)
                    throw new ArgumentOutOfRangeException(
                        nameof(cost),
                        "Стоимость должна быть от 0 до 100000.");

                _selected.Cost = cost;

                // Форсим обновление списка, чтобы грид/redraw подтянул новые значения.
                var index = ItemsList.SelectedIndex;
                ItemsList.ItemsSource = null;
                ItemsList.ItemsSource = _items;
                ItemsList.SelectedIndex = index;

                ErrorText.Text = "Сохранено";

                ItemsChanged?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }

        /// <summary>
        /// Реакция на изменение состава коллекции товаров.
        /// </summary>
        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Item item in e.OldItems)
                    item.PropertyChanged -= OnItemPropertyChanged;
            }

            if (e.NewItems != null)
            {
                foreach (Item item in e.NewItems)
                    item.PropertyChanged += OnItemPropertyChanged;
            }

            ItemsChanged?.Invoke();
        }

        /// <summary>
        /// Реакция на изменение свойств конкретного товара.
        /// </summary>
        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Любое изменение товара пересчитывает корзину/заказы.
            ItemsChanged?.Invoke();
        }

        /// <summary>
        /// Подписывает все элементы коллекции на событие изменения свойств.
        /// </summary>
        private static void SubscribeItems(ObservableCollection<Item> items)
        {
            foreach (var item in items)
                item.PropertyChanged += OnStaticItemPropertyChanged;
        }

        /// <summary>
        /// Отписывает все элементы коллекции от события изменения свойств.
        /// </summary>
        private static void UnsubscribeItems(ObservableCollection<Item> items)
        {
            foreach (var item in items)
                item.PropertyChanged -= OnStaticItemPropertyChanged;
        }

        /// <summary>
        /// Проксирующий обработчик для подписки/отписки из статических методов.
        /// Вызывает экземплярный <see cref="OnItemPropertyChanged"/> через <see cref="ItemsChanged"/>.
        /// </summary>
        private static void OnStaticItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Тут мы ничего не знаем про конкретный ItemsTab, поэтому
            // логика реакции на изменения делается через CollectionChanged/ItemsChanged.
            // Если хочется жёсткой связи с конкретным табом, можно не делить на static.
        }

        // ===== Валидация (итог в ErrorText) =====

        /// <summary>
        /// Выполняет полную проверку всех полей формы.
        /// </summary>
        private bool ValidateAll()
        {
            var message = string.Empty;

            if (!ValidateName(out var nameError))
                message += nameError + Environment.NewLine;

            if (!ValidateInfo(out var infoError))
                message += infoError + Environment.NewLine;

            if (!ValidateCost(out var costError))
                message += costError + Environment.NewLine;

            if (!ValidateCategory(out var categoryError))
                message += categoryError + Environment.NewLine;

            ErrorText.Text = message.TrimEnd();

            return string.IsNullOrEmpty(message);
        }

        /// <summary>
        /// Проверяет корректность названия товара.
        /// </summary>
        private bool ValidateName(out string error)
        {
            var value = NameBox.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                error = "Название не может быть пустым.";
                return false;
            }

            if (value.Length > 200)
            {
                error = "Название до 200 символов.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        /// <summary>
        /// Проверяет корректность описания товара.
        /// </summary>
        private bool ValidateInfo(out string error)
        {
            var value = InfoBox.Text ?? string.Empty;

            if (value.Length > 1000)
            {
                error = "Описание до 1000 символов.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        /// <summary>
        /// Проверяет корректность стоимости товара.
        /// </summary>
        private bool ValidateCost(out string error)
        {
            var value = CostBox.Text ?? string.Empty;

            if (!decimal.TryParse(value, out var decimalValue))
            {
                error = "Стоимость должна быть числом.";
                return false;
            }

            if (decimalValue < 0m || decimalValue > 100_000m)
            {
                error = "Стоимость от 0 до 100 000.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        /// <summary>
        /// Проверяет, что выбрана категория товара.
        /// </summary>
        private bool ValidateCategory(out string error)
        {
            if (CategoryBox.SelectedItem is not Category)
            {
                error = "Выберите категорию.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}