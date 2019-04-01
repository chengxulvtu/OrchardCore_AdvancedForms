using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms.Models
{
    public class EditorPart
    {
        public EditorPart(string html)
        {
            Html = html;
        }

        public string Html { get; set; }
    }

    public class JSonEditorPart
    {
        public JSonEditorPart(string html)
        {
            Html = !string.IsNullOrEmpty(html) ? JObject.Parse(html) : null;
        }
        public JObject Html { get; set; }
    }
}
