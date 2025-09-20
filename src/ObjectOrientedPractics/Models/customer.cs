using ObjectOrientedPractics.Services;

namespace ObjectOrientedPractics.Model
{
    /// <summary>
    /// Представляет покупателя.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Уникальный идентификатор покупателя.
        /// </summary>
        private readonly int _id;

        /// <summary>
        /// Полное имя (Фамилия Имя Отчество), до 200 символов.
        /// </summary>
        private string _fullname = string.Empty;

        /// <summary>
        /// Адрес доставки, до 500 символов.
        /// </summary>
        private string _address = string.Empty;

        /// <summary>
        /// Возвращает уникальный идентификатор покупателя.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// Полное имя покупателя (до 200 символов).
        /// </summary>
        public string Fullname
        {
            get => _fullname;
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 200, nameof(Fullname));
                _fullname = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Адрес доставки (до 500 символов).
        /// </summary>
        public string Address
        {
            get => _address;
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 500, nameof(Address));
                _address = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Создает экземпляр покупателя.
        /// </summary>
        /// <param name="fullname">Полное имя (<=200).</param>
        /// <param name="address">Адрес (<=500).</param>
        public Customer(string fullname, string address)
        {
            _id = IdGenerator.GetNextId();
            Fullname = fullname;
            Address  = address;
        }
    }
}