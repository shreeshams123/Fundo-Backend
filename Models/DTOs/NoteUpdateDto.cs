using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class NoteUpdateDto
    {
        public string? Title {  get; set; }
        public string? Description { get; set; }
        public string? Color {  get; set; }
        public bool? IsArchive { get; set; }
        public bool? IsTrash { get; set; }
    }
}
