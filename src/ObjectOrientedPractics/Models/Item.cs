using ObjectOrientedPractics.Services;
using System;

namespace ObjectOrientedPractics.Model
{
    /// <summary>
    /// Представляет товар.
    /// </summary>
    public class Item
    {
        // ----- readonly-поля -----
        /// <summary>Уникальный идентификатор товара.</summary>
        private readonly int _id;

        // ----- обычные поля -----
        /// <summary>Название товара.</summary>
        private string _name = string.Empty;

        /// <summary>Описание товара.</summary>
        private string _info = string.Empty;

        /// <summary>Стоимость товара.</summary>
        private decimal _cost;

        // ----- свойства -----
        /// <summary>Возвращает уникальный идентификатор товара.</summary>
        public int Id => _id;
        /// <summary>
        /// Категория товара.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>Название (до 200 символов).</summary>
        public string Name
        {
            get => _name;
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 200, nameof(Name));
                _name = value ?? string.Empty;
            }
        }

        /// <summary>Описание (до 1000 символов).</summary>
        public string Info
        {
            get => _info;
            set
            {
                ValueValidator.AssertStringOnLength(value ?? string.Empty, 1000, nameof(Info));
                _info = value ?? string.Empty;
            }
        }

        /// <summary>Стоимость [0; 100000].</summary>
        public decimal Cost
        {
            get => _cost;
            set
            {
                if (value < 0 || value > 100000)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(Cost),
                        "Стоимость должна быть от 0 до 100000."
                    );
                }
                _cost = value;
            }
        }

        // ----- конструктор -----
        /// <summary>
        /// Создает экземпляр товара.
        /// </summary>
        /// <param name="name">Название (<=200).</param>
        /// <param name="info">Описание (<=1000).</param>
        /// <param name="cost">Стоимость [0;100000].</param>
        /// <param name="category">Категория товара.</param>

        public Item(string name, string info, decimal cost, Category category)
        {
            _id = IdGenerator.GetNextId();
            Name = name;
            Info = info;
            Cost = cost;
            Category = category;
        }
        /// <summary>
        /// Возвращает строковое представление товара.
        /// </summary>
        public override string ToString() => $"{Name} — {Cost:C}";
    }
}