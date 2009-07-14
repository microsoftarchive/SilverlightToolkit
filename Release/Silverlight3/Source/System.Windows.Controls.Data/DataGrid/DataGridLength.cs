// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls
{
    // These aren't flags
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    internal enum DataGridLengthUnitType
    {
        Auto = 0,
        Pixel = 1,
        SizeToCells = 2,
        SizeToHeader = 3,
        // 
    }

    /// <summary>
    /// Represents the lengths of elements within the <see cref="T:System.Windows.Controls.DataGrid" /> control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TypeConverter(typeof(DataGridLengthConverter))]
    public struct DataGridLength : IEquatable<DataGridLength>
    {
        #region Data

        private double _unitValue;      //  unit value storage
        private DataGridLengthUnitType _unitType; //  unit type storage

        //  static instances of value invariant DataGridLengths
        private static readonly DataGridLength _auto = new DataGridLength(DATAGRIDLENGTH_DefaultValue, DataGridLengthUnitType.Auto);
        private static readonly DataGridLength _sizeToCells = new DataGridLength(DATAGRIDLENGTH_DefaultValue, DataGridLengthUnitType.SizeToCells);
        private static readonly DataGridLength _sizeToHeader = new DataGridLength(DATAGRIDLENGTH_DefaultValue, DataGridLengthUnitType.SizeToHeader);

        // WPF uses 1.0 as the default value as well
        internal const double DATAGRIDLENGTH_DefaultValue = 1.0;

        #endregion Data

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridLength" /> class. 
        /// </summary>
        /// <param name="value"></param>
        public DataGridLength(double value)
            : this(value, DataGridLengthUnitType.Pixel)
        {
        }

        private DataGridLength(double value, DataGridLengthUnitType type)
        {
            if (double.IsNaN(value))
            {
                throw DataGridError.DataGrid.ValueCannotBeSetToNAN("value");
            }
            if (double.IsInfinity(value))
            {
                throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("value");
            }
            if (value < 0)
            {
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "value", 0);
            }

            if (type != DataGridLengthUnitType.Auto &&
                type != DataGridLengthUnitType.SizeToCells &&
                type != DataGridLengthUnitType.SizeToHeader &&
                // 
                type != DataGridLengthUnitType.Pixel)
            {
                throw DataGridError.DataGridLength.InvalidUnitType("type");
            }

            _unitValue = (type == DataGridLengthUnitType.Auto) ? DATAGRIDLENGTH_DefaultValue : value;
            _unitType = type;
        }

        #region Properties

        /// <summary>
        /// Gets a <see cref="T:System.Windows.Controls.DataGridLength" /> structure that represents the standard automatic sizing mode.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.DataGridLength" /> structure that represents the standard automatic sizing mode.
        /// </returns>
        public static DataGridLength Auto
        {
            get
            {
                return _auto;
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if this DataGridLength instance holds 
        ///     an absolute (pixel) value.
        /// </summary>
        public bool IsAbsolute
        {
            get
            {
                return _unitType == DataGridLengthUnitType.Pixel;
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if this DataGridLength instance is 
        ///     automatic (not specified).
        /// </summary>
        public bool IsAuto
        {
            get
            {
                return _unitType == DataGridLengthUnitType.Auto;
            }
        }

        // 












        /// <summary>
        ///     Returns <c>true</c> if this instance is to size to the cells of a column or row.
        /// </summary>
        public bool IsSizeToCells
        {
            get
            {
                return _unitType == DataGridLengthUnitType.SizeToCells;
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if this instance is to size to the header of a column or row.
        /// </summary>
        public bool IsSizeToHeader
        {
            get
            {
                return _unitType == DataGridLengthUnitType.SizeToHeader;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Windows.Controls.DataGridLength" /> structure that represents the cell-based automatic sizing mode.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.DataGridLength" /> structure that represents the cell-based automatic sizing mode.
        /// </returns>
        public static DataGridLength SizeToCells
        {
            get
            {
                return _sizeToCells;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Windows.Controls.DataGridLength" /> structure that represents the header-based automatic sizing mode.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.DataGridLength" /> structure that represents the header-based automatic sizing mode.
        /// </returns>
        public static DataGridLength SizeToHeader
        {
            get
            {
                return _sizeToHeader;
            }
        }

        internal DataGridLengthUnitType UnitType
        {
            get
            {
                return _unitType;
            }
        }

        /// <summary>
        /// Gets the absolute value of the <see cref="T:System.Windows.Controls.DataGridLength" /> in pixels.
        /// </summary>
        /// <returns>
        /// The absolute value of the <see cref="T:System.Windows.Controls.DataGridLength" /> in pixels.
        /// </returns>
        public double Value
        {
            get
            {
                return _unitValue;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Overloaded operator, compares 2 GridLength's.
        /// </summary>
        /// <param name="gl1">first DataGridLength to compare.</param>
        /// <param name="gl2">second DataGridLength to compare.</param>
        /// <returns>true if specified DataGridLength have same value 
        /// and unit type.</returns>
        public static bool operator ==(DataGridLength gl1, DataGridLength gl2)
        {
            return (gl1.UnitType == gl2.UnitType
                    && gl1.Value == gl2.Value);
        }

        /// <summary>
        /// Overloaded operator, compares 2 GridLength's.
        /// </summary>
        /// <param name="gl1">first DataGridLength to compare.</param>
        /// <param name="gl2">second DataGridLength to compare.</param>
        /// <returns>true if specified DataGridLength have either different value or 
        /// unit type.</returns>
        public static bool operator !=(DataGridLength gl1, DataGridLength gl2)
        {
            return (gl1.UnitType != gl2.UnitType
                    || gl1.Value != gl2.Value);
        }

        /// <summary>
        /// Compares this instance of DataGridLength with another instance.
        /// </summary>
        /// <param name="other">DataGridLength length instance to compare.</param>
        /// <returns><c>true</c> if this DataGridLength instance has the same value 
        /// and unit type as gridLength.</returns>
        public bool Equals(DataGridLength other)
        {
            return (this == other);
        }

        /// <summary>
        /// Compares this instance of GridLength with another object.
        /// </summary>
        /// <param name="obj">Reference to an object for comparison.</param>
        /// <returns><c>true</c> if this DataGridLength instance has the same value 
        /// and unit type as oCompare.</returns>
        public override bool Equals(object obj)
        {
            DataGridLength? dataGridLength = obj as DataGridLength?;
            if (dataGridLength.HasValue)
            {
                return (this == dataGridLength);
            }
            return false;
        }

        /// <summary>
        /// Returns a unique hash code for this DataGridLength
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return ((int)_unitValue + (int)_unitType);
        }

        #endregion Methods
    }
}
