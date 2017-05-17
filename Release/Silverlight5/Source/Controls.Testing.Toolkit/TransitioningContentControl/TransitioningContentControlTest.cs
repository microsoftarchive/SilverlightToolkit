// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for TransitioningContentControl.
    /// </summary>
    [TestClass]
    public class TransitioningContentControlTest : ContentControlTest
    {
        #region instances to test
        /// <summary>
        /// Gets a default instance of ContentControl (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override ContentControl DefaultContentControlToTest
        {
            get { return DefaultTransitioningContentControlToTest; }
        }

        /// <summary>
        /// Gets instances of ContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<ContentControl> ContentControlsToTest
        {
            get { return (IEnumerable<ContentControl>)TransitioningContentControlsTest; }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get { return OverriddenTransitioningContentControlsToTest; }
        }

        /// <summary>
        /// Gets the default transitioning content control to test.
        /// </summary>
        /// <value>The default transitioning content control to test.</value>
        public virtual TransitioningContentControl DefaultTransitioningContentControlToTest
        {
            get { return new TransitioningContentControl(); }
        }

        /// <summary>
        /// Gets the transitioning content controls test.
        /// </summary>
        /// <value>The transitioning content controls test.</value>
        public virtual IEnumerable<TransitioningContentControl> TransitioningContentControlsTest
        {
            get { yield return DefaultTransitioningContentControlToTest; }
        }

        /// <summary>
        /// Gets the overridden transitioning content controls to test.
        /// </summary>
        /// <value>The overridden transitioning content controls to test.</value>
        public virtual IEnumerable<IOverriddenContentControl> OverriddenTransitioningContentControlsToTest
        {
            get { yield break; }
        } 
        #endregion

        /// <summary>
        /// Gets the Transition dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TransitioningContentControl, string> TransitionProperty { get; private set; }

        /// <summary>
        /// Gets the RestartTransitionOnContentChange dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TransitioningContentControl, bool> RestartTransitionProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="TransitioningContentControlTest"/> class.
        /// </summary>
        public TransitioningContentControlTest()
        {
            // control should not be used with UIElements
            ContentProperty.OtherValues = new object[] { 12, "Test Text", Environment.OSVersion };

            Func<TransitioningContentControl> initializer = () => new TransitioningContentControl();

            TransitionProperty = new DependencyPropertyTest<TransitioningContentControl, string>(this, "Transition")
                                     {
                                         Initializer = initializer,
                                         Property = TransitioningContentControl.TransitionProperty,
                                         DefaultValue = "DefaultTransition",
                                         OtherValues = new[] { "UpTransition", "DownTransition" }
                                     };

            RestartTransitionProperty = new DependencyPropertyTest<TransitioningContentControl, bool>(this, "RestartTransitionOnContentChange")
                                            {
                                                Initializer = initializer,
                                                Property = TransitioningContentControl.RestartTransitionOnContentChangeProperty,
                                                DefaultValue = false,
                                                OtherValues = new[] { true }
                                            };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // will not work because there is a transition going on
            tests.Where(method => method.PropertyName == "Content" && method.Name.Contains("DataTemplate")).ToList().ForEach(method => tests.Remove(method));

            // cannot test TemplateBinding since retemplating is expected to 
            // throw an exception if the transitions are not defined.
            tests.Where(method => method.Name.Contains("TemplateBind")).ToList().ForEach(method => tests.Remove(method));

            // changing the content actually will trigger a visualstate change
            tests.Remove(tests.First(method => method.PropertyName == "Content" && method.Name.Contains("DoesNotChangeVisualState")));

            // TransitionProperty tests
            tests.Add(TransitionProperty.CheckDefaultValueTest);
            tests.Add(TransitionProperty.ChangeClrSetterTest);
            tests.Add(TransitionProperty.ChangeSetValueTest);
            tests.Add(TransitionProperty.ClearValueResetsDefaultTest);
            tests.Add(TransitionProperty.CanBeStyledTest);
            tests.Add(TransitionProperty.SetXamlAttributeTest);

            // RestartTransitionOnContentChange tests
            tests.Add(RestartTransitionProperty.CheckDefaultValueTest);
            tests.Add(RestartTransitionProperty.ChangeClrSetterTest);
            tests.Add(RestartTransitionProperty.ChangeSetValueTest);
            tests.Add(RestartTransitionProperty.ClearValueResetsDefaultTest);
            tests.Add(RestartTransitionProperty.CanBeStyledTest);
            tests.Add(RestartTransitionProperty.SetXamlAttributeTest);

            return tests;
        }

        #region control contract
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultTransitioningContentControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(2, templateParts.Count);
            Assert.AreSame(typeof(ContentControl), templateParts["PreviousContentPresentationSite"]);
            Assert.AreSame(typeof(ContentControl), templateParts["CurrentContentPresentationSite"]);
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultTransitioningContentControlToTest.GetType().GetVisualStates();
            Assert.AreEqual(2, visualStates.Count);

            Assert.AreEqual("PresentationStates", visualStates["Normal"]);
            Assert.AreEqual("PresentationStates", visualStates["DefaultTransition"]);
        }
        #endregion

        /// <summary>
        /// Tests that transition names can be set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that transition names can be set.")]
        public virtual void ShouldBeAbleToSetValidTransitionNames()
        {
            TransitioningContentControl tcc = DefaultTransitioningContentControlToTest;

            TestAsync(
                tcc,
                () => Assert.AreEqual("DefaultTransition", tcc.Transition),
                () => tcc.Transition = "UpTransition",
                () => Assert.AreEqual("UpTransition", tcc.Transition),
                () => tcc.Transition = "DownTransition",
                () => Assert.AreEqual("DownTransition", tcc.Transition));
        }

        /// <summary>
        /// Tests that invalid transition will throw.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Asynchronous]
        [Description("Tests that invalid transition names are silently coerced.")]
        public virtual void ShouldThrowExceptionOnInvalidTransitionName()
        {
            // the control will cache the invalid Transition until such time
            // that the template is applied, so that it can be validated.
            TransitioningContentControl tcc = DefaultTransitioningContentControlToTest;

            TestAsync(
                tcc,
                () => tcc.Transition = "non exisiting transition");
        }

        /// <summary>
        /// Tests that transition completed event fires.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that transition completed event fires.")]
        public virtual void ShouldFireTransitionCompletedEvent()
        {
            TransitioningContentControl tcc = DefaultTransitioningContentControlToTest;

            int count = 0;
            tcc.TransitionCompleted += (sender, e) => count++;

            bool tccIsLoaded = false;
            tcc.Loaded += delegate { tccIsLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            EnqueueCallback(() => TestPanel.Children.Add(tcc));
            EnqueueConditional(() => tccIsLoaded);

            EnqueueCallback(() => tcc.Content = "different content");
            EnqueueCallback(() => Assert.AreEqual(0, count));
            EnqueueDelay(500);  // template has 300 ms transitions
            EnqueueCallback(() => Assert.AreEqual(1, count, "Should fire completed event."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that transition completed event fires once when quickly changing content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that transition completed event fires once when quickly changing content.")]
        public virtual void ShouldFireCompletedEventOnceWithinTransition()
        {
            TransitioningContentControl tcc = DefaultTransitioningContentControlToTest;

            int count = 0;
            tcc.TransitionCompleted += (sender, e) => count++;

            bool tccIsLoaded = false;
            tcc.Loaded += delegate { tccIsLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            EnqueueCallback(() => TestPanel.Children.Add(tcc));
            EnqueueConditional(() => tccIsLoaded);

            EnqueueCallback(() => tcc.Content = "different content");
            EnqueueCallback(() => Assert.AreEqual(0, count));
            EnqueueCallback(() => tcc.Content = "different content2");
            EnqueueCallback(() => tcc.Content = "different content3");
            EnqueueCallback(() => tcc.Content = "different content4");
            EnqueueDelay(500);  // template has 300 ms transitions
            EnqueueCallback(() => Assert.AreEqual(1, count, "Should fire completed event."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that IsTransitioningProperty defaults to unset value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that IsTransitioningProperty defaults to unset value.")]
        public virtual void ShouldNotSetIsTransitioningProperty()
        {
            TransitioningContentControl tcc = new TransitioningContentControl();
            TestAsync(
                tcc,
                () => Assert.IsTrue(DependencyProperty.UnsetValue == tcc.ReadLocalValue(TransitioningContentControl.IsTransitioningProperty)));
        }

        /// <summary>
        /// Tests that the same visual element is allowed in the transition.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the same visual element is allowed in the transition.")]
        public virtual void ShouldWorkWithVisuals()
        {
            List<Rectangle> rectangles = new List<Rectangle>
                               {
                                   new Rectangle { Height = 30, Width = 30, Fill = new SolidColorBrush(Colors.Red) },
                                   new Rectangle { Height = 30, Width = 30, Fill = new SolidColorBrush(Colors.Green) },
                                   new Rectangle { Height = 30, Width = 30, Fill = new SolidColorBrush(Colors.Blue) },
                               };

            TransitioningContentControl tcc = new TransitioningContentControl();

            bool tccIsLoaded = false;
            tcc.Loaded += delegate { tccIsLoaded = true; };

            int count = 0;
            tcc.TransitionCompleted += (sender, e) => count++;

            EnqueueCallback(() => TestPanel.Children.Add(tcc));
            EnqueueConditional(() => tccIsLoaded);

            EnqueueCallback(() => tcc.Content = rectangles[0]);
            EnqueueConditional(() => count == 1);

            EnqueueCallback(() => tcc.Content = rectangles[1]);
            EnqueueConditional(() => count == 2);

            EnqueueCallback(() => tcc.Content = rectangles[2]);
            EnqueueConditional(() => count == 3);

            EnqueueTestComplete();
        }
    }
}
