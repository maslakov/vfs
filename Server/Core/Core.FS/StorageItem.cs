using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS
{
    /// <summary>
    /// класс для хранения дерева элементов VFS. Нода дерева
    /// </summary>
    internal class StorageItem
    {
        /// <summary>
        /// родительский элемент
        /// </summary>
        private StorageItem _parent;

        /// <summary>
        /// Вложенные элементы
        /// </summary>
        private List<StorageItem> _items;

        /// <summary>
        /// полезная нагрузка узла дерева, каталог или файл
        /// </summary>
        private IFsItem _contentItem;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="content"></param>
        public StorageItem(IFsItem content)
        {
            _contentItem = content;
        }

        public void AddItem(StorageItem item)
        {
            _items.Add(item);
        }

        public IEnumerable<StorageItem> GetItems()
        {
            return _items.AsEnumerable();
        }

        public void Remove(StorageItem item)
        {
            _items.Remove(item);
        }


    }
}
