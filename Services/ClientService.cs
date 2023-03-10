using BankAPI.Data;
using Microsoft.EntityFrameworkCore;
using BankAPI.Data.BankModels;
using BC = BCrypt.Net.BCrypt;
namespace BankAPI.Services;

public class ClientService
{
    private readonly BankContext _context;

    public ClientService(BankContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAll()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client?> GetById(int id)
    {
        return await _context.Clients.FindAsync(id);
    }

    public async Task<Client> Create(Client newClient)
    {
        var client = new Client();
        client = newClient;
        client.Pwd = BC.HashPassword(newClient.Pwd);

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return newClient;
    }

    public async Task Update(int id, Client client)
    {
        var existingClient = await GetById(id);

        if(existingClient is not  null)
        {
            existingClient.Name = client.Name;
            existingClient.Email = client.Email;
            existingClient.PhoneNumber = client.PhoneNumber;

            await _context.SaveChangesAsync();
        }
    }

    public async Task Delete(int id)
    {
        var clientToDelte = await GetById(id);

        if(clientToDelte is not  null)
        {
            _context.Clients.Remove(clientToDelte);
            await _context.SaveChangesAsync();
        }
    }


}