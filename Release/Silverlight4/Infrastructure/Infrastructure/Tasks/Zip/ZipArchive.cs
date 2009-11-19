// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//-----------------------------------------------------------------------------
// Originally from Zip.cs on the http://ironpython.codeplex.com/ site.
//-----------------------------------------------------------------------------

/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System.Collections.Generic;
using System.Diagnostics;

namespace System.IO.Compression
{
    /// <summary>
    /// ZipArchive represents a Zip Archive.  It uses the System.IO.File 
    /// structure as its guide.
    /// 
    /// The largest structual difference between a ZipArchive and the textStream
    /// system is that the archive has no independent notion of a 'directory'. 
    /// Instead files know their complete path name. For the most part this
    /// difference is hard to notice, but does have some ramifications. For
    /// example, there is no concept of the modification time for a directory.
    /// </summary>
    internal sealed class ZipArchive
    {
        /// <summary>
        /// Set of file entries in the archive.
        /// </summary>
        private SortedDictionary<string, ZipArchiveFile> _entries;

        /// <summary>
        /// The input stream.
        /// </summary>
        private Stream _fromStream;

        /// <summary>
        /// The file access set for the archive.
        /// </summary>
        private FileAccess _access;

        /// <summary>
        /// The Zip archive path.
        /// </summary>
        private string _archivePath;

        /// <summary>
        /// A value indicating whether close has been called yet.
        /// </summary>
        private bool _closeCalled;

        /// <summary>
        /// Openes an existing ZIP archive 'archivePath' for reading.
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        public ZipArchive(string archivePath) : this(archivePath, FileAccess.Read)
        {
        }

        /// <summary>
        /// Opens a ZIP archive, 'archivePath'. If 'access' is ReadWrite or
        /// Write then the target does not need to exist, but will be created
        /// with the ZipArchive is closed.
        /// 
        /// If 'access' is ReadWrite the target can exist, and that data is used
        /// to initially populate the archive. Any modifications that were made
        /// will be updated when the Close() method is called (and not before).
        /// 
        /// If 'access' is Write then the target is either created or truncated
        /// to 0 before the archive is written (thus the original data in the 
        /// archiveFile is ignored).
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        /// <param name="access">The file access mode requested.</param>
        public ZipArchive(string archivePath, FileAccess access)
        {
            _entries = new SortedDictionary<string, ZipArchiveFile>(StringComparer.OrdinalIgnoreCase);
            _archivePath = archivePath;
            _access = access;
            if (access == FileAccess.Read)
            {
                _fromStream = new FileStream(archivePath, FileMode.Open, access);
            }
            else if (access == FileAccess.ReadWrite)
            {
                _fromStream = new FileStream(archivePath, FileMode.OpenOrCreate, access);
            }

            // For the write case, we are lazy so as not to empty files on failure.
            if (_fromStream != null)
            {
                Read(_fromStream);
            }
        }

        /// <summary>
        /// Read an archive from an exiting stream or write a new archive into a
        /// stream.
        /// </summary>
        /// <param name="fromStream">The input stream.</param>
        /// <param name="desiredAccess">The desired file access mode.</param>
        public ZipArchive(Stream fromStream, FileAccess desiredAccess)
        {
            _entries = new SortedDictionary<string, ZipArchiveFile>(StringComparer.OrdinalIgnoreCase);

            _access = desiredAccess;
            _fromStream = fromStream;

            if ((desiredAccess & FileAccess.Read) != 0)
            {
                if (!fromStream.CanRead)
                {
                    throw new InvalidOperationException("Error: Can't read from stream.");
                }
                Read(fromStream);
            }
            else if ((desiredAccess & FileAccess.Write) != 0)
            {
                if (!fromStream.CanWrite)
                {
                    throw new InvalidOperationException("Error: Can't write to stream.");
                }
            }
        }

        /// <summary>
        /// Gets an enumerable for the files in the archive (directories don't
        /// have an independent existance).
        /// </summary>
        public IEnumerable<ZipArchiveFile> Files
        {
            get
            {
                return _entries.Values;
            }
        }

        /// <summary>
        /// Gets the entries dictionary internally.
        /// </summary>
        internal SortedDictionary<string, ZipArchiveFile> Entries { get { return _entries; } }

        /// <summary>
        /// Gets the from stream internally.
        /// </summary>
        internal Stream FromStream { get { return _fromStream; } }

