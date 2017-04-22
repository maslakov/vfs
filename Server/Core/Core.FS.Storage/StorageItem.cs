using System;
using System.Collections.Generic;
using System.Linq;
using Core.Sessions;

namespace Core.FS.Storage
{
    /// <summary>
    /// Class to store element of VFS. Node in the tree
    /// </summary>
    //internal
    internal class StorageItem : ICloneable
    {
        /// <summary>
        /// Parent node
        /// </summary>
        private StorageItem _parent;

        /// <summary>
        /// Chils nodes
        /// </summary>
        private List<StorageItem> _items;

        /// <summary>
        /// Node content: directory or file
        /// </summary>
        private FsItem _contentItem;

        /// <summary>
        /// List of users, who blocked an element
        /// </summary>
        private List<SID> _lockList;

        /// <summary>
        /// If element is locked or not
        /// </summary>
        public bool IsLocked { get { return _lockList.Count > 0; } }

        /// <summary>
        /// Textual representation of the user list, who locked the element
        /// </summary>
        public string LockList 
        {
            get 
            {
                if (IsLocked)
                    return String.Join(",", _lockList.Select(i => i.Label).ToArray());
                else return string.Empty;

            }
        
        }

        /// <summary>
        /// Node content - file system element
        /// </summary>
        public FsItem Content { get { return _contentItem; } }

        /// <summary>
        /// Parent node
        /// </summary>
        public StorageItem Parent { get { return _parent; } }

        /// <summary>
        /// Update for current node the path, based on real current position in the tree
        /// </summary>
        protected void UpdateContentsPath()
        {
            if (_contentItem != null)
            {
                if (_parent != null)
                {
                    if (_parent.Content != null)
                    {
                        _contentItem.Path = _parent.Content.FullName;
                    }
                    else _contentItem.Path = String.Empty;
                }
                else _contentItem.Path = String.Empty;

                foreach (var child in _items)
                {
                    child.UpdateContentsPath();
                }
            }
        }

        /// <summary>
        /// Attach current node to another one as a child
        /// </summary>
        /// <param name="newParent">New parent node</param>
        public void MoveTo(StorageItem newParent)
        {
            _parent.RemoveChildItem(this);
            if (newParent != null)
            {
                StorageItem exact = newParent.GetChildItems().FirstOrDefault(i => i.Content.Equals(this.Content));
                if (exact == null)
                {
                    newParent.AddChildItem(this);
                    this.UpdateContentsPath();
                }
                else
                {
                    // If are trying to move to the directory with the same name 0 move only children items
                    while (_items.Count > 0)
                    {
                        StorageItem itemToMove = _items.LastOrDefault();
                        if (itemToMove != null) itemToMove.MoveTo(exact);
                    }
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public StorageItem()
        {
            _items = new List<StorageItem>();
            _lockList = new List<SID>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">Value of the item</param>
        public StorageItem(FsItem content)
            : this()
        {
            _contentItem = content;
            UpdateContentsPath();
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">File system element</param>
        /// <param name="parent">Parent node</param>
        public StorageItem(StorageItem parent, FsItem content)
            : this(content)
        {
            _parent = parent;
        }

        /// <summary>
        /// Add new child item to the current one
        /// </summary>
        /// <param name="item">Child item</param>
        public void AddChildItem(StorageItem item)
        {
            lock (item)
            {
                item._parent = this;
                _items.Add(item);
                item.UpdateContentsPath();
            }
        }

        /// <summary>
        /// Get all child nodes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StorageItem> GetChildItems()
        {
            return _items.AsEnumerable();
        }

        /// <summary>
        /// Delete child item
        /// </summary>
        /// <param name="item">Item to be removed</param>
        public void RemoveChildItem(StorageItem item)
        {
            lock (item)
            {
                _items.Remove(item);
                item._parent = null;
                item.UpdateContentsPath();
            }
        }

        /// <summary>
        /// Lock current node
        /// </summary>
        /// <param name="userToken">User who locks the item</param>
        public void Lock(SID userToken)
        {
            if (_lockList.All(i => i.Token != userToken.Token))
            {
                _lockList.Add(userToken);
            }
        }

        /// <summary>
        /// Unlock current node
        /// </summary>
        /// <param name="userToken">User who performs the action</param>
        public void UnLock(SID userToken)
        {
            _lockList.RemoveAll(i => i.Token == userToken.Token);
        }


        public object Clone()
        {
            // copy node value
            FsItem content = this.Content.ShallowCopy();
            // new node
            StorageItem newItem = new StorageItem(content);
            foreach (var i in _items)
            {
                StorageItem newChild = (StorageItem)i.Clone();
                newItem.AddChildItem(newChild);
            }
            // we do not take a locking list
            return newItem;
        }
    }
}
