//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="BasicDataClass.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// A basic data class for testing the <see cref="DataForm"/>.
    /// </summary>
    public class BasicDataClass : INotifyPropertyChanged
    {
        /// <summary>
        /// Private accessor to BoolProperty.
        /// </summary>
        private bool boolProperty;

        /// <summary>
        /// Private accessor to DateTimeProperty.
        /// </summary>
        private DateTime dateTimeProperty;

        /// <summary>
        /// Private accessor to IntProperty.
        /// </summary>
        private int intProperty;

        /// <summary>
        /// Private accessor to StringProperty.
        /// </summary>
        private string stringProperty;

        /// <summary>
        /// Gets or sets a value indicating whether or not the bool property is true.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool", Justification = "This property is only used in unit tests.")]
        [Display(Description = "Bool Property Description", Name = "Bool Property")]
        public bool BoolProperty
        {
            get
            {
                return this.boolProperty;
            }

            set
            {
                if ((this.boolProperty != value))
                {
                    ValidateProperty("BoolProperty", value);
                    this.boolProperty = value;
                    this.RaiseDataMemberChanged("BoolProperty");
                }
            }
        }

        /// <summary>
        /// Gets or sets a DateTime value.
        /// </summary>
        [Display(Description = "Date Time Property Description", Name = "Date Time Property")]
        public DateTime DateTimeProperty
        {
            get
            {
                return this.dateTimeProperty;
            }

            set
            {
                if ((this.dateTimeProperty != value))
                {
                    ValidateProperty("DateTimeProperty", value);
                    this.dateTimeProperty = value;
                    this.RaiseDataMemberChanged("DateTimeProperty");
                }
            }
        }

        /// <summary>
        /// Gets or sets an int.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "This property is only used in unit tests.")]
        [Display(Description = "Int Property Description", Name = "Int Property")]
        public int IntProperty
        {
            get
            {
                return this.intProperty;
            }

            set
            {
                if ((this.intProperty != value))
                {
                    ValidateProperty("IntProperty", value);
                    this.intProperty = value;
                    this.RaiseDataMemberChanged("IntProperty");
                }
            }
        }

        /// <summary>
        /// Gets or sets a string.
        /// </summary>
        [Display(Description = "String Property Description", Name = "String Property")]
        public string StringProperty
        {
            get
            {
                return this.stringProperty;
            }

            set
            {
                if ((this.stringProperty != value))
                {
                    ValidateProperty("StringProperty", value);
                    this.stringProperty = value;
                    this.RaiseDataMemberChanged("StringProperty");
                }
            }
        }

        /// <summary>
        /// Flags the fact that a data member has changed.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        private void RaiseDataMemberChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Validates a property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to validate against.</param>
        private void ValidateProperty(string propertyName, object value)
        {
            ValidationContext context = new ValidationContext(this, null, null);
            context.MemberName = propertyName;
            Validator.ValidateProperty(value, context);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
