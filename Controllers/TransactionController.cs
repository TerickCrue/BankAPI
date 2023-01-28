
using BankAPI.Services;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
namespace BankAPI.Controllers;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/transaction")]
public class TransactionContoller : ControllerBase
{
    private readonly TransactionService transactionService;
    private readonly AccountService accountService;
    private readonly ClientService clientService;
    private readonly TransactionTypeService transactionTypeService;
    public TransactionContoller(TransactionService transactionService, AccountService accountService, ClientService clientService, TransactionTypeService transactionTypeService)
    {
        this.transactionService = transactionService;
        this.accountService = accountService;
        this.clientService = clientService;
        this.transactionTypeService = transactionTypeService;
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpGet("getall")]
    public async Task<IEnumerable<TransactionDtoOut>> Get()
    {
        return await transactionService.GetAll();
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDtoOut>> GetById(int id)
    {
        var transaction = await transactionService.GetDtoById(id);

        if(transaction is null)
            return NotFound();
        
        return transaction;

    }

    [Authorize(Policy = "AuthUser")]
    [HttpGet("accounts/{id}")]
    public async Task<IEnumerable<AccountDtoOut>> GetAccounts(int id)
    {
        var accountList = await accountService.GetAccountsByClientId(id);

        return accountList;
    }

    [Authorize(Policy = "AuthUser")]
    [HttpPost("movement")]
    public async Task<IActionResult> DepositOrWithdraw(TransactionDtoIn transaction)
    {
        string validation = await validateTransaction(transaction);

        if(!validation.Equals("valid"))
            return BadRequest(new {message = validation});


        var account = await accountService.GetById(transaction.accountId);
        var updatedAccount = new AccountDtoIn();

        if(account is not null)
        {
            updatedAccount.Id = account.Id;
            updatedAccount.AccountType = account.AccountType;
            updatedAccount.ClientId = account.ClientId;

            if(transaction.transactionType == 2 || transaction.transactionType == 4)
            {
                decimal newBalance = (account.Balance - transaction.amount);
                updatedAccount.Balance = newBalance;
            }
            if(transaction.transactionType == 1)
            {
                decimal newBalance = (account.Balance + transaction.amount);
                updatedAccount.Balance = newBalance;
            }
        }
        
        
        await accountService.Update(updatedAccount);

        var newTransaction = await transactionService.DepositOrWithdraw(transaction);

        return CreatedAtAction(nameof(GetById), new {id = newTransaction.Id}, newTransaction);

    }

    [Authorize(Policy = "AuthUser")]
    [HttpDelete("accounts/delete/{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var accountToDelete = await accountService.GetById(id);

        if(accountToDelete is not null && accountToDelete.Balance.Equals(0))
        {
            await accountService.Delete(id);
        }
        else if(accountToDelete is null)
        {
            return AccountNotFound(id);
        }
        else if(!accountToDelete.Balance.Equals(0))
        {
            return BadRequest(new {message = "La cuenta aún tiene saldo disponible, no se puede eliminar"});
        }

        return Ok();
    }

    public async Task<string> validateTransaction(TransactionDtoIn transactionDto)
    {
        string result = "valid";

        var transactionType = await transactionTypeService.GetById(transactionDto.transactionType);

        if(transactionType is null)
            result = $"El tipo de transacción {transactionDto.transactionType} no existe.";

        var accountId = transactionDto.accountId;
        var account = await accountService.GetById(accountId);

        if(account is null)
            result = $"La cuenta con ID {transactionDto.accountId} no existe.";
        else
        {
            if(transactionDto.amount <= 0)
            result = "No se puede realizar la transaccion con un monto invalido";

            if(transactionDto.transactionType == 2 || transactionDto.transactionType == 4)
            {
                if(transactionDto.amount > account.Balance)
                    result = "No hay suficientes fondos en la cuenta para realizar esta transacción";
            }

            if(transactionDto.transactionType == 3)
            result = "Solo se pueden hacer depositos en efectivo";

            if(transactionDto.transactionType == 4)
            {
                if(transactionDto.externalAccount is null)
                    result = "El tipo de transaccion requiere un numero de cuenta externo";
            }

            if(transactionDto.transactionType == 2 || transactionDto.transactionType == 1 )
            {
                if(transactionDto.externalAccount is not null)
                    result = "El tipo de transaccion no necesita una cuenta externa";
            }

        }
        

        return result;
    }

    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new { message = $"La cuenta con ID = {id} no existe. "});
    }



}