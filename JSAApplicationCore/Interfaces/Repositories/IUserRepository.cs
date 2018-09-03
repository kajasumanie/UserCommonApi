using JSAApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using JSAApplicationCore.Interfaces.Repositories;
namespace JSAApplicationCore.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>, IAsyncRepository<User>
    {
        User GetByName(string name);
    }
}
