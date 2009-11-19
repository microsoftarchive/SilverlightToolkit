// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// The LogWriter class is used to record information about the basic blocks
    /// being processed during instrumentation for analysis of code coverage
    /// after the data has been gathered.
    /// </summary>
    public sealed partial class LogWriter : IDisposable
    {
        /// <summary>
        /// Writer used to generate the log.
        /// </summary>
        private XmlTextWriter _writer;

        /// <summary>
        /// Initializes a new instance of the Logger class.
        /// </summary>
        /// <param name="path">Path to the log file.</param>
        public LogWriter(string path)
        {
            _writer = new XmlTextWriter(path, Encoding.UTF8);
        }

        /// <summary>
        /// Dispose the writer.
        /// </summary>
        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
        }

        /// <summary>
        /// Start writing the file.
        /// </summary>
        public void Start()
        {
            _writer.WriteStartDocument(true);
            _writer.Formatting = Formatting.Indented;
            _writer.Indentation = 2;
            _writer.IndentChar = ' ';

            _writer.WriteStartElement("coverage");
        }

        /// <summary>
        /// Finish writing the file.
        /// </summary>
        public void Close()
        {
            _writer.WriteEndElement();
            _writer.WriteEndDocument();
        }

        /// <summary>
        /// Start writing an assembly.
        /// </summary>
        /// <param name="assembly">Name of the assembly.</param>
        public void StartAssembly(string assembly)
        {
            _writer.WriteStartElement("assembly");
            _writer.WriteAttributeString("name", assembly);
        }

        /// <summary>
        /// Finish writing an assembly.
        /// </summary>
        public void EndAssembly()
        {
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a comment.
        /// </summary>
        /// <param name="comment">The comment text.</param>
        public void WriteComment(string comment)
        {
            _writer.WriteComment(comment);
        }

        /// <summary>
        /// Start writing a method.
        /// </summary>
        /// <param name="method">Name of the method.</param>
        /// <param name="type">Type of the method.</param>
        /// <param name="file">File containing the method source.</param>
        /// <param name="line">First line of the method source.</param>
        public void StartMethod(string method, string type, string file, uint line)
        {
            _writer.WriteStartElement("method");
            _writer.WriteAttributeString("name", method);
            _writer.WriteAttributeString("class", type);
            WriteSourcePosition(file, line);
        }

        /// <summary>
        /// Finish writing a method.
        /// </summary>
        public void EndMethod()
        {
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Start writing the blocks section.
        /// </summary>
        public void StartBlocks()
        {
            _writer.WriteStartElement("blocks");
        }

        /// <summary>
        /// Finish writing the blocks section.
        /// </summary>
        public void EndBlocks()
        {
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Start writing a block.
        /// </summary>
        /// <param name="id">ID of the block.</param>
        /// <param name="offset">
        /// Starting offset of the block from the beginning of the function.
        /// </param>
        /// <param name="file">File containing the block source.</param>
        /// <param name="startLine">First line of the block source.</param>
        /// <param name="startColumn">First column of the block source.</param>
        /// <param name="lastLine">Last line of the block source.</param>
        /// <param name="lastColumn">Last column of the block source.</param>
        public void StartBlock(uint id, uint offset, string file, uint startLine, uint startColumn, uint lastLine, uint lastColumn)
        {
            _writer.WriteStartElement("block");
            _writer.WriteAttributeString("id", id.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("visited", 0.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("offset", offset.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("startLine", startLine.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("startColumn", startColumn.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("lastLine", lastLine.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("lastColumn", lastColumn.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("file", file);
        }

        /// <summary>
        /// Finish writing a block.
        /// </summary>
        public void EndBlock()
        {
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Start writing a block disassembly.
        /// </summary>
        public void StartBlockDisassembly()
        {
            _writer.WriteStartElement("block-disassembly");
        }

        /// <summary>
        /// Finish writing a block disassembly.
        /// </summary>
        public void EndBlockDisassembly()
        {
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Start writing a method disassembly.
        /// </summary>
        public void StartMethodDisassembly()
        {
            _writer.WriteStartElement("disassembly");
        }

        /// <summary>
        /// Finish writing a method disassembly.
        /// </summary>
        public void EndMethodDisassembly()
        {
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Write the disassembly source for a block.
        /// </summary>
        /// <param name="offset">
        /// Offset of the block from the start of the method.
        /// </param>
        /// <param name="source">Disassembled source for the block.</param>
        /// <param name="block">ID of the basic block.</param>
        public void WriteBlockDisassembly(uint offset, string source, uint block)
        {
            foreach (string sourceLine in source.Split('\n'))
            {
                _writer.WriteStartElement("instruction");
                _writer.WriteAttributeString("offset", offset.ToString(CultureInfo.InvariantCulture));
                _writer.WriteAttributeString("block", block.ToString(CultureInfo.InvariantCulture));
                _writer.WriteString(sourceLine.Trim());
                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write information about the position of the element in the source.
        /// </summary>
        /// <param name="file">File containing the element source.</param>
        /// <param name="line">First line of the element in the source.</param>
        private void WriteSourcePosition(string file, uint line)
        {
            _writer.WriteAttributeString("line", line.ToString(CultureInfo.InvariantCulture));
            _writer.WriteAttributeString("file", file);
        }
    }
}