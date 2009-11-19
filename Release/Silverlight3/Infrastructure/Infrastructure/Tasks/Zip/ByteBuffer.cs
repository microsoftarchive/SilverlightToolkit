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

namespace System.IO.Compression
{
    /// <summary>
    /// A byte[] array that also stores a current offset. Useful for 
    /// reading/writing headers, ensuring the offset is updated correctly.
    /// </summary>
    internal struct ByteBuffer
    {
        /// <summary>
        /// The byte buffer.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// The current offset.
        /// </summary>
        private int _offset;

        /// <summary>
        /// Gets the length of the buffer.
        /// </summary>
        public int Length
        {
            get { return _buffer.Length; }
        }

        /// <summary>
        /// Initializes a new instance of the ByteBuffer type.
        /// </summary>
        /// <param name="size">The buffer size.</param>
        public ByteBuffer(int size)
        {
            _buffer = new byte[size];
            _offset = 0;
        }

        /// <summary>
        /// Skips ahead a count of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        public void SkipBytes(int count)
        {
            _offset += count;
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the buffer.
        /// </summary>
        /// <returns>Returns an unsigned integer from the buffer.</returns>
        public uint ReadUInt32()
        {
            return (uint)(_buffer[_offset++] | ((_buffer[_offset++] | ((_buffer[_offset++] | (_buffer[_offset++] << 8)) << 8)) << 8));
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from the buffer.
        /// </summary>
        /// <returns>Returns an unsigned short from the buffer.</returns>
        public ushort ReadUInt16()
        {
            return (ushort)(_buffer[_offset++] | ((_buffer[_offset++]) << 8));
        }

        /// <summary>
        /// Write an unsigned integer to the buffer.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt32(uint value)
        {
            _buffer[_offset++] = (byte)value;
            _buffer[_offset++] = (byte)(value >> 8);
            _buffer[_offset++] = (byte)(value >> 16);
            _buffer[_offset++] = (byte)(value >> 24);
        }

        /// <summary>
        /// Writes an unsigned short value to the buffer.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt16(ushort value)
        {
            _buffer[_offset++] = (byte)value;
            _buffer[_offset++] = (byte)(value >> 8);
        }

        /// <summary>
        /// Writes the contents of the buffer to a stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        public void WriteContentsTo(Stream writer)
        {
            if (_offset != _buffer.Length)
            {
                // Was: Debug.Assert(_offset == _buffer.Length);
                throw new InvalidOperationException("The byte buffer is in an invalid state for this operation.");
            }

            writer.Write(_buffer, 0, _buffer.Length);
        }

        /// <summary>
        /// Fills the buffer from the stream.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        /// <returns>Returns the number of bits read.</returns>
        public int ReadContentsFrom(Stream reader)
        {
            if (_offset != 0)
            {
                throw new InvalidOperationException("The byte buffer is in an invalid state for this operation.");
                // Was: Debug.Assert(_offset == 0);
            }

            return reader.Read(_buffer, 0, _buffer.Length);
        }
    }
}