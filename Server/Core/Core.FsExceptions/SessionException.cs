namespace Core.FsExceptions
{
    /// <summary>
    /// Session manager exception
    /// </summary>
    public class SessionException : BaseVFSException
    {
        public SessionException() { }
        public SessionException(string message) : base(message) { }
    }
}
