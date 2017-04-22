using System;

namespace Core.FS
{
    /// <summary>
    /// Abstract class to describe file system elements
    /// </summary>
    public abstract class FsItem 
    {
        protected string _name;
        protected string _fullName;
        protected string _path;

        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Full path of the item
        /// </summary>
        public string FullName 
        { 
            get {
                if ((_name == "\\" && String.IsNullOrEmpty(_path)) 
                    || (String.IsNullOrEmpty(_name) && _path == "\\")) return "\\";
                else 
                    return _path +
                        (String.IsNullOrEmpty(_path) ? "" 
                            : ((_path.EndsWith("\\")) ? "" 
                                : (_name.EndsWith("\\")) ? "" 
                                    : "\\")) 
                        + _name;
            } 
        }

        /// <summary>
        /// Parent path
        /// </summary>
        public string Path 
        { 
            get { return _path; }
            set { _path = value; }
        }

        internal FsItem()
        {
            _path = "";
        }

        /// <summary>
        /// Create an instance of file system entry
        /// </summary>
        /// <param name="name">name of the element (file or directory)</param>
        internal FsItem(string name) : this()
        {
            _name = name;
        }

        /// <summary>
        /// Create an instance of file system entry
        /// </summary>
        /// <param name="name">name of the element (file or directory)</param>
        /// <param name="path">Parent catalog path</param>
        internal FsItem(string name, string path) : this(name)
        {
            _path = path;
        }

        /// <summary>
        /// Equality by name
        /// </summary>
        public override bool Equals(object obj)
        {
            if ((obj as FsItem) != null)
                return this.Name == ((FsItem)obj).Name;
            else return false;
        }

        public override int GetHashCode()
        {
            return this.FullName.GetHashCode();
        }

        /// <summary>
        /// Full path
        /// </summary>
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// Memberwise copy
        /// </summary>
        /// <returns>FIled by field copy of the entry</returns>
        public virtual FsItem ShallowCopy()
        {
            return (FsItem)this.MemberwiseClone();
        }


    }
}
