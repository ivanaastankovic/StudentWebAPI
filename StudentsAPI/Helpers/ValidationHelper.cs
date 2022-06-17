namespace StudentsAPI.Helpers
{
    public class ValidationHelper
    {

        public static bool IsNullOrEmpty(string field)
        {
            return field == null || field.Length == 0;
        }
    }
}
