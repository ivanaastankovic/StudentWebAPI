
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudentsWebAPI.Domain
{
    public class Parent:Person
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public override string? UserId { get ; set ; }
        
    }
}
