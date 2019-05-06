using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.Models
{
    public class CommentPart
    {

        public CommentPart(string comment, string attachement)
        {
            Comment = new EditorPart(comment);
            Attachment = new TextPart(attachement);
        }

        public TextPart Attachment { get; set; }

        public EditorPart Comment { get; set; }
    }
}
