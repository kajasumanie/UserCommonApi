using JSAApplicationCore.Entities;
using JSAApplicationCore.Interfaces.Repositories;
using JSAApplicationCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSAApplicationCore.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Authenticate(string userName, string password)
        {
            var user = _userRepository.GetByName(userName);
            if (user.UserName == userName && user.Password == password)
            {
                return user;
            }
            return null;
        }



    }
}
