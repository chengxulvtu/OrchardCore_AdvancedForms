using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.Models
{
    public class BooleanPart
    {
        public BooleanPart(bool value)
        {
            Value = value;
        }

        public bool Value { get; set; }
    }
}
