using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiblioTech_v2.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [StringLength(40)]
        [Required(ErrorMessage = "Campo Obrigatório!")]
        public string Nome { get; set; }

        [Display(Name = "Email")]
        [StringLength(40)]
        [Required(ErrorMessage = "Campo Obrigatório!")]
        [EmailAddress(ErrorMessage = "Endereço de email inválido")]
        public string Email { get; set; }

        [Display(Name = "RA")]
        [StringLength(10)]
        [Required(ErrorMessage = "Campo Obrigatório!")]
        public string RA { get; set; }
    }
}
