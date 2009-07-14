//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace System.Windows.Controls.UnitTests
{
    public partial class Customer : IEditableObject, INotifyPropertyChanged
    {
        private string _name;
        private int _age;
        private DateTime _birthDate;

        public Customer()
        {
            // Test data
            _name = "Scott";
            _email = "scott@contoso.com";
            _birthDate = new DateTime(2008, 2, 21);
            _altEmail = "scott@hotmail.com";
            this.Warnings = new ObservableCollection<string>();
        }
        public Customer(string name, string email, DateTime birthDate)
        {
            // Test data
            _name = name;
            _email = email;
            _birthDate = birthDate;
            _altEmail = "scott@hotmail.com";
            this.Warnings = new ObservableCollection<string>();
        }

        [Required]
        [StringLength(5, ErrorMessage = "String must be less than 5 characters.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "The name field must contain only letters.")]
        [Display(Name = "Name", Description = "This is your first name.")]
        public string Name
        {
            get { return _name; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "Name";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _name = value;
                Notify("Name");
            }
        }

        [Required(ErrorMessage = "The age field is required.")]
        [Range(0, 100, ErrorMessage = "Must be between 0 and 100.")]
        [Display(Name = "Age", Description = "This is your Age.")]
        public int Age
        {
            get { return _age; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "Age";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _age = value;
                Notify("Age");
            }
        }

        [Display(Name = "Birth date", Description = "This is the day you were born.")]
        [Range(typeof(DateTime), "2008, 1, 1", "2009, 1, 1", ErrorMessage = "Date out of range!")]
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "BirthDate";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _birthDate = value;
                Notify("BirthDate");
            }
        }

        private string _email;
        [Required]
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w][^_]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,4})$", ErrorMessage = "Invalid email address")]
        [Display(Name = "Email", Description = "Your email address")]
        public string Email
        {
            get { return _email; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "Email";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _email = value;
                Notify("Email");
            }

        }

        private string _altEmail;
        [RegularExpression(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w][^_]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,4})$", ErrorMessage = "Invalid email address")]
        [Display(Name = "Alternate Email", Description = "Your email address")]
        public string AltEmail
        {
            get { return _altEmail; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "AltEmail";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _altEmail = value;
                Notify("AltEmail");
            }

        }

        private string _securityQuestion;
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Must contain only letters.")]
        [Required(ErrorMessage = "Security Question is required.")]
        [Display(Name = "Security Question", Description = "Type in your security question")]
        public string SecurityQuestion
        {
            get { return _securityQuestion; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "SecurityQuestion";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _securityQuestion = value;
                Notify("SecurityQuestion");
            }
        }

        private string _secretAnswer;
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Must contain only letters.")]
        [Required(ErrorMessage = "Secret Answer is required.")]
        [Display(Name = "Secret Answer")]
        public string SecretAnswer
        {
            get { return _secretAnswer; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "SecretAnswer";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _secretAnswer = value;
                Notify("SecretAnswer");
            }

        }

        private string _password;
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Must contain only letters.")]
        [Required(ErrorMessage = "Password is required.")]
        [Display(Description = "This is your password")]
        public string Password
        {
            get { return _password; }
            set
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = "Password";
                System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, vc);
                _password = value;
                Notify("Password");
            }
        }

        public ObservableCollection<string> Warnings
        {
            get;
            private set;
        }

        [Display(ShortName = "short name")]
        public string OnlyShortName
        {
            get;
            set;
        }

        [Display(Name = "", Description = "This is your EmptyName")]
        public string EmptyName
        {
            get;
            set;
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

        #region IEditableObject Members

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
            _name = "Scott";
            Notify("Name");
            _email = "scott@contoso.com";
            Notify("Email");
            _birthDate = new DateTime(2008, 2, 21);
            Notify("BirthDate");
            _altEmail = "scott@hotmail.com";
            Notify("AltEmail");
        }

        public void EndEdit()
        {
        }

        #endregion
    }
}
