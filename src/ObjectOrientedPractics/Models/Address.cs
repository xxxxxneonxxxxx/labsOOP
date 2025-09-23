using System;

using ObjectOrientedPractics.Services;

namespace ObjectOrientedPractics.Model
{
    public class Address
    {
        /// <summary>
        /// Почтовый индекс.
        /// </summary>
        private int _index;

        /// <summary>
        /// Страна/регион.
        /// </summary>
        private string _country = string.Empty;

        /// <summary>
        /// Город (населенный пункт).
        /// </summary>
        private string _city = string.Empty;

        /// <summary>
        /// Улица.
        /// </summary>
        private string _street = string.Empty;

        /// <summary>
        /// Номер дома.
        /// </summary>
        private string _building = string.Empty;

        /// <summary>
        /// Номер квартиры/помещения.
        /// </summary>
        private string _apartment = string.Empty;

        /// <summary>
        /// Создает экземпляр класса <see cref="Address"/> по умолчанию.
        /// </summary>
        public Address()
        {
            Index = 100000;
            Country = "";
            City = "";
            Street = "";
            Building = "";
            Apartment = "";
        }

        /// <summary>
        /// Создает экземпляр класса <see cref="Address"/>.
        /// </summary>
        /// <param name="index">Почтовый индекс.</param>
        /// <param name="country">Страна/регион.</param>
        /// <param name="city">Город.</param>
        /// <param name="street">Улица.</param>
        /// <param name="building">Номер дома.</param>
        /// <param name="apartment">Номер квартиры.</param>
        public Address(int index, string country, string city, string street, string building, string apartment)
        {
            Index = index;
            Country = country;
            City = city;
            Street = street;
            Building = building;
            Apartment = apartment;
        }
        /// <summary>
        /// Возвращает или задает почтовый индекс. Должен быть шестизначным числом.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set
            {
                if (value < 100000 || value > 999999)
                {
                    throw new ArgumentException("Почтовый индекс должен быть шестизначным числом.");
                }
                _index = value;
            }
        }
        /// <summary>
        /// Возвращает или задает страну/регион. Длина не более 50 символов.
        /// </summary>
        public string Country
        {
            get { return _country; }
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 50, nameof(Country));
                _country = value ?? string.Empty;
            }
        }
        /// <summary>
        /// Возвращает или задает город. Длина не более 50 символов.
        /// </summary>
        public string City
        {
            get { return _city; }
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 50, nameof(City));
                _city = value ?? string.Empty;
            }
        }
        /// <summary>
        /// Возвращает или задает улицу. Длина не более 100 символов.
        /// </summary>
        public string Street
        {
            get { return _street; }
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 100, nameof(Street));
                _street = value ?? string.Empty;
            }
        }
        /// <summary>
        /// Возвращает или задает номер дома. Длина не более 10 символов.
        /// </summary>
        public string Building
        {
            get { return _building; }
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 10, nameof(Building));
                _building = value ?? string.Empty;
            }
        }
        /// <summary>
        /// Возвращает или задает номер квартиры. Длина не более 10 символов.
        /// </summary>
        public string Apartment
        {
            get { return _apartment; }
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 10, nameof(Apartment));
                _apartment = value ?? string.Empty;
            }
        }
        /// <summary>
        /// Возвращает строковое представление адреса.
        /// </summary>
        /// <returns>Строка адреса.</returns>
        public override string ToString()
        {
            return $"{Index}, {Country}, {City}, {Street}, {Building}, {Apartment}";
        }
        
    }
}