
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;
namespace BankAPI.Services;

public class TransactionService
{
    private readonly BankContext _context;
    public TransactionService(BankContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<TransactionDtoOut>> GetAll()
    {
        return await _context.BankTransactions.Select(a => new TransactionDtoOut 
        {
            Id = a.Id,
            Name = a.Account.Client.Name,
            transactionType = a.TransactionTypeNavigation.Name, 
            amount = a.Amount,
            externalAccount = a.ExternalAccount,
            regDate = a.RegDate
        }).ToListAsync();
    }

    public async Task<TransactionDtoOut?> GetDtoById(int id)
    {
        return await _context.BankTransactions.
        Where(a => a.Id == id).
        Select(a => new TransactionDtoOut 
        {
            Id = a.Id,
            Name = a.Account.Client.Name,
            transactionType = a.TransactionTypeNavigation.Name, 
            amount = a.Amount,
            externalAccount = a.ExternalAccount,
            regDate = a.RegDate
        }).SingleOrDefaultAsync();
    }

    public async Task<BankTransaction> DepositOrWithdraw(TransactionDtoIn transaction) 
    {
        var newTransaction = new BankTransaction();

        newTransaction.AccountId = transaction.accountId;
        newTransaction.TransactionType = transaction.transactionType;
        newTransaction.Amount = transaction.amount;
        newTransaction.ExternalAccount = transaction.externalAccount;


        _context.BankTransactions.Add(newTransaction);
        await _context.SaveChangesAsync();

        return newTransaction;
    }


}