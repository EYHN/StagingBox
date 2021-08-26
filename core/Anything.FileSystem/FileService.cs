using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anything.FileSystem.Impl;
using Anything.FileSystem.Provider;
using Anything.FileSystem.Tracker;
using Anything.FileSystem.Walker;
using Anything.Utils;
using Anything.Utils.Event;
using FileNotFoundException = Anything.FileSystem.Exception.FileNotFoundException;

namespace Anything.FileSystem
{
    public class FileService : Disposable, IFileService
    {
        private readonly List<IDisposable> _disposables = new();
        private readonly EventEmitter<FileEvent[]> _fileEventEmitter = new();
        private readonly IDictionary<Url, IFileSystem> _fileSystems = new Dictionary<Url, IFileSystem>();

        /// <inheritdoc />
        public ValueTask CreateDirectory(Url url)
        {
            return GetFileSystemByUrl(url).CreateDirectory(url);
        }

        /// <inheritdoc />
        public ValueTask Delete(Url url, bool recursive)
        {
            return GetFileSystemByUrl(url).Delete(url, recursive);
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<(string Name, FileStats Stats)>> ReadDirectory(Url url)
        {
            return GetFileSystemByUrl(url).ReadDirectory(url);
        }

        /// <inheritdoc />
        public ValueTask<byte[]> ReadFile(Url url)
        {
            return GetFileSystemByUrl(url).ReadFile(url);
        }

        /// <inheritdoc />
        public ValueTask Rename(Url oldUrl, Url newUrl, bool overwrite)
        {
            var oldUrlFileSystem = GetFileSystemByUrl(oldUrl);
            var newUrlFileSystem = GetFileSystemByUrl(newUrl);

            if (ReferenceEquals(oldUrlFileSystem, newUrlFileSystem))
            {
                return oldUrlFileSystem.Rename(oldUrl, newUrl, overwrite);
            }

            throw new InvalidOperationException("not in same file system");
        }

        /// <inheritdoc />
        public ValueTask<FileStats> Stat(Url url)
        {
            return GetFileSystemByUrl(url).Stat(url);
        }

        /// <inheritdoc />
        public ValueTask WriteFile(Url url, byte[] content, bool create = true, bool overwrite = true)
        {
            return GetFileSystemByUrl(url).WriteFile(url, content, create, overwrite);
        }

        public ValueTask ReadFileStream(Url url, Func<Stream, ValueTask> reader)
        {
            return GetFileSystemByUrl(url).ReadFileStream(url, reader);
        }

        /// <inheritdoc />
        public ValueTask<T> ReadFileStream<T>(Url url, Func<Stream, ValueTask<T>> reader)
        {
            return GetFileSystemByUrl(url).ReadFileStream(url, reader);
        }

        /// <inheritdoc />
        public Event<FileEvent[]> FileEvent => _fileEventEmitter.Event;

        /// <inheritdoc />
        public ValueTask AttachData(Url url, FileRecord fileRecord, FileAttachedData data)
        {
            return GetFileSystemByUrl(url).AttachData(url, fileRecord, data);
        }

        /// <inheritdoc cref="IFileSystem.WaitComplete" />
        public ValueTask WaitComplete()
        {
            return new(Task.WhenAll(_fileSystems.Select(item => item.Value.WaitComplete().AsTask())));
        }

        /// <inheritdoc />
        public ValueTask WaitFullScan()
        {
            return new(Task.WhenAll(_fileSystems.Select(item => item.Value.WaitFullScan().AsTask())));
        }

        /// <inheritdoc />
        public IFileSystemWalker CreateWalker(Url rootUrl)
        {
            return GetFileSystemByUrl(rootUrl).CreateWalker(rootUrl);
        }

        public ValueTask Copy(Url source, Url destination, bool overwrite)
        {
            var sourceFileSystem = GetFileSystemByUrl(source);
            var destinationFileSystem = GetFileSystemByUrl(source);

            if (ReferenceEquals(sourceFileSystem, destinationFileSystem))
            {
                return sourceFileSystem.Copy(source, destination, overwrite);
            }

            throw new InvalidOperationException("not in same file system");
        }

        public string? ToLocalPath(Url url)
        {
            return GetFileSystemByUrl(url).ToLocalPath(url);
        }

        public void AddTestFileSystem(Url rootUrl, IFileSystemProvider fileSystemProvider)
        {
            var testFileSystem = new TestFileSystem(rootUrl, fileSystemProvider);
            AddFileSystem(rootUrl, testFileSystem);
            _disposables.Add(testFileSystem);
        }

        public void AddFileSystem(Url rootUrl, IFileSystem fileSystem)
        {
            _fileSystems.Add(rootUrl, fileSystem);
            _disposables.Add(fileSystem.FileEvent.On(events =>
            {
                _fileEventEmitter.Emit(events);
            }));
        }

        private IFileSystem GetFileSystemByUrl(Url url)
        {
            foreach (var (rootUrl, fileSystem) in _fileSystems)
            {
                if (url.StartsWith(rootUrl))
                {
                    return fileSystem;
                }
            }

            throw new FileNotFoundException(url);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
