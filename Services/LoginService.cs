using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;



namespace BankAPI.Services;

public class LoginService
{
    private readonly BankContext _context;
    public LoginService(BankContext context)
    {
        _context = context;
    }

    public async Task<Administrator?> GetAdmin(AdminDto admin)
    {
        return await _context.Administrators.SingleOrDefaultAsync(x => x.Email == admin.Email && x.Pwd == admin.Pwd);
    }

    public async Task<Client?> GetUser(ClientDto client)
    {
        var coincidence = await _context.Clients.SingleOrDefaultAsync(x => x.Email == client.Email);
        if(coincidence == null || !BC.Verify(client.Pwd, coincidence.Pwd))
        {
            return null;
        }
        else
            return coincidence;
    }
}