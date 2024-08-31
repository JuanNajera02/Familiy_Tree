using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Core;
using Entities.Entities;
using Microsoft.EntityFrameworkCore.Storage;



namespace Data
{
    public class UnitOfWork : IDisposable
    {
        public Context dbcontext;
        private IDbContextTransaction? _currentTransaction;

        private GenericRepository<Persons>? personsRepository;
        private GenericRepository<Relationships>? relationshipsRepository;


        public UnitOfWork(Context context)
        {
            dbcontext = context;
        }

        public void Save()
        {
            dbcontext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await dbcontext.SaveChangesAsync();
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    dbcontext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            if (_currentTransaction == null)
            {
                _currentTransaction = dbcontext.Database.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            try
            {
                _currentTransaction?.Commit();
            }
            catch
            {
                // Manejar cualquier excepción aquí
                RollbackTransaction();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public GenericRepository<Persons> PersonsRepository
        {
            get
            {
                if (personsRepository == null)
                {
                    personsRepository = new GenericRepository<Persons>(dbcontext);
                }
                return personsRepository;
            }
        }

        public GenericRepository<Relationships> RelationshipsRepository
        {
            get
            {
                if (relationshipsRepository == null)
                {
                    relationshipsRepository = new GenericRepository<Relationships>(dbcontext);
                }
                return relationshipsRepository;
            }
        }



    }
}
