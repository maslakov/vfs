using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Sessions;
using System.Diagnostics;
using Core.FsExceptions;
using Core.FS.Storage;


namespace Core.FS.Test
{
    [TestClass]
    public class StorageTests
    {
        [TestMethod]
        public void TestStorageCreate()
        {
            Core.FS.Storage.VirtualStorage storage = new Core.FS.Storage.VirtualStorage();
            storage.AddRoot(new Root() { Label = "D:" });

            DirectoryItem d = new DirectoryItem("\\","D:");
            DirectoryItem c = new DirectoryItem("\\", "C:");

            Assert.AreEqual(storage.Roots.Count, 2);
            
            storage.CreateDirectory(d, "C:\\NEW1", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(c, "NEW1\\TEST2", SID.Empty);

           try
            {
                storage.CreateDirectory(c, "D:\\NEW1\\TEST2", SID.Empty);
                Assert.Fail("Exception must be thrown here");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(BaseVFSException));
                Assert.AreEqual(e.Message, VirtualStorage.NoIntermediateDiretoryMessage);            
            }

            

            storage.CreateDirectory(d, "C:\\NEW1\\TEST\\A", SID.Empty);

            try
            {
                storage.CreateDirectory(d, "C:\\NEW1\\TEST4\\A", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ParamsNotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.NoIntermediateDiretoryMessage);            
            }


            try
            {
                storage.CreateDirectory(d, "C:\\NEW1\\TEST", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ParamsNotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementAlreadyExistMessage);
            }

            try
            {
                DirectoryItem neew_root = new DirectoryItem("\\", "E:");
                storage.CreateDirectory(neew_root, "E:\\NEW1\\TEST", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.RootElementMissingMessage);
            }

             try
            {
                storage.CreateDirectory(d, "", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ParamsNotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.EmptyElementNameMessage);
            }
       }

