using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aper_bot.Database.Model
{
    public class User
    {
        [Key]
        public int? Id { get; set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        /// TODO: This might differ form server to server this should be somehow handeled
        [Required]
        public string Name { get; set; } = "";

        [InverseProperty("Source")]
        public List<Quote> quotes { get; set; } = new List<Quote>();
    }
}