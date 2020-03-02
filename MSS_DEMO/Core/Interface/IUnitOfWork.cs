﻿using MSS_DEMO.Core.Components;
using MSS_DEMO.Core.Implement;
using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS_DEMO.Repository
{
    public interface IUnitOfWork
    {
        StudentRepository Students { get; }
        CampusRepository Campus { get; }
        CoursesRepository Courses { get; }
        SpecificationsRepository Specifications { get; }
        SubjectRepository Subject { get; }
        StudentSpecificationLogRepository SpecificationsLog { get; }
        StudentCoursesLogRepository CoursesLog { get; }
        UserRepository User { get; }
        SemestersRepository Semesters { get; }
        ClassStudentRepository ClassStudent { get; }
        SubjectStudentRepository SubjectStudent { get; }
        ClassRepository Classes { get; }
        CoursesDeadlineRepository DeadLine { get; }
        bool Save();
    }
}
