
namespace BankAPI.Data.DTOs;

public class TransactionDtoIn
{
    public int accountId {get; set;}

    public int transactionType {get; set;}

    public decimal amount {get; set;}

    public int? externalAccount {get; set;} = null!;

}