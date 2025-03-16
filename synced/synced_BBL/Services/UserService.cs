using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL.Entities;
using synced_DAL.Interfaces;
using synced_DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using velocitaApi.Mappers;

namespace synced_BBL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    

        public int LoginUser(LoginDto login)
        {
            return _userRepository.Login(login.email, login.password);
        }

        // fix return type for now this way
        public bool RegisterUser(UserDto userDto)
        {
            var mappedUser = Mapper.MapCreate<User>(userDto);

            return _userRepository.Register(mappedUser);
        }
    }
}
