// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;

namespace PhoneToolkitSample.Data
{
    public class AllPeople : IEnumerable<Person>
    {
        private static Dictionary<int, Person> _personLookup;
        private static AllPeople _instance;

        public static AllPeople Current
        {
            get
            {
                return _instance ?? (_instance = new AllPeople());
            }
        }

        public Person this[int index]
        {
            get
            {
                Person person;
                _personLookup.TryGetValue(index, out person);
                return person;
            }
        }

        #region IEnumerable<Person> Members

        public IEnumerator<Person> GetEnumerator()
        {
            EnsureData();
            return _personLookup.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            EnsureData();
            return _personLookup.Values.GetEnumerator();
        }

        #endregion

        private void EnsureData()
        {
            if (_personLookup == null)
            {
                _personLookup = new Dictionary<int, Person>();
                for (int n = 0; n < 100; ++n)
                {
                    Person person = Person.GetRandomPerson(n);
                    _personLookup[n] = person;
                }
            }
        }

    }
}
