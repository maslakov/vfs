namespace Core.FS
{
    /// <summary>
    /// File system type of the changes
    /// </summary>
    public enum FsChangeType {
        CREATE,
        DELETE,
        MOVE,
        LOCK,
        UNLOCK,
        COPY,
        HZ
    }

    /// <summary>
    /// Event argument: file sytem changed
    /// </summary>
    public class FSChangedEventArgs : StorageChangedEventArgs
    {
        public FsChangeType Type { get; set; }

        public FSChangedEventArgs(StorageChangedEventArgs e)
        {
            Item = e.Item;
            UserToken = e.UserToken;
            Type = FsChangeType.HZ;
        }

        public FSChangedEventArgs(StorageChangedEventArgs e, FsChangeType type)
            : this(e)
        {
            Type = type;
        }

    }
}
