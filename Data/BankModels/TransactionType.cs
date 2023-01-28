using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BankAPI.Data.BankModels;

public partial class TransactionType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime RegDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<BankTransaction> BankTransactions { get; } = new List<BankTransaction>();
}