        [TestMethod]
        public void TestStorageFind()
        {
            Core.FS.Storage.VirtualStorage storage = new Core.FS.Storage.VirtualStorage();
            Assert.AreEqual(storage.Roots[0].Label, "C:");

            storage.AddRoot(new Root { Label = "D:" });
            Assert.AreEqual(storage.Roots.Count, 2);

            DirectoryItem c = new DirectoryItem("\\", "C:");

            storage.CreateDirectory(c, "C:\\NEW1", SID.Empty);

            storage.CreateDirectory(c, "NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(c, "NEW1\\TEST2", SID.Empty);

            storage.CreateFile(c, "C:\\NEW1\\TEST\\A.txt", SID.Empty);

            FsItem find = storage.FindItem(c, "C:\\NEW1\\TEST\\A.txt", SID.Empty);
            
            Assert.AreEqual(find.Path,"C:\\NEW1\\TEST");
            
            try
            {
                FsItem find1 = storage.FindItem(c, "C:\\NEW1\\TEST\\B.txt", SID.Empty);
            }
            catch (Exception e)
            {

                Assert.IsInstanceOfType(e, typeof(NotFoundException));
            }
            
        }

        [TestMethod]
        public void TestStorageFiles()
        {
            Core.FS.Storage.VirtualStorage storage = new Core.FS.Storage.VirtualStorage();
            storage.AddRoot(new Root { Label = "D:" });
            Assert.AreEqual(storage.Roots.Count, 2);

            DirectoryItem d = new DirectoryItem("\\", "D:");
            DirectoryItem c = new DirectoryItem("\\", "C:");

            storage.CreateDirectory(d, "C:\\NEW1", SID.Empty);

            storage.CreateDirectory(d, "NEW1", SID.Empty);

            storage.CreateDirectory(d, "NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST2", SID.Empty);

            try
            {
                storage.CreateFile(c, "NEW1\\TEST2\\A\\1.txt", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ParamsNotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.NoIntermediateDiretoryMessage);            

            }

            storage.CreateDirectory(c, "C:\\NEW1\\TEST\\A", SID.Empty);

            storage.CreateFile(d, "C:\\NEW1\\TEST\\A.txt", SID.Empty);


            try
            {
                storage.CreateFile(d, "C:\\NEW1\\TEST\\A.txt", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ParamsNotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementAlreadyExistMessage);
            }

            try
            {
                DirectoryItem neew_root = new DirectoryItem("\\", "E:");
                storage.CreateDirectory(neew_root, "E:\\NEW1\\TEST\\4.txt", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.RootElementMissingMessage );

            }

            try
            {
                storage.CreateFile(d, "", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ParamsNotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.EmptyElementNameMessage);

            }


            FsItem find = storage.FindItem(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateFile(d, "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);

            storage.CreateFile(d, "\\NEW1\\asd.txt", SID.Empty);

            storage.CreateFile(find as DirectoryItem, "blabla.txt", SID.Empty);

            Debug.Print(storage.GetPrintableView(SID.Empty));

            storage.MoveItem(find as DirectoryItem, "blabla.txt", "D:\\NEW1\\",SID.Empty);

            Debug.Print(storage.GetPrintableView(SID.Empty));

            //folders with the same name

            storage.MoveItem(find as DirectoryItem, "C:\\NEW1\\TEST", "D:\\NEW1\\", SID.Empty);
                        
            FsItem find_D = storage.FindItem(d, "NEW1\\TEST\\A\\qwe.txt", SID.Empty);

            //moved from С:
            Assert.AreEqual(find_D.FullName, "D:\\NEW1\\TEST\\A\\qwe.txt");

        }


        [TestMethod]
        public void TestStorageDelete()
        {
            Core.FS.Storage.VirtualStorage storage = new Core.FS.Storage.VirtualStorage();

            storage.AddRoot(new Root { Label = "D:" });
            Assert.AreEqual(storage.Roots.Count, 2);

            DirectoryItem d = new DirectoryItem("\\", "D:");
            DirectoryItem c = new DirectoryItem("\\", "C:");

            storage.CreateDirectory(d, "C:\\NEW1", SID.Empty);

            storage.CreateDirectory(d, "NEW1", SID.Empty);

            storage.CreateDirectory(d, "NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST2", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST\\A", SID.Empty);


            FsItem find = storage.FindItem(d, "C:\\NEW1\\TEST", SID.Empty);
            
            Assert.IsNotNull(find);

            storage.CreateFile(d, "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);
            storage.CreateFile(d, "C:\\NEW1\\TEST\\A\\rty.txt", SID.Empty);

            storage.CreateFile(d, "\\NEW1\\asd.txt", SID.Empty);

            storage.CreateFile(find as DirectoryItem, "blabla.txt", SID.Empty);

            Debug.Print(storage.GetPrintableView(SID.Empty));


            try
            {
                storage.DeleteDirectory(d, "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementIsNotADirectoryMessage);

            }

            try
            {
                storage.DeleteFile(d, "C:\\NEW1\\TEST\\A", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementIsNotAFileMessage);

            }

            try
            {
                storage.DeleteFile(d, "C:\\NEW1\\TEST\\A\\LALA.txt", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementNotFoundMessage);

            }

            try
            {
                storage.DeleteDirectory(d, "C:\\NEW1\\TEST\\B", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementNotFoundMessage);

            }


            try
            {
                storage.DeleteDirectory(d, "C:\\NEW1", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.CannotDeleteSubdirectoriesMessage);

            }

            try
            {
                FsItem find_A = storage.FindItem(d, "C:\\NEW1\\TEST\\A", SID.Empty);
                storage.DeleteDirectory(find_A as DirectoryItem, "C:\\NEW1\\TEST\\A", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.CannotDeleteCurrentDirectoryMessage);

            }

            try
            {
                FsItem find_A = storage.FindItem(d, "C:\\NEW1\\TEST\\A", SID.Empty);
                storage.DeleteItemTree(find_A as DirectoryItem, "C:\\NEW1\\", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.CannotDeleteCurrentDirectoryMessage);

            }

            storage.DeleteFile(d, "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);

            storage.LockItem(d, "C:\\NEW1\\TEST\\A\\rty.txt", new SID { Token = "User1", Label = "user1" });

            try
            {
                storage.DeleteFile(d, "C:\\NEW1\\TEST\\A\\rty.txt", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.CannotDeleteLockedItemMessage);

            }

            try
            {
                storage.DeleteDirectory(d, "C:\\NEW1\\TEST\\A", SID.Empty);
                Assert.Fail("Exception must be thrown");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.CannotDeleteLockedItemMessage);

            }

            storage.UnLockItem(d, "C:\\NEW1\\TEST\\A\\rty.txt", new SID { Token = "User1", Label = "user1" });

            storage.DeleteItemTree(find as DirectoryItem, "C:\\NEW1\\TEST\\A", SID.Empty);

            storage.DeleteDirectory(d as DirectoryItem, "C:\\NEW1\\TEST", SID.Empty);

        }
           

        [TestMethod]
        public void TestStorageCopy()
        {
            Core.FS.Storage.VirtualStorage storage = new Core.FS.Storage.VirtualStorage();
            storage.AddRoot(new Root { Label = "D:" });

            DirectoryItem d = new DirectoryItem("\\", "D:");

            storage.CreateDirectory(d, "C:\\NEW1", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST2", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST\\A", SID.Empty);


            FsItem find = storage.FindItem(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateFile(d, "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);

            storage.CreateFile(find as DirectoryItem, "blabla.txt", SID.Empty);


            try
            {
                storage.CopyItem(d, "C:\\NEW1", "E:\\", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementNotFoundMessage);

            }

            try
            {
                storage.CopyItem(d, "E:\\NEW1", "D:\\", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementNotFoundMessage);

            }

            try
            {
                storage.CopyItem(d, "C:\\NEW1", "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementIsNotADirectoryMessage);

            }
            
            
            storage.CopyItem(find as DirectoryItem, "A", "D:\\", SID.Empty);

        }


        [TestMethod]
        public void TestStorageMove()
        {
            Core.FS.Storage.VirtualStorage storage = new Core.FS.Storage.VirtualStorage();
            storage.AddRoot(new Root { Label = "D:" });

            DirectoryItem d = new DirectoryItem("\\", "D:");

            storage.CreateDirectory(d, "C:\\NEW1", SID.Empty);

            storage.CreateDirectory(d, "NEW1", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateDirectory(d, "NEW1\\TEST", SID.Empty);


            storage.CreateDirectory(d, "C:\\NEW1\\TEST2", SID.Empty);

            storage.CreateDirectory(d, "C:\\NEW1\\TEST\\A", SID.Empty);


            FsItem find = storage.FindItem(d, "C:\\NEW1\\TEST", SID.Empty);

            storage.CreateFile(d, "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);
            storage.CreateFile(d, "C:\\NEW1\\TEST\\A\\rty.txt", SID.Empty);

            storage.CreateFile(find as DirectoryItem, "blabla.txt", SID.Empty);


            try
            {
                storage.MoveItem(d, "C:\\NEW1", "E:\\", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementNotFoundMessage);

            }

            try
            {
                storage.MoveItem(d, "E:\\NEW1", "D:\\", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotFoundException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementNotFoundMessage);

            }

            try
            {
                storage.MoveItem(d, "NEW1", "C:\\NEW1\\TEST\\A\\qwe.txt", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementIsNotADirectoryMessage);

            }

            storage.LockItem(d, "C:\\NEW1\\TEST\\A\\rty.txt", new SID { Token = "User1", Label = "user1" });

            try
            {
                storage.MoveItem(d, "C:\\NEW1\\TEST\\A\\rty.txt", "NEW1", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.IsTrue(e.Message.StartsWith(VirtualStorage.ElementIsLockedByUsersMessage));

            }


            try
            {
                storage.MoveItem(d, "C:\\NEW1\\TEST", "NEW1", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.CannotMoveLockedElementMessage);

            }

            storage.UnLockItem(d, "C:\\NEW1\\TEST\\A\\rty.txt", new SID { Token = "User1", Label = "user1" });

            storage.MoveItem(d, "C:\\NEW1\\TEST\\A\\rty.txt", "NEW1", SID.Empty);
            storage.CreateFile(d, "C:\\NEW1\\TEST\\A\\rty.txt", SID.Empty);

            try
            {
                storage.MoveItem(d, "C:\\NEW1\\TEST\\A\\rty.txt", "NEW1", SID.Empty);
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(NotAllowedException));
                Assert.AreEqual(e.Message, VirtualStorage.ElementAlreadyExistMessage);

            }



        }
       
    }
}

