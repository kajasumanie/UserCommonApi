using JSAApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSAApplicationCore.Interfaces.Services
{
    public interface IUserService
    {
        User Authenticate(string userName, string password);
    }
}
