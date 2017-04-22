namespace Core.FsExceptions
{
    /// <summary>
    /// Operation not correct exception
    /// </summary>
    public class NotAllowedException : BaseVFSException
    {
        public NotAllowedException() { }
        public NotAllowedException(string message) : base(message) { }
    }
}