        /// <summary>
        /// Returns a subset of the files in the archive that are in the 
        /// directory 'archivePath'. If searchOptions is TopDirectoryOnly only
        /// files in the directory 'archivePath' are returns. If searchOptions
        /// is AllDirectories then all files that are in subdiretories are also
        /// returned. 
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        /// <param name="searchOptions">The search options.</param>
        /// <returns>Returns an enumerable of zip archive files.</returns>
        public IEnumerable<ZipArchiveFile> GetFilesInDirectory(string archivePath, SearchOption searchOptions)
        {
            foreach (ZipArchiveFile entry in _entries.Values)
            {
                string name = entry.Name;
                if (name.StartsWith(archivePath, StringComparison.OrdinalIgnoreCase) && name.Length > archivePath.Length)
                {
                    if (searchOptions == SearchOption.TopDirectoryOnly)
                    {
                        if (name.IndexOf(Path.DirectorySeparatorChar, archivePath.Length + 1) >= 0)
                        {
                            continue;
                        }
                    }
                    yield return entry;
                }
            }
        }

        /// <summary>
        /// Gets an archiveFile by name. 'archivePath' is the full path name of
        /// the archiveFile in the archive. It returns null if the name does not
        /// exist.
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        public ZipArchiveFile this[string archivePath]
        {
            get
            {
                ZipArchiveFile ret = null;
                _entries.TryGetValue(archivePath, out ret);
                return ret;
            }
        }

        /// <summary>
        /// Open the archive textStream 'archivePath' for reading and returns
        /// the resulting Stream. KeyNotFoundException is thrown if
        /// 'archivePath' does not exist.
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        /// <returns>Returns the archive textStream.</returns>
        public Stream OpenRead(string archivePath)
        {
            return _entries[archivePath].OpenRead();
        }

        /// <summary>
        /// Opens the archive textStream 'archivePath' for writing and returns
        /// the resulting Stream. If the textStream already exists, it is
        /// truncated to be an empty textStream.
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        /// <returns>Returns the archive text stream.</returns>
        public Stream Create(string archivePath)
        {
            ZipArchiveFile newEntry;
            if (!_entries.TryGetValue(archivePath, out newEntry))
            {
                newEntry = new ZipArchiveFile(this, archivePath);
            }
            return newEntry.Create();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the archive cannot be
        /// written to (it was opened with FileAccess.Read). 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _access == FileAccess.Read;
            }

            set
            {
                if (_fromStream != null)
                {
                    if (value == true)
                    {
                        if (!_fromStream.CanRead)
                        {
                            throw new InvalidOperationException("Can't read from stream");
                        }
                        _access = FileAccess.Read;
                    }
                    else
                    {
                        if (!_fromStream.CanWrite)
                        {
                            throw new InvalidOperationException("Can't reset IsReadOnly on a ZipArchive whose stream is ReadOnly.");
                        }
                        _access = _fromStream.CanRead ? FileAccess.ReadWrite : FileAccess.Write;
                    }
                }
                else
                {
                    _access = value ? FileAccess.Read : FileAccess.ReadWrite;
                }
            }
        }

        /// <summary>
        /// Closes the archive. Until this call is made any pending
        /// modifications to the archive are NOT made (the archive remains
        /// unchanged).
        /// </summary>
        public void Close()
        {
            _closeCalled = true;
            if (!IsReadOnly)
            {
                if (_fromStream == null)
                {
                    Debug.Assert(_archivePath != null, "Archive path cannot be null.");
                    Debug.Assert(_access == FileAccess.Write, "The file access value must be Write.");
                    _fromStream = new FileStream(_archivePath, FileMode.Create);
                }

                _fromStream.Position = 0;
                _fromStream.SetLength(0);      // delete the data in the stream.

                foreach (ZipArchiveFile entry in _entries.Values)
                {
                    entry.WriteToStream(_fromStream);
                }

                WriteArchiveDirectoryToStream(_fromStream);
            }

            _fromStream.Close();
        }

