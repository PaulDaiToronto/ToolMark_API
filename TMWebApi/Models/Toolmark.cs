using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TMWebApi.Models
{
    [Table("Toolmark")]
    public class Toolmark
    {
        [Key]
        public int ToolmarkID { get; set; }
        public string CaseNumber { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public DateTime DateOfCollected { get; set; }
        public string ImageFileName { get; set; }
        public string Note { get; set; }
    }
}
