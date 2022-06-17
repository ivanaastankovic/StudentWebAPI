using StudentsWebAPI.Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentsAPI.Helpers
{
    public class JsonHelper
    {
       
        public static bool SaveToJson(ICollection<Student> students)
        {
            try
            {
                JsonSerializerOptions _options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                var jsonString = JsonSerializer.Serialize(students, _options);

                File.WriteAllText($"output/{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.json", jsonString);

                return true;
            }
            catch (Exception)
            {
                // should log the exc message ... 
                return false;
            }
        }
    }
}
