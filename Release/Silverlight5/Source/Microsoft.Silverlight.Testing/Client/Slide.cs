// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A specialized content control that contains a fixed header, a standard
    /// header content property, plus content. It is designed specifically for
    /// a "slide-based" user interface for simple user interfaces.
    /// </summary>
    [TemplatePart(Name = PartContent, Type = typeof(object))]
    [TemplatePart(Name = PartHeader, Type = typeof(object))]
    [TemplatePart(Name = PartFixedHeader, Type = typeof(object))]
    [TemplateVisualState(Name = StatePositionLeft, GroupName = GroupPosition)]
    [TemplateVisualState(Name = StatePositionRight, GroupName = GroupPosition)]
    [TemplateVisualState(Name = StatePositionNormal, GroupName = GroupPosition)]
    public class Slide : ContentControl
    {
        /// <summary>
        /// The visual state group name for slide position.
        /// </summary>
        private const string GroupPosition = "SlidePosition";

        /// <summary>
        /// The visual state name for left position.
        /// </summary>
        private const string StatePositionLeft = "PositionLeft";
        
        /// <summary>
        /// The visual state name for right position.
        /// </summary>
        private const string StatePositionRight = "PositionRight";
        
        /// <summary>
        /// The normal visual state name for position.
        /// </summary>
        private const string StatePositionNormal = "PositionNormal";

        /// <summary>
        /// The content template part name.
        /// </summary>
        private const string PartContent = "Content";

        /// <summary>
        /// The header template part name.
        /// </summary>
        private const string PartHeader = "Header";
        
        /// <summary>
        /// The fixed header template name.
        /// </summary>
        private const string PartFixedHeader = "FixedHeader";

        /// <summary>
        /// The manager of the slide and its siblings.
        /// </summary>
        private SlideManager _parent;

        /// <summary>
        /// Event fired when the current slide changes.
        /// </summary>
        public event EventHandler SlideChanged;

        #region public object Header
        /// <summary>
        /// Gets or sets the primary header content.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(Slide),
                new PropertyMetadata(null));
        #endregion public object Header

        #region public object FixedHeader
        /// <summary>
        /// Gets or sets the fixed header content.
        /// </summary>
        public object FixedHeader
        {
            get { return GetValue(FixedHeaderProperty); }
            set { SetValue(FixedHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the FixedHeader dependency property.
        /// </summary>
        public static readonly DependencyProperty FixedHeaderProperty =
            DependencyProperty.Register(
                "FixedHeader",
                typeof(object),
                typeof(Slide),
                new PropertyMetadata(null));
        #endregion public object FixedHeader

        #region public SlidePosition Position
        /// <summary>
        /// Gets or sets the position of the slide.
        /// </summary>
        public SlidePosition Position
        {
            get { return (SlidePosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Identifies the Position dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                "Position",
                typeof(SlidePosition),
                typeof(Slide),
                new PropertyMetadata(SlidePosition.Normal, OnPositionPropertyChanged));

        /// <summary>
        /// PositionProperty property changed handler.
        /// </summary>
        /// <param name="d">Slide that changed its Position.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Slide source = d as Slide;
            SlidePosition value = (SlidePosition)e.NewValue;

            if (value != SlidePosition.Left &&
                value != SlidePosition.Normal &&
                value != SlidePosition.Right)
            {
                d.SetValue(e.Property, e.OldValue);
                return;
            }

            source.UpdateVisualState(true, value);
        }
        #endregion public SlidePosition Position

        /// <summary>
        /// Initializes a new instance of the Slide class.
        /// </summary>
        public Slide()
        {
            DefaultStyleKey = typeof(Slide);
        }

        /// <summary>
        /// Gets or sets the slide manager for the slide.
        /// </summary>
        public SlideManager SlideManager
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// Remove the slide from the parent manager.
        /// </summary>
        public void RemoveFromManager()
        {
            if (_parent != null)
            {
                _parent.Remove(this);
            }
        }

        /// <summary>
        /// Locate template parts and assign instances to fields during template
        /// application.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateGroup groupPosition = VisualStates.TryGetVisualStateGroup(this, GroupPosition);
            if (null != groupPosition)
            {
                groupPosition.CurrentStateChanged += OnCurrentStateChanged;
            }

            UpdateVisualState(false, Position);
        }

        /// <summary>
        /// Fires the slide changed event.
        /// </summary>
        protected void OnSlideChanged()
        {
            var handler = SlideChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires the current state changed event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            // Collapse the slide when "offscreen"
            if (e.NewState.Name != StatePositionNormal)
            {
                Visibility = Visibility.Collapsed;
                OnSlideChanged();
            }
        }

        /// <summary>
        /// Helps debugging by providing information about the slide name.
        /// </summary>
        /// <returns>Returns the name of the slide.</returns>
        public override string ToString()
        {
            string name = Name;
            if (!string.IsNullOrEmpty(name))
            {
                return name + " : " + base.ToString();
            }
            return base.ToString();
        }

        /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">A value indicating whether to use
        /// visual transitions for the state change.</param>
        /// <param name="sp">The slide position to use.</param>
        private void UpdateVisualState(bool useTransitions, SlidePosition sp)
        {
            string state;
            switch (Position)
            {
                case SlidePosition.Left:
                    state = StatePositionLeft;
                    break;

                case SlidePosition.Right:
                    state = StatePositionRight;
                    break;

                default:
                    state = StatePositionNormal;
                    break;
            }

            // Always expand the visual for a state change
            Visibility = Visibility.Visible;

            VisualStateManager.GoToState(this, state, false); // useTransitions);
        }
    }
}