using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.Models
{
    public class CommentPart
    {

        public CommentPart(string comment)
        {
            Comment = new EditorPart(comment);
        }

        public EditorPart Comment { get; set; }
    }
}
