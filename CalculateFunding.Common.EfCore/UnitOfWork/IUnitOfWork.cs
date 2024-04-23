using CalculateFunding.Common.EfCore.GenericRepository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.EfCore.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext context { get; set; }
        void Commit();
        GenericRepository<T> GenericRepository<T>() where T : class;
        Task CommitAsync();
        void Dispose();
    }
}

