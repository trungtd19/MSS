using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSS_DEMO.Core.Components;
using MSS_DEMO.Models;

namespace MSS_DEMO.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private MSSEntities context = new MSSEntities();
        private StudentRepository studentRepository;
        private CampusRepository campusRepository;
        public StudentRepository Students
        {
            get
            {
                return studentRepository ?? (studentRepository = new StudentRepository(context));
            }
        }
        public CampusRepository Campus
        {
            get
            {
                return campusRepository ?? (campusRepository = new CampusRepository(context));
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }   
    }
}