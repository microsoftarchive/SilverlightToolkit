// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//-----------------------------------------------------------------------------
// Originally from Zip.cs on the http://ironpython.codeplex.com/ site.
//-----------------------------------------------------------------------------

using System.Diagnostics;
using System.Text;

namespace System.IO.Compression
{
    /// <summary>
    /// ZipArchiveFile represents one archive file in the ZipArchive. It is 
    /// analogous to the System.IO.DiffFile object for normal files.
    /// </summary>
    public sealed class ZipArchiveFile
    {
        #region Constants
        /// <summary>
        /// Signature file entry marker.
        /// </summary>
        internal const uint SignatureFileEntry = 0x04034b50;

        /// <summary>
        /// Signature archive directory marker.
        /// </summary>
        internal const uint SignatureArchiveDirectory = 0x02014b50;

        /// <summary>
        /// Signature archive directory ending marker.
        /// </summary>
        internal const uint SignatureArchiveDirectoryEnd = 0x06054b50;

        /// <summary>
        /// Constant for the version needed to extract.
        /// </summary>
        /// <remarks>Original IronPython code also noted this as version 1.0,
        /// along with a to-do statement.</remarks>
        internal const ushort VersionNeededToExtract = 0x0014;

        /// <summary>
        /// The maximum version extractable.
        /// </summary>
        internal const ushort MaximumVersionExtractable = 0x0014;

        /// <summary>
        /// Indicates as MS-DOS file system.
        /// </summary>
        /// <remarks>Original IronPython code also noted a to-do that perhaps
        /// this should be NTFS.</remarks>
        internal const ushort VersionMadeBy = 0;

        /// <summary>
        /// A general purpose bit flag.
        /// </summary>
        /// <remarks>Original IronPython code marks this with to-do.</remarks>
        internal const ushort GeneralPurposeBitFlag = 0;

        /// <summary>
        /// Constant for the extra field length.
        /// </summary>
        internal const ushort ExtraFieldLength = 0;

        /// <summary>
        /// Constant for the file comment length.
        /// </summary>
        internal const ushort FileCommentLength = 0;

        /// <summary>
        /// Constant for the starting disk number.
        /// </summary>
        internal const ushort DiskNumberStart = 0;

        /// <summary>
        /// Indicates as a binary file.
        /// </summary>
        /// <remarks>Original IronPython source indicates a to-do around ASCII
        /// support.</remarks>
        internal const ushort InternalFileAttributes = 0;

        /// <summary>
        /// An external file attributes flag.
        /// </summary>
        /// <remarks>Original IronPython source indicates a to-do as to whether
        /// they wanted to support external attributes.</remarks>
        internal const uint ExternalFileAttributes = 0;

        #endregion

        /// <summary>
        /// The checksum for the file.
        /// </summary>
        private uint? _crc32;
        
        /// <summary>
        /// The archive file name.
        /// </summary>
        private string _name;
        
        /// <summary>
        /// The last write time.
        /// </summary>
        private DateTime _lastWriteTime;
        
        /// <summary>
        /// The length of the file.
        /// </summary>
        private long _length;
        
        /// <summary>
        /// The compressed length.
        /// </summary>
        private int _compressedLength;
        
        /// <summary>
        /// The compresison method.
        /// </summary>
        private CompressionMethod _compressionMethod;
        
        /// <summary>
        /// A reference to the parent zip archive instance.
        /// </summary>
        private ZipArchive _archive;
        
        /// <summary>
        /// The location of the header.
        /// </summary>
        private uint _headerOffset;

        /// <summary>
        /// The uncompressed data.
        /// </summary>
        /// <remarks>
        /// To optimize for the common usage of read-only sequential access of
        /// the ZipArchive files we don't uncompress the data immediately. We
        /// instead just remember where in the stream the compressed data lives
        /// (positionOfCompressedDataInArhive). If the archive is read-write,
        /// we can't do this (because we are changing the archive), so in that 
        /// case we copy the data to 'compressedData'. If the archiveFile
        /// actually gets writen too, however we throw that away and store the
        /// data in uncompressed form.
        /// /// </remarks>
        private MemoryStream _uncompressedData;
        
