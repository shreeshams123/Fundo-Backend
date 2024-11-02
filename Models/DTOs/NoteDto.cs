using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class NoteDto
    {
        [Required]
        public string Title {  get; set; }
        [Required]
        public string Description { get; set; }
        public string? Color {  get; set; }
    }
}
