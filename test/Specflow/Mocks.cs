//// Copyright (c) Kaylumah, 2022. All rights reserved.
//// See LICENSE file in the project root for full license information.

//using System.IO.Abstractions;
//using Microsoft.Extensions.FileProviders;
//using Moq;

//namespace Test.Specflow;

//public class StrictMock<T> : Mock<T> where T : class
//{
//    public StrictMock() : base(MockBehavior.Strict) { }
//}

//public class CollectionMock<TCollection, TItem>
//    : StrictMock<TCollection> where TCollection : class, IEnumerable<TItem>, IEnumerable
//{
//    private List<TItem> _items;

//    public CollectionMock(List<TItem> items)
//    {
//        _items = items;
//        Setup();
//    }

//    public CollectionMock<TCollection, TItem> Setup()
//    {
//        Setup(x => x.GetEnumerator())
//            .Returns(() => _items.GetEnumerator());
//        return this;
//    }
//}

//public class FileInfoMock : StrictMock<IFileInfo>
//{
//}

//public class DirectoryContentsMock : CollectionMock<IDirectoryContents, IFileInfo>
//{
//    public DirectoryContentsMock(List<IFileInfo> items) : base(items) { }
//}

//public class MockFileSystem : Mock<IFileSystem>
//{
//    public MockFileSystem() : base(MockBehavior.Strict) { }

//    public MockFileSystem SetupGetDirectoryContents()
//    {
//        var items = new List<Microsoft.Extensions.FileProviders.IFileInfo>()
//        {
//            new FileInfoMock().Object
//        };
//        var mockDirectory = new DirectoryContentsMock(items).Object;

//        Setup(fileSystem =>
//            fileSystem.GetDirectoryContents(It.IsAny<string>()))
//        .Returns(mockDirectory);
//        return this;
//    }
//}
