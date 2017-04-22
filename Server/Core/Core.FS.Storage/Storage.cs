using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.FsExceptions;
using Core.Sessions;

namespace Core.FS.Storage
{
    /// <summary>
    /// Implementation of virtual file system.
    /// Stores a tree of elements: directories and files.
    /// Implemented based on linked lists.
    /// </summary>
    public sealed class VirtualStorage : IStorage
    {
        public const string EmptyElementNameMessage = "The name of the element cannot be empty!";
        public const string RootElementMissingMessage = "Cannot find root element for the path or current directory does not exist!";
        public const string ElementAlreadyExistMessage = "Element with this name already exists!";
        public const string ElementNotFoundMessage = "Element not found!";
        public const string NoIntermediateDiretoryMessage = "There is no intermediate directory exists!";
        public const string CannotDeleteCurrentDirectoryMessage = "Cannot delete current directory!";
        public const string ElementIsNotAFileMessage = "Target element is not a file!";
        public const string ElementIsNotADirectoryMessage = "Target element is not a directory!";
        public const string CannotDeleteSubdirectoriesMessage = "Cannot delete! There are subdirectories!";
        public const string CannotMoveLockedElementMessage = "Cannot move locked item!";
        public const string CannotDeleteLockedItemMessage = "Cannot delete locked item!";
        public const string ElementIsLockedByUsersMessage = "Element is locked by users";

        private enum ItemType {FILE, DIR, TREE};

        /// <summary>
        /// Root elements
        /// </summary>
        private readonly List<Root> _roots;

        /// <summary>
        /// Root elements
        /// </summary>
        public IReadOnlyList<Root> Roots
        {
            get { return _roots.AsReadOnly(); }
        }

        /// <summary>
        /// Filed to store the trees. Every root element (drive) has a tree with "/" directory
        /// </summary>
        private readonly List<StorageItem> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        public VirtualStorage()
        {
            _storage = new List<StorageItem>(1);
            _roots = new List<Root>(1);
            AddRoot(new Root { Label = "C:" });
        }


        #region event handlers
        
        /// <summary>
        /// An element created
        /// </summary>
        public event ItemCreatedEventHandler OnItemCreated;

        /// <summary>
        /// An element deleted
        /// </summary>
        public event ItemDeletedEventHandler OnItemDeleted;

        /// <summary>
        /// An element copied
        /// </summary>
        public event ItemCopyEventHandler OnItemCopy;

        /// <summary>
        /// An element moved
        /// </summary>
        public event ItemMoveEventHandler OnItemMoved;
        
        #endregion

        /// <summary>
        /// Check from which root element starts the path
        /// </summary>
        /// <param name="dirName">Path to check</param>
        /// <returns>Index of the root element</returns>
        private int GetRootForPath(string dirName)
        {
            int rootNum = 0;
            bool match = false;
            do
            {
                match = dirName.StartsWith(_roots[rootNum].Label,StringComparison.InvariantCultureIgnoreCase);
                rootNum++;
            } while (rootNum < Roots.Count && match == false);

            if(match) return rootNum-1;
            else return -1;
        }

        /// <summary>
        /// Fins the cell, which contains an element
        /// </summary>
        /// <param name="searchRoot">Initial cell</param>
        /// <param name="itemToSearch">Element to be search(directory or file)</param>
        /// <returns>Element found or null</returns>
        private StorageItem SearchStorageItem(StorageItem searchRoot, FsItem itemToSearch)
        {
            //if path is equal- return current cell
            if (searchRoot.Content.FullName.ToUpperInvariant() == itemToSearch.FullName.ToUpperInvariant())
            {
                return searchRoot;
            }
            
            StorageItem currentItem = null;
            //Use recursion to search in all children
            IEnumerator<StorageItem> iterator = searchRoot.GetChildItems().GetEnumerator();
            while(iterator.MoveNext() && currentItem == null)
            {
                currentItem = SearchStorageItem(iterator.Current, itemToSearch);
            }

            return currentItem;
        }

