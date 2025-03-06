using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApplicationMenu.Models
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasParent { get; set; } = false;
        public int ParentId { get; set; }
        public int PrevId { get; set; }
        public int NextId { get; set; }
        public int Order { get; set; }
        public string Intent { get; set; }
        public  List<MenuViewModel> Items { get; set; }
        public bool Selected { get; set; } = false;
    }
}
