// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace PhoneToolkitSample.Data
{
    public class Person
    {
        public int ID { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public string Email { get; private set; }
        public string Mobile { get; private set; }
        public string Home { get; private set; }
        public string ImageUrl { get; private set; }

        public static Person GetRandomPerson(int id)
        {
            Person person = new Person();
            
            person.ID = id;
            person.FirstName = RandomPeople.GetRandomFirstName();
            person.LastName = RandomPeople.GetRandomLastName();
            person.Email = person.FirstName + "@email+more.com";
            person.Mobile = RandomPeople.GetRandomPhoneNumber();
            person.Home = RandomPeople.GetRandomPhoneNumber();
            person.ImageUrl = "/Images/Person.jpg";
            return person;
        }

        public static string GetFirstNameKey(Person person)
        {
            char key = char.ToLower(person.FirstName[0]);

            if (key < 'a' || key > 'z')
            {
                key = '#';
            }

            return key.ToString();
        }

        public static int CompareByFirstName(object obj1, object obj2)
        {
            Person p1 = (Person) obj1;
            Person p2 = (Person) obj2;

            int result = p1.FirstName.CompareTo(p2.FirstName);
            if (result == 0)
            {
                result = p1.LastName.CompareTo(p2.LastName);
            }

            return result;
        }
    }
}
