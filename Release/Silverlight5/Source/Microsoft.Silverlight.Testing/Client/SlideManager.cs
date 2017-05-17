// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A helper class to managing sets of slides.
    /// </summary>
    public class SlideManager
    {
        /// <summary>
        /// Initializes a new instance of the SlideManager class.
        /// </summary>
        public SlideManager()
        {
            _slides = new LinkedList<Slide>();
        }

        /// <summary>
        /// Stores the linked list node for the current slide.
        /// </summary>
        private LinkedListNode<Slide> _current;

        /// <summary>
        /// Backing field for the set of slides.
        /// </summary>
        private LinkedList<Slide> _slides;

        /// <summary>
        /// Gets the current slide instance.
        /// </summary>
        public Slide Current
        {
            get
            {
                if (_current == null)
                {
                    return null;
                }

                return _current.Value;
            }
        }

        /// <summary>
        /// Adds a slide or an array of slides to the managed slide set.
        /// </summary>
        /// <param name="slides">The slides to manage.</param>
        public void Add(params Slide[] slides)
        {
            if (slides != null && slides.Length > 0)
            {
                LinkedListNode<Slide> before = _slides.First;
                for (int i = 0; i < slides.Length; i++)
                {
                    Slide slide = slides[i];
                    slide.SlideManager = this;
                    LinkedListNode<Slide> lln = InsertAfter(before, slide);
                    /*if (i == 0 && _slides.First == lln)
                    {
                        _current = lln;
                    }*/
                    // InitializePosition(slide, i == 0 && _current == lln);
                }
            }
        }

        /// <summary>
        /// Initializes the position of the slide based on location.
        /// </summary>
        /// <param name="slide">The slide instance.</param>
        /// <param name="isFirst">A property indicating whether the slide is the
        /// first to be managed.</param>
        private void InitializePosition(Slide slide, bool isFirst)
        {
            SlidePosition c = slide.Position;
            SlidePosition expected = isFirst ? SlidePosition.Normal : SlidePosition.Right;
            if (c != expected)
            {
                slide.Position = expected;
            }
            // CONSIDER: Need to modify all others as well?

            if (_slides.First != null && _slides.First.Value == slide)
            {
                _current = _slides.First;
            }
        }

        /// <summary>
        /// Manages a new slide, inserting it after an existing slide node.
        /// </summary>
        /// <param name="before">The node to insert after.</param>
        /// <param name="newSlide">The new slide instance.</param>
        /// <returns>Returns the linked list node that is inserted.</returns>
        public LinkedListNode<Slide> InsertAfter(Slide before, Slide newSlide)
        {
            if (before == null)
            {
                return InsertFirst(newSlide);
            }

            LinkedListNode<Slide> node = _slides.Find(before);
            return InsertAfter(node, newSlide);
        }

        /// <summary>
        /// Inserts a slide as the first in the linked list.
        /// </summary>
        /// <param name="newSlide">The new slide instance.</param>
        /// <returns>Returns the linked list node.</returns>
        public LinkedListNode<Slide> InsertFirst(Slide newSlide)
        {
            LinkedListNode<Slide> node = _slides.AddFirst(newSlide);
            InitializePosition(newSlide, true);
            return node;
        }

        /// <summary>
        /// Insert a slide after a provided linked list node.
        /// </summary>
        /// <param name="before">The node to insert after.</param>
        /// <param name="newSlide">The new slide to insert.</param>
        /// <returns>Returns the new linked list node.</returns>
        public LinkedListNode<Slide> InsertAfter(LinkedListNode<Slide> before, Slide newSlide)
        {
            LinkedListNode<Slide> newNode;
            if (before == null)
            {
                newNode = _slides.AddFirst(newSlide);
            }
            else
            {
                newNode = _slides.AddAfter(before, newSlide);
            }
            InitializePosition(newSlide, before == null);
            return newNode;
        }

        /// <summary>
        /// Remove a slide from management.
        /// </summary>
        /// <param name="slide">The slide instance.</param>
        public void Remove(Slide slide)
        {
            _slides.Remove(slide);
        }

        /// <summary>
        /// Move to the previous slide.
        /// </summary>
        public void Previous()
        {
            Move(false);
        }

        /// <summary>
        /// Move to the next slide.
        /// </summary>
        public void Next()
        {
            Move(true);
        }

        /// <summary>
        /// Moves to a specific slide, moving the others to the appropriate
        /// direction on screen.
        /// </summary>
        /// <param name="slide">The slide to move to.</param>
        public void MoveTo(Slide slide)
        {
            LinkedListNode<Slide> sn = _slides.Find(slide);
            if (sn != null)
            {
                // Before
                LinkedListNode<Slide> otherNode = sn.Previous;
                while (otherNode != null)
                {
                    otherNode.Value.Position = SlidePosition.Right;
                    otherNode = otherNode.Previous;
                }

                // The slide
                slide.Position = SlidePosition.Normal;
                _current = sn;

                // After
                otherNode = sn.Next;
                while (otherNode != null)
                {
                    otherNode.Value.Position = SlidePosition.Left;
                    otherNode = otherNode.Next;
                }
            }
        }

        /// <summary>
        /// Move in a direction.
        /// </summary>
        /// <param name="forward">A value indicating whether the direction to
        /// move is forward or not.</param>
        private void Move(bool forward)
        {
            LinkedListNode<Slide> oldNode = _current;
            Slide old = oldNode.Value;

            SlidePosition sp = SlidePosition.Normal;
            if (forward && oldNode.Next != null)
            {
                _current = oldNode.Next;
                sp = SlidePosition.Left;
            }
            else if (!forward && oldNode.Previous != null)
            {
                _current = oldNode.Previous;
                sp = SlidePosition.Right;
            }
            if (oldNode != _current)
            {
                _current.Value.Position = SlidePosition.Normal;
                old.Position = sp;
            }
        }
    }
}