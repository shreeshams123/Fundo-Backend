using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class LabelCheckListDto
    {
        public int LabelId {  get; set; }
        public string LabelName { get; set; }
        public bool IsChecked { get; set; } 
    }
}
