using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using System.Collections.Generic;

namespace StudentApi.Hateoas
{
    public class HateoasLinkGenerator : ILinkGenerator
    {
        public StudentResource GenerateStudentResource(Student student, IUrlHelper urlHelper)
        {
            var resource = new StudentResource
            {
                ID = student.ID,
                Name = student.Name,
                Phone = student.Phone
            };

            // Добавление HATEOAS ссылок
            resource.Links.Add(new Link(urlHelper.Link("GetStudent", new { id = student.ID }), "self", "GET"));
            resource.Links.Add(new Link(urlHelper.Link("UpdateStudent", new { id = student.ID }), "update_student", "PUT"));
            resource.Links.Add(new Link(urlHelper.Link("DeleteStudent", new { id = student.ID }), "delete_student", "DELETE"));
            resource.Links.Add(new Link(urlHelper.Link("GetStudents", null), "collection", "GET"));

            return resource;
        }

        public IEnumerable<Link> GenerateErrorLinks(string errorCode)
        {
            // Пример: ссылки на документацию или главную страницу API
            return new List<Link>
            {
                new Link($"/docs/errors/{errorCode}", "error_details", "GET"),
                new Link("/api/students", "students_collection", "GET")
            };
        }

        public IEnumerable<Link> GenerateStudentLinks(int id, IUrlHelper urlHelper)
        {
            return new List<Link>
            {
                new Link(urlHelper.Link("GetStudent", new { id }), "self", "GET"),
                new Link(urlHelper.Link("UpdateStudent", new { id }), "update_student", "PUT"),
                new Link(urlHelper.Link("DeleteStudent", new { id }), "delete_student", "DELETE"),
                new Link(urlHelper.Link("GetStudents", null), "collection", "GET")
            };
        }
    }
}