        /// <summary>
        /// The compressed data.
        /// </summary>
        /// <remarks>
        /// If uncompressed data is null, look here for compressed data.
        /// </remarks>
        private byte[] _compressedData;

        /// <summary>
        /// Position of the compressed data inside the archive.
        /// </summary>
        /// <remarks>
        /// If uncompressed data and compressed data are both null, the data is
        /// at this offset in the archive stream.
        /// </remarks>
        private long _positionOfCompressedDataInArchive;

        /// <summary>
        /// Gets or sets the last time the archive was updated (Create() was
        /// called). The copy operations transfer the LastWriteTime from the 
        /// source to the target.
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                return _lastWriteTime; 
            }

            set
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("Archive is ReadOnly");
                }

                _lastWriteTime = value;
            }
        }

        /// <summary>
        /// Gets the length of the archive textStream in bytes.
        /// </summary>
        public long Length
        {
            get
            {
                return _uncompressedData != null ? _uncompressedData.Length : _length;
            }
        }

        /// <summary>
        /// Gets or sets the file name in the archive. 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { MoveTo(value); }
        }

        /// <summary>
        /// Gets the CRC32 checksum associated with the data. Useful for quickly
        /// determining if the data has changed.  
        /// </summary>
        [CLSCompliant(false)]
        public uint CheckSum
        {
            get
            {
                if (_crc32 == null)
                {
                    Debug.Assert(_uncompressedData != null, "Uncompressed data cannot be null.");
                    _crc32 = Crc32.Calculate(0, _uncompressedData.GetBuffer(), 0, (int)_uncompressedData.Length);
                }

                return _crc32.Value;
            }
        }

        /// <summary>
        /// Gets the archive assoated with the ZipArchiveFile.
        /// </summary>
        internal ZipArchive Archive { get { return _archive; } }

        /// <summary>
        /// Gets a value indicating whether the textStream can be  written (the
        /// archive is read-only).
        /// </summary>
        public bool IsReadOnly { get { return _archive.IsReadOnly; } }

#if DEBUG
        /// <summary>
        /// Gets the data as text.
        /// </summary>
        /// <remarks>Helpful for debugging.</remarks>
        public string DataAsText { get { return ReadAllText(); } }
