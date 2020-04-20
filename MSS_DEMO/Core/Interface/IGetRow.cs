using MSS_DEMO.Core.Components;
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
        Student GetStudent(List<string> row, string semester, string campus);
        Subject_Student GetSubjectStudent(List<string> row, string semester);
        Student_Specification_Log GetStudentSpec(List<string> row, int userID, string dateImport, List<Specification> listIdSubject, string semesterID);
        Student_Course_Log GetStudentCourse(List<string> row, int userID, string dateImport, List<Course_Spec_Sub> course_Spec_Subs, string semesterID);
    }
}
