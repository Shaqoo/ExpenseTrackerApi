using System.Text.Json.Serialization;

namespace ExpenseTracker.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Expense = 1,
        Income = 2
    }

}