#endif

        #region Primitive Operations

        /// <summary>
        /// Truncates the archiveFile represented by the ZipArchiveFile to be empty and returns a Stream that can be used
        /// to write (binary) data into it.
        /// </summary>
        /// <returns>A Stream that can be written on. </returns>
        public Stream Create()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("Archive is ReadOnly");
            }
            if (_uncompressedData != null && (_uncompressedData.CanWrite || _uncompressedData.CanRead))
            {
                throw new InvalidOperationException("ZipArchiveFile already open.");
            }

            // abandon any old data
            _compressedData = null;
            _positionOfCompressedDataInArchive = 0;
            _compressedLength = 0;

            // We allocate some buffer so that GetBuffer() does not return null. 
            _uncompressedData = new RepairedMemoryStream(256);
            return _uncompressedData;
        }

        /// <summary>
        /// Opens the archiveFile represented by the ZipArchiveFile and returns a stream that can use to read (binary) data.
        /// </summary>
        /// <returns>A Stream that can be read from.</returns>
        public Stream OpenRead()
        {
            if (_uncompressedData == null)
            {
                if (_compressedData == null)
                {
                    // TODO if we had a rangeStream, we could avoid this copy. 
                    _compressedData = new byte[_compressedLength];
                    _archive.FromStream.Seek(_positionOfCompressedDataInArchive, SeekOrigin.Begin);
                    _archive.FromStream.Read(_compressedData, 0, _compressedLength);
                }
                MemoryStream compressedReader = new MemoryStream(_compressedData);
                if (_compressionMethod == CompressionMethod.None)
                {
                    return compressedReader;
                }
                else
                {
                    return new DeflateStream(compressedReader, CompressionMode.Decompress);
                }
            }
            else
            {
                if (_uncompressedData.CanWrite)
                {
                    throw new InvalidOperationException("ZipArchiveFile still open for writing.");
                }
                return new MemoryStream(_uncompressedData.GetBuffer(), 0, (int)_uncompressedData.Length, false);
            }
        }

        /// <summary>
        /// Truncates the archiveFile represented by the ZipArchiveFile to be empty and returns a TextWriter that text
        /// can be written to (using the default encoding). 
        /// </summary>
        /// <param name="newArchivePath">The new archive path.</param>
        public void MoveTo(string newArchivePath)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("Archive is ReadOnly");
            }

            _archive.Entries.Remove(_name);
            _name = newArchivePath;
            _archive.Entries[newArchivePath] = this;
        }

        /// <summary>
        /// Delete the archiveFile represented by the ZipArchiveFile.   The textStream can be in use without conflict.
        /// Deleting a textStream simply means it will not be persisted when ZipArchive.Close() is called.  
        /// </summary>
        public void Delete()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("Archive is ReadOnly");
            }

            _archive.Entries.Remove(_name);
            _name = null;
            _archive = null;
            _uncompressedData = null;
            _compressedData = null;
        }

        #endregion

        /// <summary>
        ///  A text summary of the archive textStream (name and length).
        /// </summary>
        /// <returns>Returns a text summary of the file.</returns>
        public override string ToString()
        {
            return "ZipArchiveEntry " + Name + " length " + Length;
        }

        // These are convinence methods (could be implemented outside the class)

        /// <summary>
        /// Truncate the archive textStream and return a StreamWrite sutable for
        /// writing text to the textStream. 
        /// </summary>
        /// <returns>Returns a stream writer for the file.</returns>
        public StreamWriter CreateText()
        {
            return new StreamWriter(Create());
        }

        /// <summary>
        /// Opens the archiveFile represented by the ZipArchiveFile and returns 
        /// a stream that can use to read text.
        /// </summary>
        /// <returns>A TextReader text can be read from.</returns>
        public StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        /// <summary>
        /// Read all the text from the archiveFile represented by the 
        /// ZipArchiveFile and return it as a string.
        /// </summary>
        /// <returns>The string contained in the archiveFile.</returns>
        public string ReadAllText()
        {
            TextReader reader = OpenText();
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        /// <summary>
        /// Replaces the data in the archiveFile represented by the 
        /// ZipArchiveFile with the text in 'data'.
        /// </summary>
        /// <param name="data">The data to replace the archiveFile data with.</param>
        public void WriteAllText(string data)
        {
            TextWriter writer = CreateText();
            writer.Write(data);
            writer.Close();
        }

        /// <summary>
        /// Copy the data in from the 'this' ZipArchiveFile to the archive 
        /// textStream named 'outputFilePath' in to the file system at 
        /// 'outputFilePath'.
        /// </summary>
        /// <param name="outputFilePath">The output file path.</param>
        public void CopyToFile(string outputFilePath)
        {
            string outputDirectory = Path.GetDirectoryName(outputFilePath);
            if (outputDirectory.Length > 0)
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (Stream outFile = new FileStream(outputFilePath, FileMode.Create))
            {
                using (Stream inFile = OpenRead())
                {
                    CopyStream(inFile, outFile);
                }
            }

            File.SetLastWriteTime(outputFilePath, LastWriteTime);
        }

        /// <summary>
        /// Copy the data in archive textStream named 'inputFilePath' into the
        /// 'this' archive textStream, discarding what was there before.
        /// </summary>
        /// <param name="outputArchivePath">The output archive path.</param>
        public void CopyTo(string outputArchivePath)
        {
            using (Stream outFile = _archive.Create(outputArchivePath))
            using (Stream inFile = OpenRead())
                CopyStream(inFile, outFile);
            _archive[outputArchivePath].LastWriteTime = LastWriteTime;
        }

        #region PrivateImplementation

        /// <summary>
        /// Compression method enum.
        /// </summary>
        internal enum CompressionMethod : ushort
        {
            /// <summary>
            /// No compression method.
            /// </summary>
            None = 0,

            /// <summary>
            /// The deflate compression method.
            /// </summary>
            Deflate = 8,
        }

        /// <summary>
        /// Set of invalid path characters.
        /// </summary>
        private static char[] invalidPathChars = Path.GetInvalidPathChars();

        /// <summary>
        /// Copies from one stream to another.
        /// </summary>
        /// <param name="fromStream">The input stream.</param>
        /// <param name="toStream">The output stream.</param>
        /// <returns>Returns the number of total bytes.</returns>
        static internal int CopyStream(Stream fromStream, Stream toStream)
        {
            byte[] buffer = new byte[8192];
            int totalBytes = 0;
            for (;;)
            {
                int count = fromStream.Read(buffer, 0, buffer.Length);
                if (count == 0)
                {
                    break;
                }
                toStream.Write(buffer, 0, count);
                totalBytes += count;
            }
            return totalBytes;
        }

        /// <summary>
        /// Convert a classic DOS time to date time.
        /// </summary>
        /// <param name="dateTime">A 32-bit number containing the date time.</param>
        /// <returns>Returns a DateTime instance.</returns>
        static private DateTime DosTimeToDateTime(uint dateTime)
        {
            int dateTimeSigned = (int)dateTime;
            int year = 1980 + (dateTimeSigned >> 25);
            int month = (dateTimeSigned >> 21) & 0xF;
            int day = (dateTimeSigned >> 16) & 0x1F;
            int hour = (dateTimeSigned >> 11) & 0x1F;
            int minute = (dateTimeSigned >> 5) & 0x3F;
            int second = (dateTimeSigned & 0x001F) * 2;       // only 5 bits for second, so we only have a granularity of 2 sec. 
            if (second >= 60)
            {
                second = 0;
            }

            DateTime ret = new DateTime();
            try
            {
                ret = new System.DateTime(year, month, day, hour, minute, second, 0);
            }
            catch
            {
            }

            return ret;
        }

        /// <summary>
        /// Encodes a DateTime instance as a 32-bit number to save space. The
        /// format is used in DOS.
        /// </summary>
        /// <param name="dateTime">The DateTime instance.</param>
        /// <returns>The encoded unsigned integer with the date and time.</returns>
        static private uint DateTimeToDosTime(DateTime dateTime)
        {
            int ret = ((dateTime.Year - 1980) & 0x7F);
            ret = (ret << 4) + dateTime.Month;
            ret = (ret << 5) + dateTime.Day;
            ret = (ret << 5) + dateTime.Hour;
            ret = (ret << 6) + dateTime.Minute;
            ret = (ret << 5) + (dateTime.Second / 2);   // only 5 bits for second, so we only have a granularity of 2 sec.
            return (uint)ret;
        }

        // These routines are only to be used by ZipArchive.

        /// <summary>
        /// Used by ZipArchive to write the entry to the archive.
        /// </summary>
        /// <param name="writer">The stream representing the archive to write 
        /// the entry to.</param>
        internal void WriteToStream(Stream writer)
        {
            Debug.Assert(!IsReadOnly, "Cannot be read only.");
            Debug.Assert(_positionOfCompressedDataInArchive == 0, "Position must be 0.");   // we don't use this on read-write archives. 

            if (_uncompressedData != null)
            {
                if (_uncompressedData.CanWrite)
                {
                    throw new InvalidOperationException("Unclosed writable handle to " + Name + " still exists at Save time");
                }

                // Original to-do statements from the IronPython source:
                // Consider using seeks instead of copying to the compressed data stream.  
                // Support not running Deflate but simply skipping

                // Compress the data
                MemoryStream compressedDataStream = new RepairedMemoryStream((int)(_uncompressedData.Length * 5 / 8));
                Stream compressor = new DeflateStream(compressedDataStream, CompressionMode.Compress);
                compressor.Write(_uncompressedData.GetBuffer(), 0, (int)_uncompressedData.Length);
                compressor.Close();

                _compressionMethod = CompressionMethod.Deflate;
                _compressedLength = (int)compressedDataStream.Length;
                _compressedData = compressedDataStream.GetBuffer();
            }

            Debug.Assert(_compressedData != null, "Must be compressed data.");
            WriteZipFileHeader(writer);                             // Write the header.
            writer.Write(_compressedData, 0, _compressedLength);      // Write the data. 
        }

        /// <summary>
        /// Writes the Zip file header to the stream.
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        private void WriteZipFileHeader(Stream writer)
        {
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(_name.Replace(Path.DirectorySeparatorChar, '/'));
            if ((uint)_length != _length)
            {
                throw new InvalidOperationException("File length too long.");
            }

            // Local file header:
            //
            // local file header signature     4 bytes  (0x04034b50)
            // version needed to extract       2 bytes
            // general purpose bit flag        2 bytes
            // compression method              2 bytes
            // last mod file time              2 bytes
            // last mod file date              2 bytes
            // crc-32                          4 bytes
            // compressed size                 4 bytes
            // uncompressed size               4 bytes
            // file name length                2 bytes
            // extra field length              2 bytes
            //
            // file name (variable size)
            // extra field (variable size)

            // Save the start of the header file for later use in the dir entry
            _headerOffset = (uint)writer.Position;

            ByteBuffer header = new ByteBuffer(30);
            header.WriteUInt32(SignatureFileEntry);
            header.WriteUInt16(VersionNeededToExtract);
            header.WriteUInt16(GeneralPurposeBitFlag);
            header.WriteUInt16((ushort)_compressionMethod);
            header.WriteUInt32(DateTimeToDosTime(_lastWriteTime));
            header.WriteUInt32(CheckSum);
            header.WriteUInt32((uint)_compressedLength);
            header.WriteUInt32((uint)Length);
            header.WriteUInt16((ushort)fileNameBytes.Length);
            header.WriteUInt16(ExtraFieldLength);                                 // extra field length (unused)

            header.WriteContentsTo(writer);

            writer.Write(fileNameBytes, 0, fileNameBytes.Length);                // write the archiveFile name
        }

        /// <summary>
        /// Writes an archive directory entry to stream.
        /// </summary>
        /// <param name="writer">The writer stream.</param>
        internal void WriteArchiveDirectoryEntryToStream(Stream writer)
        {
            // File header (in central directory):
            //
            // central file header signature   4 bytes  (0x02014b50)
            // version made by                 2 bytes
            // version needed to extract       2 bytes
            // general purpose bit flag        2 bytes
            // compression method              2 bytes
            // last mod file time              2 bytes
            // last mod file date              2 bytes
            // crc-32                          4 bytes
            // compressed size                 4 bytes
            // uncompressed size               4 bytes
            // file name length                2 bytes
            // extra field length              2 bytes
            // file comment length             2 bytes
            // disk number start               2 bytes
            // internal file attributes        2 bytes
            // external file attributes        4 bytes
            // relative offset of local header 4 bytes
            //
            // file name (variable size)
            // extra field (variable size)
            // file comment (variable size)

            byte[] fileNameBytes = Encoding.UTF8.GetBytes(_name);

            ByteBuffer header = new ByteBuffer(46);
            header.WriteUInt32(SignatureArchiveDirectory);
            header.WriteUInt16(VersionMadeBy);
            header.WriteUInt16(VersionNeededToExtract);
            header.WriteUInt16(GeneralPurposeBitFlag);
            header.WriteUInt16((ushort)_compressionMethod);
            header.WriteUInt32(DateTimeToDosTime(_lastWriteTime));
            header.WriteUInt32(CheckSum);
            header.WriteUInt32((uint)_compressedLength);
            header.WriteUInt32((uint)Length);
            header.WriteUInt16((ushort)fileNameBytes.Length);
            header.WriteUInt16(ExtraFieldLength);
            header.WriteUInt16(FileCommentLength);
            header.WriteUInt16(DiskNumberStart);
            header.WriteUInt16(InternalFileAttributes);
            header.WriteUInt32(ExternalFileAttributes);
            header.WriteUInt32(_headerOffset);

            header.WriteContentsTo(writer);

            writer.Write(fileNameBytes, 0, fileNameBytes.Length);
        }

        /// <summary>
        /// Create a new archive archiveFile with no data (empty). It is
        /// expected that only ZipArchive methods will use this routine.
        /// </summary>
        /// <param name="archive">The archive object.</param>
        /// <param name="archiveName">The archive name.</param>
        internal ZipArchiveFile(ZipArchive archive, string archiveName)
        {
            _archive = archive;
            _name = archiveName;
            if (_name != null)
            {
                archive.Entries[_name] = this;
            }
            _lastWriteTime = DateTime.Now;
        }

        /// <summary>
        /// Reads a single archiveFile from a Zip Archive. Should only be used
        /// by ZipArchive.
        /// </summary>
        /// <param name="archive">The Zip archive object.</param>
        /// <returns>A ZipArchiveFile representing the archiveFile read from the
        /// archive.</returns>
        internal static ZipArchiveFile Read(ZipArchive archive)
        {
            Stream reader = archive.FromStream;
            ByteBuffer header = new ByteBuffer(30);
            int count = header.ReadContentsFrom(reader);
            if (count == 0)
            {
                return null;
            }

            // Local file header:
            //
            // local file header signature     4 bytes  (0x04034b50)
            // version needed to extract       2 bytes
            // general purpose bit flag        2 bytes
            // compression method              2 bytes
            // last mod file time              2 bytes
            // last mod file date              2 bytes
            // crc-32                          4 bytes
            // compressed size                 4 bytes
            // uncompressed size               4 bytes
            // file name length                2 bytes
            // extra field length              2 bytes
            //
            // file name (variable size)
            // extra field (variable size)

            uint fileSignature = header.ReadUInt32();
            if (fileSignature != SignatureFileEntry)
            {
                if (fileSignature != SignatureArchiveDirectory)
                {
                    throw new InvalidOperationException("Bad ZipFile Header");
                }
                return null;
            }

            ushort versionNeededToExtract = header.ReadUInt16();
            if (versionNeededToExtract > MaximumVersionExtractable)
            {
                throw new NotSupportedException("Zip file requires unsupported features");
            }

            header.SkipBytes(2); // general purpose bit flag

            ZipArchiveFile newEntry = new ZipArchiveFile(archive, null);

            newEntry._compressionMethod = (CompressionMethod)header.ReadUInt16();
            newEntry._lastWriteTime = DosTimeToDateTime(header.ReadUInt32());
            newEntry._crc32 = header.ReadUInt32();
            newEntry._compressedLength = checked((int)header.ReadUInt32());
            newEntry._length = header.ReadUInt32();
            int fileNameLength = checked((int)header.ReadUInt16());

            byte[] fileNameBuffer = new byte[fileNameLength];
            int fileNameCount = reader.Read(fileNameBuffer, 0, fileNameLength);
            newEntry._name = Encoding.UTF8.GetString(fileNameBuffer).Replace('/', Path.DirectorySeparatorChar);
            archive.Entries[newEntry._name] = newEntry;

            if (count != header.Length || fileNameCount != fileNameLength || fileNameLength == 0 || newEntry.LastWriteTime.Ticks == 0)
            {
                throw new InvalidOperationException("Bad Zip File Header");
            }

            if (newEntry.Name.IndexOfAny(invalidPathChars) >= 0)
            {
                throw new InvalidOperationException("Invalid File Name");
            }

            if (newEntry._compressionMethod != CompressionMethod.None && newEntry._compressionMethod != CompressionMethod.Deflate)
            {
                throw new NotSupportedException("Unsupported compression mode " + newEntry._compressionMethod);
            }

            if (archive.IsReadOnly && reader.CanSeek)
            {
                // Optimization: we can defer reading in the data in the common
                // case of a read-only archive by simply remembering where the
                // data is and fetching it on demand.  This is nice because we
                // only hold on to memory of data we are actually operating on,
                // instead of the whole archive.
                // 
                // Because uncompresseData is null, we know that we fetch it
                // from the archive 'fromStream'.
                newEntry._positionOfCompressedDataInArchive = archive.FromStream.Position;
                reader.Seek(newEntry._compressedLength, SeekOrigin.Current);
            }
            else
            {
                // We may be updating the archive in place, so we need to copy
                // the data out.
                newEntry._compressedData = new byte[newEntry._compressedLength];
                reader.Read(newEntry._compressedData, 0, (int)newEntry._compressedLength);
            }

#if DEBUG
            newEntry.Validate();
#endif

            return newEntry;
        }

        /// <summary>
        /// Validate the data.
        /// </summary>
        internal void Validate()
        {
            Stream readStream = OpenRead();
            uint crc = 0;
            byte[] buffer = new byte[655536];
            for (;;)
            {
                int count = readStream.Read(buffer, 0, buffer.Length);
                if (count == 0)
                {
                    break;
                }
                crc = Crc32.Calculate(crc, buffer, 0, count);
            }

            readStream.Close();

            if (crc != CheckSum)
            {
                throw new InvalidOperationException("Error: data checksum failed for " + Name);
            }
        }

        #endregion
    }
}