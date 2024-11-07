using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Collaborator
    {
        public int UserId {  get; set; }
        public User User { get; set; }
        public int NoteId {  get; set; }
        public Note Note { get; set; }
    }
}
