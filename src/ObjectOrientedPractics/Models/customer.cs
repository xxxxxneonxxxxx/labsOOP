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
        private Address _address = new Address();

        /// <summary>
        /// Возвращает уникальный идентификатор покупателя.
        /// </summary>
        public int Id => _id;

        /// <summary>
        /// Создает экземпляр покупателя.
        /// </summary>
        /// <param name="fullname">Полное имя (<=200).</param>
        /// <param name="address">Адрес доставки.</param>
        public Customer(string fullname, Address address)
        {
            _id = IdGenerator.GetNextId();
            Fullname = fullname;
            Address = address;
            
        }

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
        public Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        
    }
}