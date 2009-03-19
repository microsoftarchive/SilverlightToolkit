// (c) Copyright Microsoft Corporation. 
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved. 

using System.ComponentModel;

namespace System.Windows.Controls.Data.Test.DataClasses
{
    public class Order : IEditableObject
    {
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
        public string CountryRegion { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        #region IEditableObject Members

        private Order backup;
        public void BeginEdit()
        {
            backup = new Order(this.ID, this.CountryRegion, this.StateProvince, this.City, this.Phone);
        }

        public void CancelEdit()
        {
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
            backup = null;
        }

        #endregion
    }
}
