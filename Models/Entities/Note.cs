using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId {  get; set; }
        public string Title {  get; set; }
        public string Description { get; set; }
        public string? Color { get; set; }
        public bool IsArchive { get; set; } = false;
        public bool IsTrash { get; set; } = false;
         
    }
}
