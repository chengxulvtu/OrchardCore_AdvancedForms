using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.Models
{
    public class ContactUs
    {
        public TextPart Name { get; set; }
        public TextPart Email { get; set; }
        public TextPart PhoneNumber { get; set; }
        public TextPart Message { get; set; }

        public ContactUs(string name, string email, string phoneNumber, string message)
        {
            Name = new TextPart(name);
            Email = new TextPart(email);
            PhoneNumber = new TextPart(phoneNumber);
            Message = new TextPart(message);
        }
    }
}
