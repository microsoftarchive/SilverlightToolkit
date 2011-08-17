// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace PhoneToolkitSample.Data
{
    /// <summary>
    /// A class to return a list of web page titles and addresses from lorem ipsum data.
    /// </summary>
    public class LoremIpsumWebsites : IEnumerable<Tuple<string, string>>
    {
        /// <summary>
        /// Enumerates the Words property.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Tuple<string, string>> GetEnumerator()
        {
            foreach (string s in LoremIpsum.Words)
            {
                string t = string.Empty;
                if (s.Length > 0)
                {
                    t = Char.ToUpper(s[0]) + s.Substring(1);
                }
                yield return new Tuple<string, string> { Item1 = t, Item2 = "http://www." + s + "+net.com/" };
            }
        }

        /// <summary>
        /// Enumerates the Words property.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
