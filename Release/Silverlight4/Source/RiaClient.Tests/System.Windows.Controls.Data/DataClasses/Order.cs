// (c) Copyright Microsoft Corporation. 
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace System.Windows.Controls.Data.Test.DataClasses
{
    public class Order : IEditableObject
    {
        // These are fields instead of properties because we don't want the DataGrid to AutoGenerate these
        public int BeginEditCount;
        public int CancelEditCount;
        public int EndEditCount;

        public Order()
        {
        }

        public Order(int id, string countryRegion, string stateProvince, string city, string phone)
        {
            this.ID = id;
            this.CountryRegion = countryRegion;
            this.StateProvince = stateProvince;
            this.City = city;
            this.Phone = phone;
            this.IsActive = true;
        }

        public int ID { get; set; }
        [Display(Name = "Country/Region")]
        public string CountryRegion { get; set; }
        [Display(Name = "State/Province")]
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        #region IEditableObject Members

        private Order backup;
        public void BeginEdit()
        {
            this.BeginEditCount++;
            backup = new Order(this.ID, this.CountryRegion, this.StateProvince, this.City, this.Phone);
        }

        public void CancelEdit()
        {
            this.CancelEditCount++;
            if (backup != null)
            {
                this.City = backup.City;
                this.CountryRegion = backup.CountryRegion;
                this.ID = backup.ID;
                this.Phone = backup.Phone;
                this.StateProvince = backup.StateProvince;
            }
        }

        public void EndEdit()
        {
            this.EndEditCount++;
            backup = null;
        }

        #endregion
    }
}
