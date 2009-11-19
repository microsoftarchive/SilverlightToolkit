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
    /// MemoryStream does not let you look at the length after it has been
    /// closed. Overridden here, storing the size when it is closed.
    /// </summary>
    internal class RepairedMemoryStream : MemoryStream
    {
        /// <summary>
        /// The stream length.
        /// </summary>
        private long _length;
        
        /// <summary>
        /// A value indicating whether the stream is closed.
        /// </summary>
        private bool _isClosed;

        /// <summary>
        /// Initializes a new instance of the RepairedMemoryStream class.
        /// </summary>
        /// <param name="size">The initial size.</param>
        public RepairedMemoryStream(int size) : base(size)
        {
        }

        /// <summary>
        /// Closes the stream, storing the length for later use.
        /// </summary>
        public override void Close()
        {
            _length = Length;
            _isClosed = true;

            base.Close();
        }

        /// <summary>
        /// Gets the Length of the stream. Can be called after the stream is
        /// closed.
        /// </summary>
        public override long Length
        {
            get
            {
                return _isClosed ? _length : base.Length;
            }
        }
    }
}