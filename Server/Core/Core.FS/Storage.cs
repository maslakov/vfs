using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS
{
    /// <summary>
    /// Класс отвечающий за хранилище дерева элементов VFS. Виртуальная реализация
    /// </summary>
    public class Storage
    {
        //TODO:рассмотреть возможность извлечения интерфейса

       /// <summary>
       /// Корневые элементы
       /// </summary>
       private List<Root> _roots;

        /// <summary>
        /// Корневые элементы
        /// </summary>
        public List<Root> Roots
        {
            get { return _roots; }
        }

        /// <summary>
        /// Поле для хранения деревьев. Каждому корневому элементу соответствует дерево, 
        /// начинающееся с каталога "\". Доступ по индексу.
        /// </summary>
        private List<StorageItem> _storage;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Storage()
        {
            _storage = new List<StorageItem>(1);
            _roots = new List<Root>(1);
            _roots.Add(new Root { Label = "C:" });
        }

        /// <summary>
        /// Добавление корня
        /// </summary>
        /// <param name="label"></param>
        public Root AddRoot(String label)
        {
            Root newRoot = new Root() { Label = label };
            return AddRoot(newRoot);
        }

        /// <summary>
        /// Добавление корня
        /// </summary>
        /// <param name="label"></param>
        public Root AddRoot(Root newRoot)
        {
            //интекс корня и его дерева всегда равны
            _roots.Add(newRoot);
            _storage.Add(new StorageItem(new DirectoryItem("\\")));
            return newRoot ;
        }


        internal void CreateDirectory(DirectoryItem _currentDirectory, string _newDirName)
        {
            throw new NotImplementedException();
        }
    }
}
