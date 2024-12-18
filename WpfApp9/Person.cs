using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp9
{
    public class Person
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public Person()
        {
            FullName = string.Empty;
            Address = string.Empty;
            PhoneNumber = string.Empty;
        }
    }


}
