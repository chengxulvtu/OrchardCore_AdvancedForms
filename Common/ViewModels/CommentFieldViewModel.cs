using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class CommentFieldViewModel
    {
        public LocalizedHtmlString AttachFileHint { get; set; }
        public LocalizedHtmlString AttachRemoveHint { get; set; }
        public LocalizedHtmlString ErrorHint { get; set; }
        public string AttachRemoveID { get; set; }
        public string FileUploadID { get; set; }
        public string AttachmentID { get; set; }
        public HTMLFieldViewModel Editor { get; set; }
        public LocalizedHtmlString EditorHint { get; set; }
        public string ErrorMessageClass { get; set; }
        public bool IsAttachAFURL { get; set; }
        public string AttachmentAFURLId { get; set; }
    }
}
