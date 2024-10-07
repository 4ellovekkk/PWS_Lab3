using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Models;
using StudentApi.Hateoas;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace StudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext _context;
        private readonly ILinkGenerator _linkGenerator;

        public StudentsController(StudentContext context, ILinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // GET: api/Students
        [HttpGet(Name = "GetStudents")]
        public async Task<ActionResult<IEnumerable<StudentResource>>> GetStudents(
            [FromQuery] int? limit,
            [FromQuery] string? sort,
            [FromQuery] int? offset,
            [FromQuery] int? minid,
            [FromQuery] int? maxid,
            [FromQuery] string? like,
            [FromQuery] string? columns,
            [FromQuery] string? globalike)
        {
            IQueryable<Student> query = _context.Students;

            // Фильтрация по minid и maxid
            if (minid.HasValue)
                query = query.Where(s => s.ID >= minid.Value);
            if (maxid.HasValue)
                query = query.Where(s => s.ID <= maxid.Value);

            // Фильтрация по шаблону like в имени
            if (!string.IsNullOrEmpty(like))
                query = query.Where(s => s.Name.Contains(like));

            // Глобальный поиск
            if (!string.IsNullOrEmpty(globalike))
                query = query.Where(s => s.ID.ToString().Contains(globalike) ||
                                         s.Name.Contains(globalike) ||
                                         s.Phone.Contains(globalike));

            // Сортировка
            if (!string.IsNullOrEmpty(sort))
                query = query.OrderBy($"Name {sort}");
            else
                query = query.OrderBy(s => s.ID);

            // Пропуск
            if (offset.HasValue)
                query = query.Skip(offset.Value);

            // Ограничение количества
            if (limit.HasValue)
                query = query.Take(limit.Value);

            // Выбор колонок
            if (!string.IsNullOrEmpty(columns))
            {
                var columnList = columns.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                                        .Select(c => c.Trim())
                                        .ToList();

                // Используем динамическую LINQ для выбора колонок
                var selectedStudents = await query.Select("new (" + string.Join(",", columnList) + ")").ToDynamicListAsync();
                return Ok(selectedStudents);
            }

            var students = await query.ToListAsync();

            // Добавление HATEOAS ссылок
            var studentsWithLinks = students.Select(s => _linkGenerator.GenerateStudentResource(s, Url));

            return Ok(studentsWithLinks);
        }

        // GET: api/Students/5
        [HttpGet("{id}", Name = "GetStudent")]
        public async Task<ActionResult<StudentResource>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NotFound",
                    Message = $"Student with ID {id} not found.",
                    Links = new List<Link>(_linkGenerator.GenerateErrorLinks("NotFound"))
                });
            }

            // Добавление HATEOAS ссылок
            var studentWithLinks = _linkGenerator.GenerateStudentResource(student, Url);
            return Ok(studentWithLinks);
        }

        // POST: api/Students
        [HttpPost(Name = "CreateStudent")]
        public async Task<ActionResult<StudentResource>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var studentWithLinks = _linkGenerator.GenerateStudentResource(student, Url);

            return CreatedAtRoute("GetStudent", new { id = student.ID }, studentWithLinks);
        }

        // PUT: api/Students/5
        [HttpPut("{id}", Name = "UpdateStudent")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.ID)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = "BadRequest",
                    Message = "ID in URL does not match ID in body.",
                    Links = new List<Link>(_linkGenerator.GenerateErrorLinks("BadRequest"))
                });
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound(new ErrorResponse
                    {
                        Code = "NotFound",
                        Message = $"Student with ID {id} not found.",
                        Links = new List<Link>(_linkGenerator.GenerateErrorLinks("NotFound"))
                    });
                }
                else
                {
                    throw;
                }
            }

            var studentWithLinks = _linkGenerator.GenerateStudentResource(student, Url);
            return Ok(studentWithLinks);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}", Name = "DeleteStudent")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NotFound",
                    Message = $"Student with ID {id} not found.",
                    Links = new List<Link>(_linkGenerator.GenerateErrorLinks("NotFound"))
                });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Student with ID {id} has been deleted.",
                Links = _linkGenerator.GenerateStudentLinks(id, Url)
            });
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
