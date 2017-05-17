// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Class representing a Tutorial. 
    /// </summary>
    public class Tutorial
    {
        /// <summary>
        /// Gets or sets the friendly name representation of this tutorial.
        /// </summary>
        /// <value>The name of the friendly.</value>
        public string TutorialName { get; set; }

        /// <summary>
        /// Gets or sets the description for the tutorial.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the link to the tutorial on Silverlight.net.
        /// </summary>
        /// <value>The number.</value>
        public string TutorialLink { get; set; }

        /// <summary>
        /// Initializes a new Tutorial class instance. This is a data-entry 
        /// friendly constructor.
        /// </summary>
        /// <param name="tutorialName">Name of the tutorial.</param>
        /// <param name="description">Description of the tutorial.</param>
        /// <param name="tutorialLink">Link to the Tutorial.</param>
        public Tutorial(string tutorialName, string description, string tutorialLink)
        {
            TutorialName = tutorialName;
            Description = description;
            TutorialLink = tutorialLink;
        }

        /// <summary>
        /// Represent the Tutorial Name.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the tutorial name.
        /// </returns>
        public override string ToString()
        {
            return TutorialName;
        }

        /// <summary>
        /// Gets a collection of Tutorials.
        /// </summary>
        public static Collection<Tutorial> Tutorials
        {
            get
            {
                return new Collection<Tutorial> 
                {
                    new Tutorial("Introduction", "Jesse Liberty shows how to install the Silverlight Toolkit and where to find resources on how to use it. ", "http://silverlight.net/learn/learnvideo.aspx?video=146244"),
                    new Tutorial("AutoComplete", "Jesse Liberty introduces the AutoCompleteBox from the Silverlight Toolkit and the infrastructure that he’ll be using in his entire video series on the Silverlight Toolkit. ", "http://silverlight.net/learn/learnvideo.aspx?video=146251"),
                    new Tutorial("Pie Chart", "In this video, Jesse Liberty demonstrates how to build a pie chart from the Silverlight Toolkit with a collection of business objects.", "http://silverlight.net/learn/learnvideo.aspx?video=156998"),
                    new Tutorial("Headers", "In this video Jesse Liberty demonstrates how to create and use the HeaderContentControl and the HeaderItemsControl from the Silverlight Toolkit, demonstrating their use with text and with images.", "http://silverlight.net/learn/learnvideo.aspx?video=164686"),
                    new Tutorial("WrapPanel", "In this video, Jesse Liberty demonstrates how to use the Wrapper Panel from the Silverlight Toolkit to add controls and have them automatically placed one beside another until there is not enough room, at which time the next control will be placed on the next row", "http://silverlight.net/learn/learnvideo.aspx?video=180072"),
                    new Tutorial("Expander", "In this video Jesse Liberty demonstrates how to create and use the Expander control and then how to template the control to cause its contents to fade in and out at a pace you set. ", "http://silverlight.net/learn/learnvideo.aspx?video=183406")
                };
            }
        }
    }
}