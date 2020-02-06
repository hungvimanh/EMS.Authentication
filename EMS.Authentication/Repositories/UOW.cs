using EMS.Authentication.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF.Authentication.Commons;
using Z.EntityFramework.Extensions;

namespace TF.Authentication.Repositories
{
    public interface IUOW : IServiceScoped
    {
        Task Begin();
        Task Commit();
        Task Rollback();
        IUserRepository UserRepository { get; }
    }
    public class UOW : IUOW
    {
        private EMSContext context;
        public IUserRepository UserRepository { get; private set; }

        public UOW(EMSContext context, ICurrentContext currentContext)
        {
            this.context = context;
            UserRepository = new UserRepository(context);
            EntityFrameworkManager.ContextFactory = DbContext => context;
        }

        public async Task Begin()
        {
            await context.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            context.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            context.Database.RollbackTransaction();
            return Task.CompletedTask;
        }
    }
}
