//-----------------------------------------------------------------------
// <copyright company="Microsoft">
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
    public partial class Order : INotifyPropertyChanged
    {
        private string _details;
        private DateTime _date;

        [Required]
        [StringLength(10, ErrorMessage = "String must be less than 10 characters.")]
        [Display(Name = "OrderDetails", Description = "Order info")]
        public string Details
        {
            get { return _details; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "Details";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _details = value;
                Notify("Details");
            }
        }

        [Range(typeof(DateTime), "2008, 1, 1", "2010, 1, 1", ErrorMessage = "Date out of range!")]
        [Display(Name = "OrderDate", Description = "Order date")]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "Date";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _date = value;
                Notify("Date");
            }
        }

        #region Methods

        protected void Notify(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
