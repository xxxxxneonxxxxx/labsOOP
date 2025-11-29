using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ObjectOrientedPractics.View.Controls
{
    /// <summary>
    /// Контрол для отображения и редактирования адреса.
    /// </summary>
    /// <remarks>
    /// Вынесен в отдельный <see cref="UserControl"/>, чтобы переиспользовать
    /// разметку адреса в разных частях интерфейса.
    /// </remarks>
    public partial class AddressControl : UserControl
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AddressControl"/>.
        /// </summary>
        public AddressControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Загружает XAML-разметку контрола.
        /// </summary>
        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}