namespace MiddleWare
{
    public class NoteException : Exception
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