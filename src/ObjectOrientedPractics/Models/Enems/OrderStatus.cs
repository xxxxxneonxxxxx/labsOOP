// Model/OrderStatus.cs
namespace ObjectOrientedPractics.Model
{
    /// <summary>
    /// Статус заказа.
    /// </summary>
    public enum OrderStatus
    {
        New,        // Новый
        Processing, // Обрабатывается
        Assembly,   // Собирается на складе
        Sent,       // Отправлен
        Delivered,  // Доставлен
        Returned,   // Возврат
        Abandoned   // Отменен магазином
    }
}