using JSAApplicationCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using JSAApplicationCore.Entities;
using System.Linq;

namespace JSAInfrastructure.Data
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(JSAContext dbContext) : base(dbContext)
        {
        }

        public User GetByName(string name)
        {
            return _dbContext.Users
                   .Where(o => o.UserName == name)
                   .FirstOrDefault();
        }

    }
}
