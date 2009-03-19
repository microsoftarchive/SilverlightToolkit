//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataFormTests_Base.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Provides the framework for testing the <see cref="DataForm"/>.
    /// </summary>
    public class DataFormTests_Base : SilverlightTest
    {
        /// <summary>
        /// Holds whether the current item has changed.
        /// </summary>
        private bool currentItemChanged;

        /// <summary>
        /// Holds whether the field edit has ended.
        /// </summary>
        private bool fieldEditEnded;

        /// <summary>
        /// Holds whether the expected events have occurred.
        /// </summary>
        private bool eventOccurred;

        /// <summary>
        /// Holds whether the <see cref="DataForm"/> has loaded.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// Holds whether the item edit has ended.
        /// </summary>
        private bool itemEditEnded;

        /// <summary>
        /// Holds whether an element has got focus.
        /// </summary>
        private bool gotFocus;

        /// <summary>
        /// Holds whether an element has lost focus.
        /// </summary>
        private bool lostFocus;

        /// <summary>
        /// Holds whether the content has been loaded.
        /// </summary>
        private bool contentLoaded;

        /// <summary>
        /// Holds the element getting or losing focus.
        /// </summary>
        private FrameworkElement elementExpectingFocusChange;

        /// <summary>
        /// Gets or sets the base <see cref="DataForm"/> app.
        /// </summary>
        protected UserControl DataFormAppBase
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        [TestInitialize]
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Cleans up the test framework.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.RemoveFromPanel();
        }

        /// <summary>
        /// Invokes a button.
        /// </summary>
        /// <param name="button">The button to invoke.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This method should only be called on a button.")]
        protected static void InvokeButton(ButtonBase button)
        {
            ButtonBaseAutomationPeer buttonPeer = ButtonBaseAutomationPeer.CreatePeerForElement(button) as ButtonBaseAutomationPeer;
            IInvokeProvider buttonInvokeProvider = buttonPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            buttonInvokeProvider.Invoke();
        }

        /// <summary>
        /// Sets focus on an element.
        /// </summary>
        /// <param name="element">The element to set focus on.</param>
        protected static void SetFocus(UIElement element)
        {
            FrameworkElementAutomationPeer elementPeer = FrameworkElementAutomationPeer.CreatePeerForElement(element) as FrameworkElementAutomationPeer;

            if (elementPeer.IsKeyboardFocusable())
            {
                elementPeer.SetFocus();
            }
        }

        /// <summary>
        /// Sets the value of an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value to set the element's value to.</param>
        protected static void SetValue(UIElement element, string value)
        {
            FrameworkElementAutomationPeer elementPeer = FrameworkElementAutomationPeer.CreatePeerForElement(element) as FrameworkElementAutomationPeer;
            IValueProvider elementValueProvider = elementPeer.GetPattern(PatternInterface.Value) as IValueProvider;
            elementValueProvider.SetValue(value);
        }

        /// <summary>
        /// Add the <see cref="DataForm"/> app to the panel and wait for the <see cref="DataForm"/> to load.
        /// </summary>
        protected void AddToPanelAndWaitForLoad()
        {
            DataForm dataForm = null;

            this.EnqueueCallback(() =>
            {
                dataForm = this.GetDataForm();
                this.isLoaded = false;
                dataForm.Loaded += new RoutedEventHandler(this.OnLoaded);
                this.TestPanel.Children.Add(this.DataFormAppBase);
            });
            this.EnqueueConditional(() => this.isLoaded);
            this.EnqueueCallback(() => dataForm.Loaded -= new RoutedEventHandler(this.OnLoaded));
        }

        /// <summary>
        /// Commits all fields on the DataForm.
        /// </summary>
        protected void CommitAllFields()
        {
            DataForm dataForm = this.GetDataForm();

            foreach (DataFormField field in dataForm.Fields)
            {
                dataForm.CommitFieldEdit(field);
            }
        }

        /// <summary>
        /// Expect a current item change.
        /// </summary>
        protected void ExpectCurrentItemChange()
        {
            this.currentItemChanged = false;
            this.GetDataForm().CurrentItemChanged += new EventHandler<EventArgs>(this.OnDataFormCurrentItemChanged);
        }

        /// <summary>
        /// Expect the ending of the field edit.
        /// </summary>
        protected void ExpectFieldEditEnded()
        {
            this.fieldEditEnded = false;
            this.GetDataForm().FieldEditEnded += new EventHandler<DataFormFieldEditEndedEventArgs>(this.OnDataFormFieldEditEnded);
        }

        /// <summary>
        /// Expect the ending of the item edit.
        /// </summary>
        protected void ExpectItemEditEnded()
        {
            this.itemEditEnded = false;
            this.GetDataForm().ItemEditEnded += new EventHandler<DataFormItemEditEndedEventArgs>(this.OnDataFormItemEditEnded);
        }

        /// <summary>
        /// Expect the form's content to be loaded.
        /// </summary>
        protected void ExpectContentLoaded()
        {
            this.contentLoaded = false;
            this.GetDataForm().ContentLoaded += new EventHandler<DataFormContentLoadedEventArgs>(this.OnDataFormContentLoaded);
        }

        /// <summary>
        /// Expect am event.
        /// </summary>
        /// <param name="eventName">The name of the event to expect.</param>
        /// <typeparam name="TEventArgs">The type of event args to expect.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Doing this would require a method per event arg type, which is unnecessary.")]
        protected void ExpectEvent<TEventArgs>(string eventName)
            where TEventArgs : EventArgs
        {
            EventInfo eventInfo = typeof(DataForm).GetEvent(eventName);
            eventInfo.AddEventHandler(this.GetDataForm(), new EventHandler<TEventArgs>(this.OnEvent<TEventArgs>));
            this.eventOccurred = false;
        }

        /// <summary>
        /// Expect a GotFocus event on an element.
        /// </summary>
        /// <param name="element">The element.</param>
        protected void ExpectGotFocusOn(FrameworkElement element)
        {
            this.gotFocus = false;
            element.GotFocus += new RoutedEventHandler(this.OnGotFocus);
            this.elementExpectingFocusChange = element;
        }

        /// <summary>
        /// Expect a LostFocus event on an element.
        /// </summary>
        /// <param name="element">The element.</param>
        protected void ExpectLostFocusOn(FrameworkElement element)
        {
            this.lostFocus = false;
            element.LostFocus += new RoutedEventHandler(this.OnLostFocus);
            this.elementExpectingFocusChange = element;
        }

        /// <summary>
        /// Gets the position of an element
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The position.</returns>
        protected Point GetPosition(UIElement element)
        {
            GeneralTransform generalTransform = element.TransformToVisual(this.TestPanel);
            return generalTransform.Transform(new Point(0, 0));
        }

        /// <summary>
        /// Get a template part.
        /// </summary>
        /// <param name="partName">The part name.</param>
        /// <typeparam name="T">The type of the template part to expect.</typeparam>
        /// <returns>The template part.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Doing this would require a method per template part type, which is unnecessary.")]
        protected T GetTemplatePart<T>(string partName)
            where T : DependencyObject
        {
            return this.GetDataForm().FindChildByName(partName) as T;
        }

        /// <summary>
        /// Remove the <see cref="DataForm"/> app from the panel, if it's been added.
        /// </summary>
        protected void RemoveFromPanel()
        {
            if (this.DataFormAppBase != null && this.TestPanel.Children.Contains(this.DataFormAppBase))
            {
                this.TestPanel.Children.Remove(this.DataFormAppBase);
            }
        }

        /// <summary>
        /// Wait for a current item change.
        /// </summary>
        protected void WaitForCurrentItemChange()
        {
            DataForm dataForm = null;

            this.EnqueueCallback(() => dataForm = this.GetDataForm());
            this.EnqueueConditional(() => this.currentItemChanged);
            this.EnqueueCallback(() => dataForm.CurrentItemChanged -= new EventHandler<EventArgs>(this.OnDataFormCurrentItemChanged));
        }

        /// <summary>
        /// Wait for the field edit being ended.
        /// </summary>
        protected void WaitForFieldEditEnded()
        {
            DataForm dataForm = null;

            this.EnqueueCallback(() => dataForm = this.GetDataForm());
            this.EnqueueConditional(() => this.fieldEditEnded);
            this.EnqueueCallback(() => dataForm.FieldEditEnded -= new EventHandler<DataFormFieldEditEndedEventArgs>(this.OnDataFormFieldEditEnded));
        }

        /// <summary>
        /// Wait for the item edit being ended.
        /// </summary>
        protected void WaitForItemEditEnded()
        {
            DataForm dataForm = null;

            this.EnqueueCallback(() => dataForm = this.GetDataForm());
            this.EnqueueConditional(() => this.itemEditEnded);
            this.EnqueueCallback(() => dataForm.ItemEditEnded -= new EventHandler<DataFormItemEditEndedEventArgs>(this.OnDataFormItemEditEnded));
        }

        /// <summary>
        /// Wait for the form's content to be loaded.
        /// </summary>
        protected void WaitForContentLoaded()
        {
            DataForm dataForm = null;

            this.EnqueueCallback(() => dataForm = this.GetDataForm());
            this.EnqueueConditional(() => this.contentLoaded);
            this.EnqueueCallback(() => dataForm.ContentLoaded -= new EventHandler<DataFormContentLoadedEventArgs>(this.OnDataFormContentLoaded));
        }

        /// <summary>
        /// Wait for a current item change.
        /// </summary>
        protected void WaitForAllEvents()
        {
            bool timeExpired = false;
            DateTime timeStartedWaiting = new DateTime();

            this.EnqueueCallback(() => timeStartedWaiting = DateTime.Now);

            this.EnqueueConditional(() =>
            {
                if ((DateTime.Now - timeStartedWaiting).TotalMilliseconds > 1000)
                {
                    timeExpired = true;
                    return true;
                }

                return this.eventOccurred;
            });

            this.EnqueueCallback(() =>
            {
                if (timeExpired)
                {
                    Assert.Fail("At least one expected event did not occur.");
                }
            });
        }

        /// <summary>
        /// Wait until a GotFocus event has occurred.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ForGot", Justification = "GotFocus is the name of the event being waited for.")]
        protected void WaitForGotFocus()
        {
            this.EnqueueConditional(() => this.gotFocus);
            this.EnqueueCallback(() => this.elementExpectingFocusChange.GotFocus -= new RoutedEventHandler(this.OnGotFocus));
        }

        /// <summary>
        /// Wait until a LostFocus event has occurred.
        /// </summary>
        protected void WaitForLostFocus()
        {
            this.EnqueueConditional(() => this.lostFocus);
            this.EnqueueCallback(() => this.elementExpectingFocusChange.LostFocus -= new RoutedEventHandler(this.OnLostFocus));
        }

        /// <summary>
        /// Get the <see cref="DataForm"/> from the app.
        /// </summary>
        /// <returns>The <see cref="DataForm"/>.</returns>
        private DataForm GetDataForm()
        {
            Assert.IsNotNull(this.DataFormAppBase, "DataFormAppBase must not be null.");
            DataForm dataForm = this.DataFormAppBase.FindChildByName("dataForm") as DataForm;
            Assert.IsNotNull(dataForm, "DataFormAppBase must contain a DataForm.");
            return dataForm;
        }


        /// <summary>
        /// Handles the form's content being loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormContentLoaded(object sender, DataFormContentLoadedEventArgs e)
        {
            this.contentLoaded = true;
        }

        /// <summary>
        /// Handles a current item change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormCurrentItemChanged(object sender, EventArgs e)
        {
            this.currentItemChanged = true;
        }

        /// <summary>
        /// Handles the ending of the field.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormFieldEditEnded(object sender, DataFormFieldEditEndedEventArgs e)
        {
            this.fieldEditEnded = true;
        }

        /// <summary>
        /// Handles the ending of the current item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnDataFormItemEditEnded(object sender, DataFormItemEditEndedEventArgs e)
        {
            this.itemEditEnded = true;
        }

        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        /// <typeparam name="TEventArgs">The type of event args to expect.</typeparam>
        private void OnEvent<TEventArgs>(object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            this.eventOccurred = true;
        }

        /// <summary>
        /// Handles a Loaded event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
        }

        /// <summary>
        /// Handles a GotFocus event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            this.gotFocus = true;
        }

        /// <summary>
        /// Handles a LostFocus event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.lostFocus = true;
        }
    }
}