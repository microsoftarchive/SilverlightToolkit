// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Helper for the phone.
    /// </summary>
    /// <remarks>
    /// All orientations are condensed into portrait and landscape, where landscape includes <see cref="PageOrientation.None"/>.
    /// </remarks>
    internal static class PhoneHelper
    {
        /// <summary>
        /// The height of the SIP in landscape orientation.
        /// </summary>
        public const double SipLandscapeHeight = 259;

        /// <summary>
        /// The height of the SIP in portrait orientation.
        /// </summary>
        public const double SipPortraitHeight = 339;

        /// <summary>
        /// The height of the SIP text completion in either orientation.
        /// </summary>
        public const double SipTextCompletionHeight = 62;

        /// <summary>
        /// Gets the current <see cref="T:PhoneApplicationFrame"/>.
        /// </summary>
        /// <param name="phoneApplicationFrame">The current <see cref="T:PhoneApplicationFrame"/>.</param>
        /// <returns><code>true</code> if the current <see cref="T:PhoneApplicationFrame"/> was found; <code>false</code> otherwise.</returns>
        public static bool TryGetPhoneApplicationFrame(out PhoneApplicationFrame phoneApplicationFrame)
        {
            phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            return phoneApplicationFrame != null;
        }

        /// <summary>
        /// Determines whether a <see cref="T:PhoneApplicationFrame"/> is oriented as portrait.
        /// </summary>
        /// <param name="phoneApplicationFrame">The <see cref="T:PhoneApplicationFrame"/>.</param>
        /// <returns><code>true</code> if the <see cref="T:PhoneApplicationFrame"/> is oriented as portrait; <code>false</code> otherwise.</returns>
        public static bool IsPortrait(this PhoneApplicationFrame phoneApplicationFrame)
        {
            PageOrientation portrait = PageOrientation.Portrait | PageOrientation.PortraitDown | PageOrientation.PortraitUp;
            return (portrait & phoneApplicationFrame.Orientation) == phoneApplicationFrame.Orientation;
        }

        /// <summary>
        /// Gets the correct width of a <see cref="T:PhoneApplicationFrame"/> in either orientation.
        /// </summary>
        /// <param name="phoneApplicationFrame">The <see cref="T:PhoneApplicationFrame"/>.</param>
        /// <returns>The width.</returns>
        public static double GetUsefulWidth(this PhoneApplicationFrame phoneApplicationFrame)
        {
            return phoneApplicationFrame.IsPortrait() ? phoneApplicationFrame.ActualWidth : phoneApplicationFrame.ActualHeight;
        }

        /// <summary>
        /// Gets the correct height of a <see cref="T:PhoneApplicationFrame"/> in either orientation.
        /// </summary>
        /// <param name="phoneApplicationFrame">The <see cref="T:PhoneApplicationFrame"/>.</param>
        /// <returns>The height.</returns>
        public static double GetUsefulHeight(this PhoneApplicationFrame phoneApplicationFrame)
        {
            return IsPortrait(phoneApplicationFrame) ? phoneApplicationFrame.ActualHeight : phoneApplicationFrame.ActualWidth;
        }

        /// <summary>
        /// Gets the correct <see cref="T:Size"/> of a <see cref="T:PhoneApplicationFrame"/>.
        /// </summary>
        /// <param name="phoneApplicationFrame">The <see cref="T:PhoneApplicationFrame"/>.</param>
        /// <returns>The <see cref="T:Size"/>.</returns>
        public static Size GetUsefulSize(this PhoneApplicationFrame phoneApplicationFrame)
        {
            return new Size(phoneApplicationFrame.GetUsefulWidth(), phoneApplicationFrame.GetUsefulHeight());
        }

        /// <summary>
        /// Gets the focused <see cref="T:TextBox"/>, if there is one.
        /// </summary>
        /// <param name="textBox">The <see cref="T:TextBox"/>.</param>
        /// <returns><code>true</code> if there is a focused <see cref="T:TextBox"/>; <code>false</code> otherwise.</returns>
        private static bool TryGetFocusedTextBox(out TextBox textBox)
        {
            textBox = FocusManager.GetFocusedElement() as TextBox;
            return textBox != null;
        }

        /// <summary>
        /// Determines whether the SIP is shown.
        /// </summary>
        /// <returns><code>true</code> if the SIP is shown; <code>false</code> otherwise.</returns>
        public static bool IsSipShown()
        {
            TextBox textBox;
            return TryGetFocusedTextBox(out textBox);
        }

        /// <summary>
        /// Determines whether the <see cref="T:TextBox"/> would show the SIP text completion.
        /// </summary>
        /// <param name="textBox">The <see cref="T:TextBox"/>.</param>
        /// <returns><code>true</code> if the <see cref="T:TextBox"/> woudl show the SIP text completion; <code>false</code> otherwise.</returns>
        public static bool IsSipTextCompletionShown(this TextBox textBox)
        {
            if (textBox.InputScope == null)
            {
                return false;
            }
            IList inputScopeNames = textBox.InputScope.Names;
            foreach (InputScopeName inputScopeName in inputScopeNames)
            {
                switch (inputScopeName.NameValue)
                {
                    case InputScopeNameValue.Text:
                    case InputScopeNameValue.Chat:
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the <see cref="T:Size"/> covered by the SIP when it is shown.
        /// </summary>
        /// <param name="phoneApplicationFrame">The <see cref="T:PhoneApplicationFrame"/>.</param>
        /// <returns>The <see cref="T:Size"/>.</returns>
        public static Size GetSipCoveredSize(this PhoneApplicationFrame phoneApplicationFrame)
        {
            if (!IsSipShown())
            {
                return new Size(0, 0);
            }
            double width = phoneApplicationFrame.GetUsefulWidth();
            double height = phoneApplicationFrame.IsPortrait() ? SipPortraitHeight : SipLandscapeHeight;
            TextBox textBox;
            if (TryGetFocusedTextBox(out textBox) && textBox.IsSipTextCompletionShown())
            {
                height += SipTextCompletionHeight;
            }
            return new Size(width, height);
        }

        /// <summary>
        /// Gets the <see cref="T:Size"/> uncovered by the SIP when it is shown.
        /// </summary>
        /// <param name="phoneApplicationFrame">The <see cref="T:PhoneApplicationFrame"/>.</param>
        /// <returns>The <see cref="T:Size"/>.</returns>
        public static Size GetSipUncoveredSize(this PhoneApplicationFrame phoneApplicationFrame)
        {
            double width = phoneApplicationFrame.GetUsefulWidth();
            double height = phoneApplicationFrame.GetUsefulHeight() - phoneApplicationFrame.GetSipCoveredSize().Height;
            return new Size(width, height);
        }
    }
}