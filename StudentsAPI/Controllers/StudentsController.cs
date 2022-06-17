using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using StudentsWebAPI.Domain;
using StudentsAPI.Helpers;

namespace StudentsWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {

        [HttpPost("SaveStudentsJson")]
        public async Task<IActionResult> saveStudentsJson([FromBody] List<Student> students)
        {

            if (students == null || students.Count < 1)
            {
                return BadRequest("There must be at least one student in the list.");
            }

            foreach (Student s in students)
            {
                bool invalidFields = ValidationHelper.IsNullOrEmpty(s.UserId)
                    || ValidationHelper.IsNullOrEmpty(s.FirstName)
                    || ValidationHelper.IsNullOrEmpty(s.LastName)
                    || ValidationHelper.IsNullOrEmpty(s.MiddleName)
                    || ValidationHelper.IsNullOrEmpty(s.Phone)
                    || ValidationHelper.IsNullOrEmpty(s.StudentId)
                    || s.Parent == null
                    || ValidationHelper.IsNullOrEmpty(s.Parent.FirstName)
                    || ValidationHelper.IsNullOrEmpty(s.Parent.MiddleName)
                    || ValidationHelper.IsNullOrEmpty(s.Parent.LastName)
                    || ValidationHelper.IsNullOrEmpty(s.Parent.Phone);

                if (invalidFields)
                {
                    return BadRequest($"Students could not be added. Not all required fields were populated!");
                }

            }

            JsonHelper.SaveToJson(students);
            return Ok("Students were successfully added to the system.");
        }


        [HttpPost("SaveStudentsCsv")]
        public async Task<IActionResult> SaveStudentsCsv(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            ICollection<Student> students = ParseFile(file.FileName);

            if (students != null && students.Count > 0)
            {
                JsonHelper.SaveToJson(students);
                return Ok("Students were successfully added to the system.");
            }

            return BadRequest("Students could not be imported. Not all required fields were populated!");
        }

        [NonAction]
        private ICollection<Student> ParseFile(string filename)
        {
            List<Student> students = new List<Student>();

            var path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + filename;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            {

                if (!csv.Read())
                {
                    return null;

                }
                csv.ReadHeader();
                var records = csv.HeaderRecord;

                while (csv.Read())
                {

                    var student = new Student
                    {
                        UserId = csv.GetField<string>(0),
                        FirstName = csv.GetField<string>(1),
                        MiddleName = csv.GetField<string>(2),
                        LastName = csv.GetField<string>(3),
                        Address = csv.GetField<string>(4),
                        StudentId = csv.GetField<string>(5),
                        Phone = csv.GetField<string>(6),
                        // for both type of schools one parent is mandatory.
                        Parents = new List<Parent> {
                            new Parent()
                            {
                                FirstName = csv.GetField<string>(7),
                                LastName = csv.GetField<string>(8),
                                Phone = csv.GetField<string>(9)
                            }
                        }
                    };

                    // parsing public schools data
                    if (records.Count() == 11)
                    {
                        student.Note = csv.GetField<string>(10);
                    }

                    // parsing private schools data
                    else if (records.Count() == 14)
                    {
                        student.Parents.Add(new Parent()
                        {
                            FirstName = csv.GetField<string>(10),
                            LastName = csv.GetField<string>(11),
                            Phone = csv.GetField<string>(12)
                        });
                        student.Note = csv.GetField<string>(13);

                    }
                    else
                    {
                        return null;
                    }
                    // validating all fields
                    bool invalidFields = ValidationHelper.IsNullOrEmpty(student.UserId)
                        || ValidationHelper.IsNullOrEmpty(student.FirstName)
                        || ValidationHelper.IsNullOrEmpty(student.MiddleName)
                        || ValidationHelper.IsNullOrEmpty(student.LastName)
                        || ValidationHelper.IsNullOrEmpty(student.Phone)
                        || ValidationHelper.IsNullOrEmpty(student.StudentId)
                        || ValidationHelper.IsNullOrEmpty(student.Address)
                        || student.Parents == null
                        || student.Parents.Count() == 0
                        || student.Parents.Any(s =>
                                      ValidationHelper.IsNullOrEmpty(s.FirstName)
                                   || ValidationHelper.IsNullOrEmpty(s.LastName)
                                   || ValidationHelper.IsNullOrEmpty(s.Phone));

                    if (invalidFields)
                    {
                        return null;
                    }
                    students.Add(student);
                }
            }
            return students;
        }


    }
}