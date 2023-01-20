using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankAPI.Data.BankModels;

public partial class Client
{
    public int Id { get; set; }

    [MaxLength(200, ErrorMessage = "El nombre no puede contener mas de 200 caracteres")]
    public string Name { get; set; } = null!;

    [MaxLength(40, ErrorMessage = "El telefono no puede contener mas de 40 caracteres")]
    public string PhoneNumber { get; set; } = null!;

    [MaxLength(50, ErrorMessage = "El Email no puede contener mas de 50 caracteres")]
    [EmailAddress(ErrorMessage = "Fomato de correo incorrecto")]
    public string? Email { get; set; }

    public DateTime RegDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<Account> Accounts { get; } = new List<Account>();
}
