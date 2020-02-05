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
        Student GetStudent(String[] row);
        Class_Student GetClassStudent(String[] row);
        Subject_Student GetSubjectStudent(String[] row);
        Student_Specification_Log GetStudentSpec(String[] row);
        Student_Course_Log GetStudentCourse(String[] row);
        String ChangeBoolean(string name);
    }
}
