using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using System.Collections.Generic;

namespace StudentApi.Hateoas
{
    public interface ILinkGenerator
    {
        StudentResource GenerateStudentResource(Student student, IUrlHelper urlHelper);
        IEnumerable<Link> GenerateErrorLinks(string errorCode);
        IEnumerable<Link> GenerateStudentLinks(int id, IUrlHelper urlHelper);
    }
}