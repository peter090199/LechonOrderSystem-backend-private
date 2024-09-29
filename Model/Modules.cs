using System.ComponentModel.DataAnnotations;

namespace BackendNETAPI.Model
{
    public class Modules
    {
        [Key]
        public int Id { get; set; }
        public string ModuleName { get; set; } = "";
        public string Application { get; set; } = "";
        public string RecordStatus { get; set; } = "";
         
    }
}
