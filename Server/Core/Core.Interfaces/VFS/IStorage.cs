using System.Collections.Generic;
using System;
using Core.Sessions;
using Core.FS;

namespace Core.FS
{
    public delegate void ItemCreatedEventHandler(object sender, StorageChangedEventArgs e);
    public delegate void ItemDeletedEventHandler(object sender, StorageChangedEventArgs e);
    public delegate void ItemCopyEventHandler(object sender, StorageChangedEventArgs e);
    public delegate void ItemMoveEventHandler(object sender, StorageChangedEventArgs e);

    /// <summary>
    /// Virtual file system storage interface
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Add root element
        /// </summary>
        /// <param name="newRoot">New root element object</param>
        /// <returns>New root element object</returns>
        Root AddRoot(Root newRoot);

        /// <summary>
        /// Add root element
        /// </summary>
        /// <param name="label">Root element label (drive letter)</param>
        /// <returns>New root element object</returns>
        Root AddRoot(string label);

        /// <summary>
        /// Copy directory or file
        /// </summary>
        /// <param name="currentDirectory">Current folder of the user</param>
        /// <param name="oldItemPath">From where path</param>
        /// <param name="newItemPath">To where path</param>
        /// <param name="userToken">User descriptor</param>
        void CopyItem(DirectoryItem currentDirectory, string oldItemPath, string newItemPath, SID userToken);
        
        /// <summary>
        /// Create directory in current folder
        /// </summary>
        /// <param name="currentDirectory">Current folder of the user</param>
        /// <param name="newDirName">New directory path</param>
        /// <param name="userToken">User descriptor</param>
        void CreateDirectory(DirectoryItem currentDirectory, string newDirName, SID userToken);
        
        /// <summary>
        /// Create file
        /// </summary>
        /// <param name="currentDirectory"></param>
        /// <param name="newDirName"></param>
        /// <param name="userToken">User descriptor</param>
        void CreateFile(DirectoryItem currentDirectory, string newDirName, SID userToken);
        
        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="currentDirectory"></param>
        /// <param name="itemToDeleteName"></param>
        /// <param name="userToken">User descriptor</param>
        void DeleteDirectory(DirectoryItem currentDirectory, string itemToDeleteName, SID userToken);
        
        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="currentDirectory"></param>
        /// <param name="itemToDeleteName"></param>
        /// <param name="userToken">User descriptor</param>
        void DeleteFile(DirectoryItem currentDirectory, string itemToDeleteName, SID userToken);
        
        /// <summary>
        /// Delete directory tree
        /// </summary>
        /// <param name="currentDirectory"></param>
        /// <param name="itemToDeleteName"></param>
        /// <param name="userToken">User descriptor</param>
        void DeleteItemTree(DirectoryItem currentDirectory, string itemToDeleteName, SID userToken);
        
        /// <summary>
        /// Find element
        /// </summary>
        /// <param name="currentDirectory"></param>
        /// <param name="newDirName"></param>
        /// <param name="userToken">User descriptor</param>
        /// <returns>Item object or null</returns>
        FsItem FindItem(DirectoryItem currentDirectory, string newDirName, SID userToken);
        
        
        /// <summary>
        /// Get printable version of tree
        /// </summary>
        /// <param name="userToken">User descriptor</param>
        /// <returns>Tree view of file system</returns>
        string GetPrintableView(SID userToken);
        
        /// <summary>
        /// Lock element
        /// </summary>
        /// <param name="currentDirectory">Current directory</param>
        /// <param name="itemName">Name of the file/directory</param>
        /// <param name="userToken">User descriptor</param>
        void LockItem(DirectoryItem currentDirectory, string itemName, SID userToken);
        
        /// <summary>
        /// Unlock an item
        /// </summary>
        /// <param name="currentDirectory">Current directory</param>
        /// <param name="itemName">Name of the file/directory</param>
        /// <param name="userToken">User descriptor</param>
        void UnLockItem(DirectoryItem currentDirectory, string itemName, SID userToken);

        /// <summary>
        /// Move element
        /// </summary>
        /// <param name="currentDirectory">current foler</param>
        /// <param name="oldItemPath">Existing path</param>
        /// <param name="newItemPath">Target path</param>
        /// <param name="userToken">User descriptor</param>
        void MoveItem(DirectoryItem currentDirectory, string oldItemPath, string newItemPath, SID userToken);
        
        /// <summary>
        /// Copied event
        /// </summary>
        event ItemCopyEventHandler OnItemCopy;

        /// <summary>
        /// Created event
        /// </summary>
        event ItemCreatedEventHandler OnItemCreated;

        /// <summary>
        /// Deleted event
        /// </summary>
        event ItemDeletedEventHandler OnItemDeleted;

        /// <summary>
        /// Moved event
        /// </summary>
        event ItemMoveEventHandler OnItemMoved;
        
        /// <summary>
        /// List of all root elemnts
        /// </summary>
        IReadOnlyList<Root> Roots { get; }

    }
}
