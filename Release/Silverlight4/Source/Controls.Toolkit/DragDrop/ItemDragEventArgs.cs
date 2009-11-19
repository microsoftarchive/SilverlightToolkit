// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;

#if SILVERLIGHT
using SW = Microsoft.Windows;
#else
using SW = System.Windows;
#endif

namespace System.Windows.Controls
{
    /// <summary>
    /// Information describing a drag event on a UIElement.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class ItemDragEventArgs : SW.ExtendedRoutedEventArgs
    {
        /// <summary>
        /// Gets the key states.
        /// </summary>
        public SW.DragDropKeyStates KeyStates { get; internal set; }

        /// <summary>
        /// Gets or sets the allowed effects.
        /// </summary>
        public SW.DragDropEffects AllowedEffects { get; set; }

        /// <summary>
        /// Gets or sets the effects of the completed drag operation.
        /// </summary>
        public SW.DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets or sets the control that is the source of the drag.
        /// </summary>
        public DependencyObject DragSource { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the item container being dragged.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the mouse offset from the item being dragged at the 
        /// beginning of the drag operation.
        /// </summary>
        public Point DragDecoratorContentMouseOffset { get; set; }

        /// <summary>
        /// Gets or sets the content to insert into the DragDecorator.
        /// </summary>
        public object DragDecoratorContent { get; set; }

        /// <summary>
        /// Initializes a new instance of the ItemDragEventArgs class.
        /// </summary>
        internal ItemDragEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the action.
        /// </summary>
        public bool Cancel { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the ItemDragEventArgs class using an
        /// existing instance.
        /// </summary>
        /// <param name="args">The instance to use as the template when creating
        /// the new instance.</param>
        internal ItemDragEventArgs(ItemDragEventArgs args)
        {
            this.AllowedEffects = args.AllowedEffects;
            this.Effects = args.Effects;
            this.Data = args.Data;
            this.DragSource = args.DragSource;
            this.KeyStates = args.KeyStates;
            this.OriginalSource = args.OriginalSource;
            this.DragDecoratorContent = args.DragDecoratorContent;
            this.DragDecoratorContentMouseOffset = args.DragDecoratorContentMouseOffset;
        }
    }
}