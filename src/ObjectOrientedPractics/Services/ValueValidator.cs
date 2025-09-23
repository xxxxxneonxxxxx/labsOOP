using System;

namespace ObjectOrientedPractics.Services
{
    /// <summary>
    /// Предоставляет методы для валидации данных.
    /// </summary>
    public static class ValueValidator
    {
        /// <summary>
        /// Проверяет, что строка не превышает максимальную длину.
        /// </summary>
        /// <param name="value">Проверяемая строка.</param>
        /// <param name="maxLength">Максимальная допустимая длина.</param>
        /// <param name="propertyName">Имя свойства, которое проверяется.</param>
        /// <exception cref="ArgumentException">Возникает, если длина строки превышает максимальную.</exception>
        public static void AssertStringOnLength(string value, int maxLength, string propertyName)
        {
            if (value != null && value.Length > maxLength)
            {
                throw new ArgumentException(
                    $"{propertyName} должен быть меньше {maxLength} символов.");
            }
        }
    }
    
}