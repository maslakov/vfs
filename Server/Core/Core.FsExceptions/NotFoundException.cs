namespace Core.FsExceptions
{
    /// <summary>
    /// Something was not found exception
    /// </summary>
    public class NotFoundException : BaseVFSException
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base (message){}
    }
}
