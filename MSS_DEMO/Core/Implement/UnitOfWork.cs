using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSS_DEMO.Core.Components;
using MSS_DEMO.Core.Implement;
using MSS_DEMO.Models;

namespace MSS_DEMO.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private MSSEntities context = new MSSEntities();
        private StudentRepository StudentRepository;
        private CampusRepository CampusRepository;
        private SubjectRepository SubjectRepository;
        private SpecificationsRepository SpecificationsRepository;
        private CoursesRepository CoursesRepository;
        private StudentSpecificationLogRepository SpecificationLogRepository;
        private StudentCoursesLogRepository CoursesLogRepository;
        private UserRepository UserRepository;
        private SemestersRepository SemestersRepository;
        private SubjectStudentRepository SubjectStudentRepository;
        private CoursesDeadlineRepository CoursesDeadlineRepository;


        public StudentRepository Students => StudentRepository ?? (StudentRepository = new StudentRepository(context));
        public CoursesDeadlineRepository DeadLine => CoursesDeadlineRepository ?? (CoursesDeadlineRepository = new CoursesDeadlineRepository(context));
        public UserRepository User => UserRepository ?? (UserRepository = new UserRepository(context));
        public CampusRepository Campus => CampusRepository ?? (CampusRepository = new CampusRepository(context));
        public SubjectRepository Subject => SubjectRepository ?? (SubjectRepository = new SubjectRepository(context));
        public SpecificationsRepository Specifications => SpecificationsRepository ?? (SpecificationsRepository = new SpecificationsRepository(context));
        public CoursesRepository Courses => CoursesRepository ?? (CoursesRepository = new CoursesRepository(context));
        public StudentSpecificationLogRepository SpecificationsLog => SpecificationLogRepository ?? (SpecificationLogRepository = new StudentSpecificationLogRepository(context));
        public StudentCoursesLogRepository CoursesLog => CoursesLogRepository ?? (CoursesLogRepository = new StudentCoursesLogRepository(context));
        public SemestersRepository Semesters => SemestersRepository ?? (SemestersRepository = new SemestersRepository(context));
        public SubjectStudentRepository SubjectStudent => SubjectStudentRepository ?? (SubjectStudentRepository = new SubjectStudentRepository(context));

        public bool Save()
        {
            bool returnValue = true;
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    returnValue = false;
                    dbContextTransaction.Rollback();
                }
            }
            return returnValue;
        }
    }
}