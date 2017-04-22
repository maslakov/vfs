namespace Core.FsExceptions
{
    /// <summary>
    /// Wrong input parameters exception
    /// </summary>
    public class ParamsNotAllowedException : BaseVFSException
    {
        public ParamsNotAllowedException() { }
        public ParamsNotAllowedException(string message) : base(message) { }
    }
}
