using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS_DEMO.Core.Interface
{
    public interface IGetRow
    {
        Student GetStudent(List<string> row);
        Class_Student GetClassStudent(List<string> row);
        Subject_Student GetSubjectStudent(List<string> row);
        Student_Specification_Log GetStudentSpec(List<string> row, int userID, string dateImport);
        Student_Course_Log GetStudentCourse(List<string> row, int userID, string dateImport);
        String ChangeBoolean(string name);
    }
}
