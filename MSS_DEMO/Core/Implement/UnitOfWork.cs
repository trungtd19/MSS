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
        public StudentRepository Students
        {
            get
            {
                return StudentRepository ?? (StudentRepository = new StudentRepository(context));
            }
        }
        public CampusRepository Campus
        {
            get
            {
                return CampusRepository ?? (CampusRepository = new CampusRepository(context));
            }
        }
        public SubjectRepository Subject
        {
            get
            {
                return SubjectRepository ?? (SubjectRepository = new SubjectRepository(context));
            }
        }
        public SpecificationsRepository Specifications
        {
            get
            {
                return SpecificationsRepository ?? (SpecificationsRepository = new SpecificationsRepository(context));
            }
        }
        public CoursesRepository Courses
        {
            get
            {
                return CoursesRepository ?? (CoursesRepository = new CoursesRepository(context));
            }
        }

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
                catch (Exception)
                {                   
                    returnValue = false;
                    dbContextTransaction.Rollback();
                }
            }
            return returnValue;
        }   
    }
}