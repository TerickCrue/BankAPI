using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BankAPI.Data.BankModels;
using BC = BCrypt.Net.BCrypt;

namespace BankAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{

    private readonly LoginService loginService;

    private IConfiguration config;

    public LoginController(LoginService loginService, IConfiguration config)
    {
        this.config = config;
        this.loginService = loginService;
    }


    [HttpPost("authenticate/admin")]
    public async Task<IActionResult> Login(AdminDto adminDto)
    {
        var admin = await loginService.GetAdmin(adminDto);

        if(admin is null)
            return BadRequest(new {message = "Credenciales invalidas."});
        
        string jwtToken = GenerateToken(admin);

        return Ok(new {token = jwtToken});
    }

    [HttpPost("authenticate/user")]
    public async Task<IActionResult> UserLogin(ClientDto clientDto)
    {
        var client = await loginService.GetUser(clientDto);

        if(client is null || !BC.Verify(clientDto.Pwd, client.Pwd))
        {
            return BadRequest(new {message = "Credenciales invalidas."});
        }

        string jwtToken = GenerateToken(client);
        return Ok(new {token = jwtToken});
        
    }


    private string GenerateToken(Administrator admin)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("AdminType", admin.AdminType)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken
        (
            claims: claims, 
            expires: DateTime.Now.AddMinutes(60), 
            signingCredentials: creds
        );

        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }

    private string GenerateToken(Client client)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Email, client.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key2").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken
        (
            claims: claims, 
            expires: DateTime.Now.AddMinutes(60), 
            signingCredentials: creds
        );

        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }


}