        /// <summary>
        /// Find a cell by relative path
        /// </summary>
        /// <param name="searchRoot">Root cell</param>
        /// <param name="pelativePath">relative path</param>
        /// <returns>Node of directory/file or null</returns>
        private StorageItem SearchStorageItem(StorageItem searchRoot, string pelativePath)
        {
            string[] dirs;
            //split path to directories
            dirs = pelativePath.Split(new Char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            int dirCount = dirs.Length;
            int curDir = 0;

            StorageItem targetCell = searchRoot;
            while (curDir < dirCount && targetCell != null)
            {
                targetCell =
                    targetCell
                        .GetChildItems()
                        .FirstOrDefault(i => i.Content.Name.ToUpperInvariant() == dirs[curDir].ToUpperInvariant());

                if (targetCell != null)
                {
                    curDir++;
                    searchRoot = targetCell;
                }
            }
            return targetCell;

        }

        /// <summary>
        /// Find a node with target element
        /// </summary>
        /// <param name="currentDirectory">Current directory</param>
        /// <param name="Path">Search path</param>
        /// <returns>Node or null</returns>
        private StorageItem SearchStorageItem(DirectoryItem currentDirectory, string Path)
        {
            StorageItem root = null;
            root = GetRootForItem(currentDirectory, ref Path);
            if (root == null)
                throw new NotFoundException(RootElementMissingMessage);

            StorageItem targetCell = SearchStorageItem(root, Path);

            return targetCell;
        }


        /// <summary>
        /// Find root element for directory
        /// </summary>
        /// <param name="currentDirectory">Current directory</param>
        /// <param name="newDirName">Path (absolute or relative)</param>
        /// <returns></returns>
        private StorageItem GetRootForItem(DirectoryItem currentDirectory, ref string newDirName)
        {
            //find first currentDirectory in the tree
            StorageItem root = null;

            //if the path absolute one
            int rootNum = GetRootForPath(newDirName);

            if (rootNum < 0)
            {
                //path is relative one. search current directory
                rootNum = 0;
                do
                {
                    root = SearchStorageItem(_storage[rootNum], currentDirectory);
                    rootNum++;
                } while (rootNum < Roots.Count && root == null);

            }
            else
            {
                //absolute path
                root = _storage[rootNum];
                //remove root from the path
                newDirName = newDirName.Substring(_roots[rootNum].Label.Length);
            }
            
            return root;
        }

        /// <summary>
        /// Create an item in file system
        /// </summary>
        /// <param name="currentDirectory">current directory</param>
        /// <param name="newDirName">Path to the element</param>
        /// <param name="userToken">User SID</param>
        /// <param name="type">element type - directory/file</param>
        private void CreateItem(DirectoryItem currentDirectory, string newDirName, SID userToken,ItemType type)
        {
            if(String.IsNullOrEmpty(newDirName))
                throw new ParamsNotAllowedException(EmptyElementNameMessage);

            //search in tree currentDirectory
            StorageItem root = null;
            root = GetRootForItem(currentDirectory, ref newDirName);

            if (root == null)
                throw new NotFoundException(RootElementMissingMessage);

            string[] dirs;
            //split to directories
            dirs = newDirName.Split(new Char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            int dirCount = dirs.Length;
            int curDir = 0;

            StorageItem targetCell = root;
            while (curDir < dirCount && targetCell != null)
            {
                targetCell =
                    targetCell.GetChildItems()
                        .FirstOrDefault(i => i.Content.Name.ToUpperInvariant() == dirs[curDir].ToUpperInvariant());

                if (targetCell != null)
                {
                    curDir++;
                    root = targetCell;
                }
            }
            //directory with number curDir does not exist already
            //if it is not the last one - error
            if (curDir < dirCount - 1)
                throw new ParamsNotAllowedException(NoIntermediateDiretoryMessage);
            else if (curDir == dirCount)
                throw new ParamsNotAllowedException(ElementAlreadyExistMessage);

            string path = root.Content.FullName;
            lock (root)
            {
                if (path != root.Content.FullName)
                    throw new NotAllowedException(ElementNotFoundMessage);

                Console.WriteLine("Creating..");
                //TODO: debug threading
                //Thread.Sleep(5000);

                if (type == ItemType.DIR)
                {
                    // create new child node
                    // put into it new element DirectoryItem with defined name
                    DirectoryItem newDirItem = new DirectoryItem(dirs[curDir]);
                    root.AddChildItem(new StorageItem(newDirItem));

                    // Raise an event
                    if (OnItemCreated != null)
                        OnItemCreated(this, new StorageChangedEventArgs { Item = newDirItem, UserToken = userToken });
                }
                else if (type == ItemType.FILE)
                {
                    // create new child node
                    // put into it new element DirectoryItem with defined name
                    FileItem newDirItem = new FileItem(dirs[curDir]);
                    root.AddChildItem(new StorageItem(newDirItem));

                    // Raise an event
                    if (OnItemCreated != null)
                        OnItemCreated(this, new StorageChangedEventArgs { Item = newDirItem, UserToken = userToken });
                }

                Console.WriteLine("End creating...");
            }
        }


        /// <summary>
        /// Delete file system element
        /// </summary>
        /// <param name="currentDirectory">current folder</param>
        /// <param name="itemToDeleteName">path to the item to be deleted</param>
        /// <param name="userToken">User SID</param>
        /// <param name="type">Type of th item</param>
        private void DeleteItem(DirectoryItem currentDirectory, string itemToDeleteName, SID userToken, ItemType type)
        {
            //try to find a node based on the path
            StorageItem delCell = SearchStorageItem(currentDirectory, itemToDeleteName);
            

            if (delCell == null)
                throw new NotFoundException(ElementNotFoundMessage);
            else
            {
                string path = delCell.Content.FullName;
                //by this moment node can be in absolute different place
                lock (delCell)
                {
                    if (path != delCell.Content.FullName)
                        throw new NotFoundException(ElementNotFoundMessage);

                    Console.WriteLine("Delete....");
                    //TODO: debug threading
                    //Thread.Sleep(5000);

                    if (type == ItemType.FILE && !(delCell.Content is FileItem))
                        throw new NotAllowedException(ElementIsNotAFileMessage);

                    if (type == ItemType.DIR && !(delCell.Content is DirectoryItem))
                        throw new NotAllowedException(ElementIsNotADirectoryMessage);

                    if (type == ItemType.DIR)
                    {
                        // check all sub-folders
                        if (delCell.GetChildItems().Any(i => i.Content is DirectoryItem))
                            throw new NotAllowedException(CannotDeleteSubdirectoriesMessage);

                        if (delCell.Content.FullName.ToUpperInvariant() == currentDirectory.FullName.ToUpperInvariant())
                            throw new NotAllowedException(CannotDeleteCurrentDirectoryMessage);

                        if (delCell.Content.FullName.ToUpperInvariant().StartsWith(currentDirectory.FullName.ToUpperInvariant()))
                            throw new NotAllowedException(CannotDeleteCurrentDirectoryMessage);

                    }

                    if (type == ItemType.TREE)
                    {
                        if (delCell.Content.FullName.ToUpperInvariant() == currentDirectory.FullName.ToUpperInvariant())
                            throw new NotAllowedException(CannotDeleteCurrentDirectoryMessage);

                        if (currentDirectory.FullName.ToUpperInvariant().StartsWith(delCell.Content.FullName.ToUpperInvariant()))
                            throw new NotAllowedException(CannotDeleteCurrentDirectoryMessage);
                    }


                    if (!DeepCheckLocks(delCell))
                    {
                        delCell.Parent.RemoveChildItem(delCell);
                        //result is a deleted node, which parent is null

                        // Raise an event
                        if (OnItemDeleted != null)
                            OnItemDeleted(this, new StorageChangedEventArgs { Item = delCell.Content, UserToken = userToken });
                    }
                    else
                        throw new NotAllowedException(CannotDeleteLockedItemMessage);
                    
                    Console.WriteLine("End delete....");
                }
            }
            
        }

        /// <summary>
        /// Builder of text representation of tree
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="level">Current level</param>
        /// <param name="sb">Accumulator - string builder, which collects the data</param>
        private void GetPrintable(StorageItem root, int level, ref StringBuilder sb)
        {
            if (level > 0)
            {
                for (int i = 1; i < level; i++)
                    sb.Append("| ");
                sb.Append("|_");
            }
            if (root.IsLocked)
            {
                sb.Append(root.Content.Name);
                sb.AppendFormat(String.Format("[LOCKED BY {0}]{1}", root.LockList ,Environment.NewLine));
            }
            else sb.AppendLine(root.Content.Name);

            // order lexicographically
            foreach (var child in root.GetChildItems().OrderBy(i=>i.Content.Name))
            {
                GetPrintable(child, level + 1, ref sb);
            }
        }

        /// <summary>
        /// Recursive check of element for locking
        /// </summary>
        /// <param name="root">node to be checked</param>
        /// <returns>If node is locked or not</returns>
        private bool DeepCheckLocks (StorageItem root)
        {
            bool locked = false;

            if (root.Content is FileItem)
            {
                return root.IsLocked;
            }
            else
                if (root.Content is DirectoryItem)
                {
                    IEnumerator<StorageItem> iterator = root.GetChildItems().GetEnumerator();
                    // go through children
                    while(iterator.MoveNext() && ! locked)
                    {
                        locked = DeepCheckLocks(iterator.Current);
                    }
                }
            return locked;
        }

        /// <summary>
        /// Add new root
        /// </summary>
        /// <param name="label">Root drive label</param>
        public Root AddRoot(String label)
        {
            Root newRoot = new Root() { Label = label };
            return AddRoot(newRoot);
        }

        /// <summary>
        /// Add new root
        /// </summary>
        /// <param name="newRoot">Complete element to be added to storage</param>
        public Root AddRoot(Root newRoot)
        {
            //index of the root and its tree are always equal
            _roots.Add(newRoot);
            DirectoryItem rootDir = new DirectoryItem("\\");
            _storage.Add(new StorageItem(null, rootDir));
            rootDir.Path =  newRoot.Label;
            return newRoot ;
        }

        /// <summary>
        /// Search for an element by defined path in tree or in current folder, if root is not specified
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="pathToSearch">Target path</param>
        /// <param name="userToken">User descriptor</param>
        /// <returns>File system element</returns>
        public FsItem FindItem(DirectoryItem currentDirectory, string pathToSearch, SID userToken)
        {
            FsItem resultItem = null;
            StorageItem targetCell = null;
            targetCell = SearchStorageItem(currentDirectory, pathToSearch);

            //directory with number curDir does not exist
            //if not the last one - error
            if (targetCell == null)
                throw new NotFoundException(ElementNotFoundMessage);
            else
            {
                resultItem = targetCell.Content;
            }
            return resultItem;

        }


        /// <summary>
        /// Create new directory in current directory or in other place, if absolute path is provided
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="pathToCreate">new folder path</param>
        /// <param name="userToken">User descriptor</param>
        public void CreateDirectory(DirectoryItem currentDirectory, string pathToCreate, SID userToken)
        {
            CreateItem(currentDirectory, pathToCreate, userToken, ItemType.DIR);
        }

        /// <summary>
        /// Create new directory in current directory or in other place, if absolute path is provided
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="filePathToCreate">New file path</param>
        /// <param name="userToken">User descriptor</param>
        public void CreateFile(DirectoryItem currentDirectory, string filePathToCreate, SID userToken)
        {
            CreateItem(currentDirectory, filePathToCreate, userToken, ItemType.FILE);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="itemToDeleteName">File path to be deleted</param>
        /// <param name="userToken">User descriptor</param>
        public void DeleteFile(DirectoryItem currentDirectory, string itemToDeleteName, SID userToken)
        {
            DeleteItem(currentDirectory, itemToDeleteName, userToken, ItemType.FILE);
        }

        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="itemToDeleteName">Path to folder to be deleted</param>
        /// <param name="userToken">User descriptor</param>
        public void DeleteDirectory(DirectoryItem currentDirectory, string itemToDeleteName, SID userToken)
        {
            DeleteItem(currentDirectory, itemToDeleteName, userToken, ItemType.DIR);
        }

        /// <summary>
        /// Delete directory with all sub-folders
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="itemToDeleteName">Path to root folder to be deleted</param>
        /// <param name="userToken">User descriptor</param>
        public void DeleteItemTree(DirectoryItem currentDirectory, string itemToDeleteName, SID userToken)
        {
            DeleteItem(currentDirectory, itemToDeleteName, userToken, ItemType.TREE);
        }


        /// <summary>
        /// Move file or directory
        /// </summary>
        /// <param name="oldItemPath">Existing item path</param>
        /// <param name="newItemPath">New directory or file path</param>
        /// <param name="userToken">User descriptor</param>
        public void MoveItem(DirectoryItem currentDirectory, string oldItemPath, string newItemPath, SID userToken)
        {
            StorageItem fromCell = SearchStorageItem(currentDirectory, oldItemPath);
            StorageItem targetCell = SearchStorageItem(currentDirectory, newItemPath);

            if (fromCell == null)
                throw new NotFoundException(ElementNotFoundMessage);
            if (targetCell == null)
                throw new NotFoundException(ElementNotFoundMessage);

            
            FsItem fromItem = fromCell.Content;
            FsItem toItem = targetCell.Content;

            string pathFrom = fromCell.Content.FullName;
            string pathTo = targetCell.Content.FullName;

            // Only folder can be moved
            if (!(toItem is DirectoryItem))
                throw new NotAllowedException(ElementIsNotADirectoryMessage);

            if (fromItem is FileItem)
            {
                lock (fromCell)
                {
                    lock (targetCell)
                    {
                        if (pathFrom != fromCell.Content.FullName ||
                            pathTo != targetCell.Content.FullName)
                            throw new NotAllowedException(ElementNotFoundMessage);

                        // check if file is not locked
                        if (fromCell.IsLocked)
                            throw new NotAllowedException(String.Format("{1}: [{0}]", ElementIsLockedByUsersMessage, targetCell.LockList));

                        if (targetCell.GetChildItems().Any(i=>i.Content.Name.ToUpperInvariant() == fromItem.Name.ToUpperInvariant()))
                            throw new NotAllowedException(ElementAlreadyExistMessage);


                        Console.WriteLine("Begin move....");
                        //TODO: debug threading
                        //Thread.Sleep(5000);
                        
                        fromCell.MoveTo(targetCell);

                        Console.WriteLine("End move....");

                    }
                }
                
            }
            else if (fromItem is DirectoryItem)
            {
                // directory must be checked if there are locked items inside
                lock (fromCell)
                {
                    lock (targetCell)
                    {
                        if (pathFrom != fromCell.Content.FullName ||
                            pathTo != targetCell.Content.FullName)
                            throw new NotAllowedException(ElementNotFoundMessage);

                        Console.WriteLine("Begin move....");
                        //TODO: debug threading
                        //Thread.Sleep(5000);

                        if (!DeepCheckLocks(fromCell))
                        {
                            fromCell.MoveTo(targetCell);
                        }
                        else
                            throw new NotAllowedException(CannotMoveLockedElementMessage);

                        Console.WriteLine("End move....");
                    }
                }
            }

            //Raise an event
            if (OnItemMoved != null)
                OnItemMoved(this, new StorageChangedEventArgs { Item = fromItem, UserToken = userToken });

        }


        
        /// <summary>
        /// Copy directory or file
        /// </summary>
        /// <param name="oldItemPath">From where path</param>
        /// <param name="newItemPath">To where path</param>
        /// <param name="userToken">User descriptor</param>
        public void CopyItem(DirectoryItem currentDirectory, string oldItemPath, string newItemPath, SID userToken)
        {
            StorageItem fromCell = SearchStorageItem(currentDirectory, oldItemPath);
            StorageItem targetCell = SearchStorageItem(currentDirectory, newItemPath);

            if (fromCell == null)
                throw new NotFoundException(ElementNotFoundMessage);
            if (targetCell == null)
                throw new NotFoundException(ElementNotFoundMessage);

            FsItem fromItem = fromCell.Content;
            FsItem toItem = targetCell.Content;

            string pathFrom = fromCell.Content.FullName;
            string pathTo = targetCell.Content.FullName;

            // target can be only directory
            if (!(toItem is DirectoryItem))
                throw new NotAllowedException(ElementIsNotADirectoryMessage);

            if (fromItem is FileItem)
            {
                // directory must be checked if there are locked items inside
                lock (fromCell)
                {
                    lock (targetCell)
                    {
                        if (pathFrom != fromCell.Content.FullName ||
                            pathTo != targetCell.Content.FullName)
                            throw new NotAllowedException(ElementNotFoundMessage);

                        //TODO: debug threading
                        Console.WriteLine("Begin copy....");
                        //Thread.Sleep(5000);
                        
                        // Create a copy
                        FileItem copyItem = new FileItem(fromItem.Name);
                        // Add to target node new child
                        targetCell.AddChildItem(new StorageItem(copyItem));

                        Console.WriteLine("End copy....");
                    }
                }
            }
            else if (fromItem is DirectoryItem)
            {
                lock (fromCell)
                {
                    lock (targetCell)
                    {
                        if (pathFrom != fromCell.Content.FullName ||
                            pathTo != targetCell.Content.FullName)
                            throw new NotAllowedException(ElementNotFoundMessage);

                        //TODO: debug threading
                        Console.WriteLine("Begin copy....");
                        //Thread.Sleep(5000);

                        StorageItem cloneRoot = (StorageItem)fromCell.Clone();

                        targetCell.AddChildItem(cloneRoot);


                        Console.WriteLine("End copy....");
                    }
                }
            }

            // Raise an event
            if (OnItemCopy != null)
                OnItemCopy(this, new StorageChangedEventArgs { Item = fromItem, UserToken = userToken });

        }

        /// <summary>
        /// Returns textual representation of file system tree
        /// </summary>
        public string GetPrintableView(SID userToken)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _roots.Count; i++)
            {
                sb.Append(_roots[i].Label);
                GetPrintable(_storage[i], 0, ref sb);
            }
            return sb.ToString();

        }


        /// <summary>
        /// Lock element
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="itemName">Path to be locked</param>
        /// <param name="userToken">User</param>
        public void LockItem(DirectoryItem currentDirectory, string itemName, SID userToken)
        {
            StorageItem targetCell = SearchStorageItem(currentDirectory, itemName);

            //directory with number curDir does not exist
            //if not the last one - error
            if (targetCell == null)
                throw new NotFoundException(ElementNotFoundMessage);
            else
            {
                string path = targetCell.Content.FullName;

                lock (targetCell) {

                    if (path != targetCell.Content.FullName)
                        throw new NotAllowedException(ElementNotFoundMessage);

                    if (targetCell.Content is FileItem)
                        targetCell.Lock(userToken);
                    else throw new NotAllowedException(ElementIsNotAFileMessage);
                }
            }


        }

        /// <summary>
        /// Unlock element
        /// </summary>
        /// <param name="currentDirectory">Current folder</param>
        /// <param name="itemName">Path to unlock</param>
        /// <param name="userToken">User</param>
        public void UnLockItem(DirectoryItem currentDirectory, string itemName, SID userToken)
        {
            StorageItem targetCell = SearchStorageItem(currentDirectory, itemName);

            //directory with number curDir does not exist
            //if not the last one - error
            if (targetCell == null)
                throw new NotFoundException(ElementNotFoundMessage);
            
            string path = targetCell.Content.FullName;
            lock (targetCell)
            {
                if (path != targetCell.Content.FullName)
                    throw new NotAllowedException(ElementNotFoundMessage);

                targetCell.UnLock(userToken);
            }
        }
    }
}
