namespace ObjectOrientedPractics.Services
{
    /// <summary>
    /// Предоставляет функционал для генерации уникальных идентификаторов.
    /// </summary>
    public static class IdGenerator
    {
        /// <summary>
        /// Текущий идентификатор.
        /// </summary>
        private static int _id = 0;

        /// <summary>
        /// Генерирует и возвращает следующий уникальный идентификатор.
        /// </summary>
        /// <returns>Уникальный идентификатор.</returns>
        public static int GetNextId()
        {
            return _id++;
        }
    }
}