using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Phone.Controls
{
        /// <summary>
        /// The event args for the Link/Unlink events.
        /// </summary>
        public class LinkUnlinkEventArgs : EventArgs
        {
            /// <summary>
            /// Create new LinkUnlinkEventArgs.
            /// </summary>
            /// <param name="cp">The ContentPresenter.</param>
            public LinkUnlinkEventArgs(ContentPresenter cp)
            {
                ContentPresenter = cp;
            }

            /// <summary>
            /// The ContentPresenter which is displaying the item.
            /// </summary>
            public ContentPresenter ContentPresenter { get; private set; }
        }

}
