using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ObjectOrientedPractics.View.Controls
{
    /// <summary>
    /// Контрол для отображения и редактирования данных заказа.
    /// </summary>
    /// <remarks>
    /// Используется как составной элемент интерфейса для работы с заказами.
    /// </remarks>
    public partial class OrderControl : UserControl
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="OrderControl"/>.
        /// </summary>
        public OrderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Загружает XAML-разметку контрола.
        /// </summary>
        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}