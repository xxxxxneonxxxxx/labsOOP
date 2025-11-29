using ObjectOrientedPractics.Services;
using System.Collections.Generic;
using System.ComponentModel;

namespace ObjectOrientedPractics.Model
{
	/// <summary>Представляет покупателя.</summary>
	public class Customer : INotifyPropertyChanged
	{
		/// <summary>Уникальный идентификатор покупателя.</summary>
		private readonly int _id;

		/// <summary>Полное имя покупателя (ФИО, до 200 символов).</summary>
		private string _fullname = string.Empty;

		/// <summary>Адрес доставки (композиция).</summary>
		private readonly Address _address;

		/// <summary>Возвращает уникальный идентификатор покупателя.</summary>
		public int Id => _id;

		/// <summary>Полное имя покупателя (до 200 символов).</summary>
		public string Fullname
		{
			get => _fullname;
			set
			{
				ValueValidator.AssertStringOnLength(value ?? string.Empty, 200, nameof(Fullname));
				if (_fullname == value) return;
				_fullname = value ?? string.Empty;
				OnPropertyChanged(nameof(Fullname));
			}
		}

		/// <summary>
		/// Адрес доставки (композиция).
		/// Объект создаётся внутри конструктора и не может быть заменён извне.
		/// Менять можно только его свойства.
		/// </summary>
		public Address Address => _address;

			/// <summary>
		/// Корзина покупателя (композиция).
		/// Создаётся в конструкторе.
		/// </summary>
		public Cart Cart { get; private set; }
		
		/// <summary>
		/// Заказы покупателя (агрегация списка заказов).
		/// </summary>
    	public List<Order> Orders { get; } = new List<Order>();

		/// <summary>
		/// Создаёт экземпляр покупателя.
		/// </summary>
		/// <param name="fullname">Полное имя (≤ 200 символов).</param>
		/// <param name="index">Почтовый индекс.</param>
		/// <param name="country">Страна/регион.</param>
		/// <param name="city">Город.</param>
		/// <param name="street">Улица.</param>
		/// <param name="building">Дом.</param>
		/// <param name="apartment">Квартира.</param>
		public Customer(
			string fullname,
			int index = 100000,
			string? country = "",
			string? city = "",
			string? street = "",
			string? building = "",
			string? apartment = "")
		{
			_id = IdGenerator.GetNextId();
			Fullname = fullname;

			// создаём адрес прямо здесь — композиция
			_address = new Address(index, country ?? "", city ?? "", street ?? "", building ?? "", apartment ?? "");
			_address.PropertyChanged += (_, __) => OnPropertyChanged(nameof(Address));

			// создаём корзину — композиция
			Cart = new Cart();
		}

		/// <summary>Возвращает строковое представление покупателя.</summary>
		public override string ToString() => $"{Fullname}, {_address}";

		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged(string propertyName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
