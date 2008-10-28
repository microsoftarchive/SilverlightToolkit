// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.Windows.Controls.DataVisualization
{
    /// <summary>
    /// A range of values.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <QualityBand>Preview</QualityBand>
    public struct Range<T>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets an empty range.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "The object is immutable.")]
        public static readonly Range<T> Empty = new Range<T>();

        /// <summary>
        /// A flag that determines whether the range is empty or not.
        /// </summary>
        private bool _isNotEmpty;

        /// <summary>
        /// Gets a value indicating whether the range is empty or not.
        /// </summary>
        public bool IsEmpty 
        { 
            get 
            { 
                return !_isNotEmpty; 
            } 
        }

        /// <summary>
        /// The maximum value in the range.
        /// </summary>
        private T _maximum;

        /// <summary>
        /// Gets the maximum value in the range.
        /// </summary>
        public T Maximum
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException(Properties.Resources.Range_get_Maximum_CannotReadTheMaximumOfAnEmptyRange);
                }
                return _maximum;
            }
        }

        /// <summary>
        /// The minimum value in the range.
        /// </summary>
        private T _minimum;

        /// <summary>
        /// Gets the minimum value in the range.
        /// </summary>
        public T Minimum 
        { 
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException(Properties.Resources.Range_get_Minimum_CannotReadTheMinimumOfAnEmptyRange);
                }
                return _minimum;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Range class.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        public Range(T minimum, T maximum)
        {
            _isNotEmpty = true;
            _minimum = minimum;
            _maximum = maximum;

            int compareValue = minimum.CompareTo(maximum);
            if (compareValue == 1)
            {
                throw new InvalidOperationException(Properties.Resources.Range_ctor_MaximumValueMustBeLargerThanOrEqualToMinimumValue);
            }
        }

        /// <summary>
        /// Compare two ranges and return a value indicating whether they are 
        /// equal.
        /// </summary>
        /// <param name="leftRange">Left-hand side range.</param>
        /// <param name="rightRange">Right-hand side range.</param>
        /// <returns>A value indicating whether the ranges are equal.</returns>
        public static bool operator ==(Range<T> leftRange, Range<T> rightRange)
        {
            if (leftRange.IsEmpty)
            {
                return rightRange.IsEmpty;
            }
            if (rightRange.IsEmpty)
            {
                return leftRange.IsEmpty;
            }

            return leftRange.Minimum.Equals(rightRange.Minimum) && leftRange.Maximum.Equals(rightRange.Maximum);
        }

        /// <summary>
        /// Compare two ranges and return a value indicating whether they are 
        /// not equal.
        /// </summary>
        /// <param name="leftRange">Left-hand side range.</param>
        /// <param name="rightRange">Right-hand side range.</param>
        /// <returns>A value indicating whether the ranges are not equal.
        /// </returns>
        public static bool operator !=(Range<T> leftRange, Range<T> rightRange)
        {
            return !(leftRange == rightRange);
        }

        /// <summary>
        /// Adds a range to the current range.
        /// </summary>
        /// <param name="range">A range to add to the current range.</param>
        /// <returns>A new range that encompasses the instance range and the
        /// range parameter.</returns>
        public Range<T> Add(Range<T> range)
        {
            if (this.IsEmpty)
            {
                return range;
            }
            else if (range.IsEmpty)
            {
                return this;
            }
            T minimum = this.Minimum.CompareTo(range.Minimum) == -1 ? this.Minimum : range.Minimum;
            T maximum = this.Maximum.CompareTo(range.Maximum) == 1 ? this.Maximum : range.Maximum;
            return new Range<T>(minimum, maximum);
        }

        /// <summary>
        /// Compares the range to another range.
        /// </summary>
        /// <param name="range">A different range.</param>
        /// <returns>A value indicating whether the ranges are equal.</returns>
        public bool Equals(Range<T> range)
        {
            return this == range;
        }

        /// <summary>
        /// Compares the range to an object.
        /// </summary>
        /// <param name="obj">Another object.</param>
        /// <returns>A value indicating whether the other object is a range,
        /// and if so, whether that range is equal to the instance range.
        /// </returns>
        public override bool Equals(object obj)
        {
            Range<T> range = (Range<T>)obj;
            if (range == null)
            {
                return false;
            }
            return this == range;
        }

        /// <summary>
        /// Returns a value indicating whether a value is within a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Whether the value is within the range.</returns>
        public bool Contains(T value)
        {
            return Minimum.CompareTo(value) < 0 && Maximum.CompareTo(value) > 0;
        }

        /// <summary>
        /// Returns a new range that contains the value.
        /// </summary>
        /// <param name="value">The value to extend the range to.</param>
        /// <returns>The range which contains the value.</returns>
        public Range<T> ExtendTo(T value)
        {
            if (IsEmpty)
            {
                return new Range<T>(value, value);
            }

            if (Minimum.CompareTo(value) > 0)
            {
                return new Range<T>(value, Maximum);
            }
            else if (Maximum.CompareTo(value) < 0)
            {
                return new Range<T>(Minimum, value);
            }

            return this;
        }

        /// <summary>
        /// Computes a hash code value.
        /// </summary>
        /// <returns>A hash code value.</returns>
        public override int GetHashCode()
        {
            if (IsEmpty)
            {
                return 0;
            }

            int num = 0x5374e861;
            num = (-1521134295 * num) + EqualityComparer<T>.Default.GetHashCode(Minimum);
            return ((-1521134295 * num) + EqualityComparer<T>.Default.GetHashCode(Maximum));
        }
    }
}
