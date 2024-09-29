using System.ComponentModel.DataAnnotations;

namespace BackendNETAPI.Model
{
    public class AccessRightSubModule
    {
        [Key] // This attribute specifies that this property is the primary key
        public int Id { get; set; }

        public int AccessRightId { get; set; }

        public int ModuleId { get; set; }
        public int SubModuleId { get; set; }

        string RecordStatus { get; set; } = "Active";

    }

}
