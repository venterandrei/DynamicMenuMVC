using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationMenu.Models
{
    public class SiteApplication
    {

        [Key]
        public int ApplicationID { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int TopicOrder { get; set; }
        public int TopicIndent { get; set; }

        public string Title { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }

        public int HeaderID { get; set; }
        public string ExtraCSS { get; set; }
        public string HRColour { get; set; }

        public string ShadowColour { get; set; }
        public bool RequiresAuthentication { get; set; } = false;
        public DateTime CreationDate { get; set; }

        public DateTime ModifiedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int MenuWidth { get; set; }

        public int SchemeID { get; set; }
        public int MenuIndex { get; set; }
        public bool ShowInLeftMenu { get; set; } = false;

        public bool IsTab { get; set; } = false;
        public bool IsContainer { get; set; } = false;
        public bool IsClickable { get; set; } = true;

        public int TemplateID { get; set; }
        public string AppURL { get; set; }
        public int SiteID { get; set; }


        public string ASPXPage { get; set; }
        public bool ASPXIDReq { get; set; } = false;
        public string ASPXPath { get; set; }

        public string Keywords { get; set; }
        public string ScriptName { get; set; }
        public bool https { get; set; } = false;
        public bool Locked { get; set; } = false;
    }
}
