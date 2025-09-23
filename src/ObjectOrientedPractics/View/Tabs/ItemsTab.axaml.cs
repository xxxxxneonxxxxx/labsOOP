using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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

        private readonly ObservableCollection<Item> _items = new();

        /// <summary>
        /// Выбранный товар.
        /// </summary>
        private Item? _selected;

        /// <summary>
        /// Инициализация вкладки ItemsTab.
        /// </summary>
        public ItemsTab()
        {
            InitializeComponent();
            CategoryBox.SelectionChanged += OnCategoryChanged;
            ItemsList.ItemsSource = _items;
            ItemsList.SelectionChanged += (_, __) =>
            {
                _selected = ItemsList.SelectedItem as Item;
                LoadToForm(_selected);
            };

            AddBtn.Click += OnAdd;
            RemoveBtn.Click += OnRemove;
            SaveBtn.Click += OnSave;
        }
        

        /// <summary>
        /// Загрузка данных товара в форму.
        /// </summary>
        private void LoadToForm(Item? it)
        {
            ErrorText.Text = "";
            IdBox.Text = it?.Id.ToString() ?? "";
            NameBox.Text = it?.Name ?? "";
            InfoBox.Text = it?.Info ?? "";
            CostBox.Text = it?.Cost.ToString() ?? "";
            CategoryBox.SelectedIndex = it is null ? -1 : (int)it.Category;
        }

        /// <summary>
        /// Добавление нового товара.
        /// </summary>
        private void OnAdd(object? sender, RoutedEventArgs e)
        {
            var it = new Item("New item", "", 0m,Category.Other);
            _items.Add(it);
            ItemsList.SelectedItem = it;
        }
        /// <summary>
        /// Событие изменения категории в ComboBox.
        /// Обновляет категорию у выбранного товара.
        /// </summary>
        private void OnCategoryChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_selected == null) return;

            _selected.Category = (Category)CategoryBox.SelectedIndex;
        }

        /// <summary>
        /// Удаление выбранного товара.
        /// </summary>
        private void OnRemove(object? sender, RoutedEventArgs e)
        {
            if (_selected is null) return;
            _items.Remove(_selected);
            _selected = null;
            LoadToForm(null);
        }

        /// <summary>
        /// Сохранение данных выбранного товара.
        /// </summary>
        private void OnSave(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (_selected is null) return;

                _selected.Name = NameBox.Text ?? "";
                _selected.Info = InfoBox.Text ?? "";
                _selected.Category = (Category)CategoryBox.SelectedIndex;

                if (!decimal.TryParse(CostBox.Text, out var cost))
                    throw new ArgumentException("Cost: введите число");

                if (cost < 0m || cost > 100_000m)
                    throw new ArgumentOutOfRangeException(nameof(cost), "Стоимость должна быть от 0 до 100000.");

                _selected.Cost = cost;

                var idx = ItemsList.SelectedIndex;
                ItemsList.ItemsSource = null;
                ItemsList.ItemsSource = _items;
                ItemsList.SelectedIndex = idx;

                ErrorText.Text = "Сохранено";
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }
    }
}