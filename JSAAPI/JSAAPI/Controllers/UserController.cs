using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JSAAPI.Models;
using JSAAPI.Models.AppSettings;
using JSAApplicationCore.Entities;
using JSAApplicationCore.Interfaces.Repositories;
using JSAApplicationCore.Interfaces.Services;
using JSAApplicationCore.Services;


namespace JSAAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        public UserController(IOptions<AppSettings> appSettings, IUserService userService, IUserRepository userRepository)
        {
            _appSettings = appSettings.Value;
            _userService = userService;
            _userRepository = userRepository;
        }
        // GET: api/User
        [HttpGet]
        public IEnumerable<LoginUserModel> Get()
        {
            try
            {
                var users = _userRepository.ListAll();
                var userList = users.Select(s => new LoginUserModel()
                {
                    UserName = s.UserName,
                    Password = s.Password,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email
                });

                return userList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // GET: api/User/5
        [HttpGet("{id}", Name = "getUserById")]
        public LoginUserModel Get(int id)
        {
            try
            {
                var user = _userRepository.GetById(id);
                if (user != null)
                {
                    var viewModel = new LoginUserModel()
                    {
                        UserName = user.UserName,
                        Password = user.Password,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    };

                    return viewModel;
                }

                throw new Exception("Record not found.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]LoginUserModel loginUserModel)
        {
            try
            {
                if (loginUserModel != null)
                {
                    var user = new User
                    {
                        UserName = loginUserModel.UserName,
                        Password = loginUserModel.Password,
                        FirstName = loginUserModel.FirstName,
                        LastName = loginUserModel.LastName,
                        Email = loginUserModel.Email
                    };

                    _userRepository.Add(user);
                }
            }
            catch (Exception ex)
            {
            }

            return new ObjectResult("OK");
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]LoginUserModel loginUserModel)
        {
            try
            {
                var user = _userRepository.GetById(id);
                if (user != null)
                {
                    user.UserName = loginUserModel.UserName;
                    user.FirstName = loginUserModel.FirstName;
                    user.LastName = loginUserModel.LastName;
                    user.Email = loginUserModel.Email;
                    user.Password = loginUserModel.Password;
                    _userRepository.Update(user);
                }
            }
            catch (Exception ex)
            {
            }

            return new ObjectResult("OK");
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginUserModel loginUserModel)
        {
            var user = _userService.Authenticate(loginUserModel.UserName, loginUserModel.Password);
            //Console.WriteLine(user);
            if (user == null)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, loginUserModel.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //var userClaims = await _userManager.GetRolesAsync(user);
#pragma warning disable IDE0028 // Simplify collection initialization
            List<Claim> claims = new List<Claim>();
#pragma warning restore IDE0028 // Simplify collection initialization
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, loginUserModel.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));
            //claims.AddRange(user.Claims.ToArray());
            //foreach (var x in userClaims)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, x));
            //}

            var jwt = new JwtSecurityToken(
                claims: claims);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);


            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Username = loginUserModel.UserName,
                Token = tokenString
            });

            // Bearer Token 
        }
    }
}