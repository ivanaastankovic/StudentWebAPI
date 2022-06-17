using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudentsWebAPI.Domain
{
    public class Student:Person
    {
        public string StudentId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Address { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<Parent>? Parents { get; set; }
        public Parent Parent { get; set; }
        public string? Note { get; set; }
    }
}
