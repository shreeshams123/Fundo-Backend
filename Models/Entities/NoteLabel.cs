using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class NoteLabel
    {
        public int NoteId {  get; set; }
        public Note Note { get; set; }
        public int LabelId {  get; set; }
        public Label Label { get; set; }
    }
}
