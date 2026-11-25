using System.ComponentModel.DataAnnotations;

namespace SistemaHotelero.Dominio;

public sealed class LoginSolicitud

{

    [Required]

    [StringLength(50)]

    public string NombreUsuario { get; set; } = string.Empty;

    [Required]

    public string Contrasena { get; set; } = string.Empty;

}