﻿using MSS_DEMO.Core.Components;
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
        void Save();
    }
}