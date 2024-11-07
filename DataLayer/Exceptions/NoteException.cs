using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Exceptions
{
    public class NoteException:Exception
    {
        public NoteException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public NoteException(string message)
            : base(message)
        {

        }
    }
}
