
namespace Contacts.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Initials {
            get
            {
                if (string.IsNullOrEmpty(LastName))
                    return Name[0].ToString();
                return Name[0] + LastName[0].ToString();
            }

            set { Initials = value; }
        }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}