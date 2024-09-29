using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BackendNETAPI.Model
{
    public class SubModule
    {

        [Key] // This attribute specifies that this property is the primary key
        public int Id { get; set; }
        public string SubModuleName { get; set; } = string.Empty;
        public int ModuleId { get; set; }
        public string RecordStatus { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;

    }
}