        /// <summary>
        /// Remove all files from the archive.
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
        }

        /// <summary>
        /// Gets the total number of files (does not include directories) in the
        /// archive. 
        /// </summary>
        public int Count { get { return _entries.Count; } }

        #region Convienence Methods
        // These are convinence methods (could be implemented outside this class)

        /// <summary>
        /// Returns true if 'archivePath' exists in the archive.  
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        /// <returns>Returns a value indicating whether the archive contains the
        /// file.</returns>
        public bool Exists(string archivePath)
        {
            return _entries.ContainsKey(archivePath);
        }

        /// <summary>
        ///  Renames sourceArchivePath to destinationArchivePath. If
        ///  destinationArchivePath exists it is discarded.
        /// </summary>
        /// <param name="sourceArchivePath">The source filename.</param>
        /// <param name="destinationArchivePath">The destination filename.</param>
        public void Move(string sourceArchivePath, string destinationArchivePath)
        {
            _entries[sourceArchivePath].MoveTo(destinationArchivePath);
        }

        /// <summary>
        /// Delete 'archivePath'.
        /// </summary>
        /// <remarks>
        /// If archivePath does not exist, it simply returns false (no exception is thrown).  The delete succeeds even if streams on the
        /// data exists (they continue to exist, but will not be persisted on Close()
        /// </remarks>
        /// <param name="archivePath">The archive path.</param>
        /// <returns>Returns a value indicating whether the deletion occurs.</returns>
        public bool Delete(string archivePath)
        {
            ZipArchiveFile entry;
            if (!_entries.TryGetValue(archivePath, out entry))
            {
                return false;
            }
            entry.Delete();
            return true;
        }

        /// <summary>
        /// Copies the archive textStream 'sourceArchivePath' to the textStream
        /// system textStream 'targetFilePath'. Will overwrite existing files,
        /// however a locked targetFilePath will cause an exception.
        /// </summary>
        /// <param name="sourceArchivePath">The source filename.</param>
        /// <param name="targetFilePath">The target file path.</param>
        public void CopyToFile(string sourceArchivePath, string targetFilePath)
        {
            _entries[sourceArchivePath].CopyToFile(targetFilePath);
        }

        /// <summary>
        /// Copies 'sourceFilePath from the textStream system to the archive as
        /// 'targetArchivePath'. Will overwrite any existing textStream.
        /// </summary>
        /// <param name="sourceFilePath">The source filename.</param>
        /// <param name="targetArchivePath">The target path.</param>
        public void CopyFromFile(string sourceFilePath, string targetArchivePath)
        {
            using (Stream inFile = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
            {
                using (Stream outFile = Create(targetArchivePath))
                {
                    ZipArchiveFile.CopyStream(inFile, outFile);
                }
            }

            this[targetArchivePath].LastWriteTime = File.GetLastWriteTime(sourceFilePath);
        }

        /// <summary>
        /// Deletes all files in the directory (and subdirectories) of
        /// 'archivePath'.
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        /// <returns>Returns the number of deleted entries.</returns>
        public int DeleteDirectory(string archivePath)
        {
            int ret = 0;
            List<ZipArchiveFile> entriesToDelete = new List<ZipArchiveFile>(GetFilesInDirectory(archivePath, SearchOption.AllDirectories));
            foreach (ZipArchiveFile entry in entriesToDelete)
            {
                entry.Delete();
                ret++;
            }
            return ret;
        }

        /// <summary>
        /// Copies recursively the files in archive directory to a textStream
        /// system directory.
        /// </summary>
        /// <param name="sourceArchiveDirectory">
        /// The name of the source directory in the archive.
        /// </param>
        /// <param name="targetDirectory">
        /// The target directory in the textStream system to copy to. If it is
        /// empty it represents all files in the archive.
        /// </param>
        public void CopyToDirectory(string sourceArchiveDirectory, string targetDirectory)
        {
            foreach (ZipArchiveFile entry in GetFilesInDirectory(sourceArchiveDirectory, SearchOption.AllDirectories))
            {
                string relativePath = GetRelativePath(entry.Name, sourceArchiveDirectory);
                entry.CopyToFile(Path.Combine(targetDirectory, relativePath));
            }
        }

        /// <summary>
        /// Copies a directory recursively from the textStream system to the
        /// archive.
        /// </summary>
        /// <param name="sourceDirectory">
        /// The direcotry in the textStream system to copy to the archive.
        /// </param>
        /// <param name="targetArchiveDirectory">
        /// The directory in the archive to copy to. An empty string indicates
        /// the top level of the archive.
        /// </param>
        public void CopyFromDirectory(string sourceDirectory, string targetArchiveDirectory)
        {
            foreach (string path in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string relativePath = GetRelativePath(path, sourceDirectory);
                CopyFromFile(path, Path.Combine(targetArchiveDirectory, relativePath));
            }
        }

        /// <summary>
        /// Open an existing textStream in the archive for reading as text and
        /// returns the resulting StreamReader.
        /// </summary>
        /// <param name="archivePath">The archive path.</param>
        /// <returns>Returns the stream reader for an archive file.</returns>
        public StreamReader OpenText(string archivePath)
        {
            return _entries[archivePath].OpenText();
        }

        /// <summary>
        /// Opens a textStream in the archive for writing as a text textStream.
        /// </summary>
        /// <param name="archivePath">The filename.</param>
        /// <returns>Returns the resulting TextWriter.</returns>
        public TextWriter CreateText(string archivePath)
        {
            ZipArchiveFile newEntry;
            if (!_entries.TryGetValue(archivePath, out newEntry))
            {
                newEntry = new ZipArchiveFile(this, archivePath);
            }
            return newEntry.CreateText();
        }

        /// <summary>
        /// Reads all the data in 'archivePath' as a text string and returns it.
        /// </summary>
        /// <param name="archivePath">The filename.</param>
        /// <returns>Returns all data in the filename as a text string.</returns>
        public string ReadAllText(string archivePath)
        {
            return _entries[archivePath].ReadAllText();
        }

        /// <summary>
        /// Overwrites the archive textStream 'archivePath' with the text in
        /// 'data'.
        /// </summary>
        /// <param name="archivePath">The filename.</param>
        /// <param name="data">The string data to write.</param>
        public void WriteAllText(string archivePath, string data)
        {
            ZipArchiveFile newEntry;
            if (!_entries.TryGetValue(archivePath, out newEntry))
            {
                newEntry = new ZipArchiveFile(this, archivePath);
            }
            newEntry.WriteAllText(data);
        }

        #endregion

        /// <summary>
        /// Returns a string reprentation of the archive (its name if known, and
        /// count of files). Most useful in the debugger.
        /// </summary>
        /// <returns>Returns information about the zip archive.</returns>
        public override string ToString()
        {
            string name = _archivePath;
            if (_archivePath == null)
            {
                name = "<fromStream>";
            }

            return "ZipArchive " + name + " FileCount = " + _entries.Count;
        }

        #region PrivateImplementation

        /// <summary>
        /// Destructor for the archive to warn the developer.
        /// </summary>
        ~ZipArchive()
        {
            Debug.Assert(
                _access == FileAccess.Read || _closeCalled || _entries.Count == 0,
                "Did not close a writable archive (use clear to abandon it)");
        }

        // Note: This is really a general purpose routine, but was put here
        // to avoid taking a dependency.

        /// <summary>
        /// Returns a relative path.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>Returns the relative path.</returns>
        internal static string GetRelativePath(string fileName, string directory)
        {
            Debug.Assert(fileName.StartsWith(directory), "directory not a prefix");

            int directoryEnd = directory.Length;
            if (directoryEnd == 0)
            {
                return fileName;
            }

            while (directoryEnd < fileName.Length && fileName[directoryEnd] == Path.DirectorySeparatorChar)
            {
                directoryEnd++;
            }

            string relativePath = fileName.Substring(directoryEnd);
            return relativePath;
        }

        /// <summary>
        /// Read from the archive stream.
        /// </summary>
        /// <param name="archiveStream">The archive stream.</param>
        private void Read(Stream archiveStream)
        {
            // Original IronPython source noted a to-do for seekable streams,
            // to seek to the end of the stream, and use the archive directory
            // there to avoid reading most of the file.
            for (;;)
            {
                ZipArchiveFile entry = ZipArchiveFile.Read(this);
                if (entry == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Implementation of writing an archive directory to the stream.
        /// </summary>
        /// <param name="writer">The writer stream.</param>
        private void WriteArchiveDirectoryToStream(Stream writer)
        {
            // Write the directory entries.
            long startOfDirectory = writer.Position;
            foreach (ZipArchiveFile entry in _entries.Values)
            {
                entry.WriteArchiveDirectoryEntryToStream(writer);
            }
            long endOfDirectory = writer.Position;

            // Write the trailer
            ByteBuffer trailer = new ByteBuffer(22);
            trailer.WriteUInt32(ZipArchiveFile.SignatureArchiveDirectoryEnd);
            trailer.WriteUInt16(ZipArchiveFile.DiskNumberStart);
            trailer.WriteUInt16(ZipArchiveFile.DiskNumberStart);
            trailer.WriteUInt16((ushort)_entries.Count);
            trailer.WriteUInt16((ushort)_entries.Count);
            trailer.WriteUInt32((uint)(endOfDirectory - startOfDirectory));      // directory size
            trailer.WriteUInt32((uint)startOfDirectory);                         // directory start
            trailer.WriteUInt16((ushort)ZipArchiveFile.FileCommentLength);
            trailer.WriteContentsTo(writer);
        }

        #endregion
    }
}