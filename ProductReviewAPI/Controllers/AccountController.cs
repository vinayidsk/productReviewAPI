using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductReviewAPI.Models;
using ProductReviewAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProductReviewAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IDataRepository<User> _repo;
        private readonly ILogger<CategoriesController> _logger;
        private readonly JwtSettings _jwtSettings;
        public AccountController(IDataRepository<User> repo,JwtSettings jwtSettings, ILogger<CategoriesController> logger)
        {
            _repo = repo;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }
        private async Task<IActionResult> GetAllUsers()
        {
            var users = await _repo.GetAllAsync();
            return Ok(users);
        }
        
        private static Dictionary<int, string> EnumNamedValues<T>() where T : System.Enum
        {
            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));

            foreach (int item in values)
                result.Add(item, Enum.GetName(typeof(T), item)!);
            return result;
        }

        /// <summary>
        /// Get Auth Token
        /// </summary>
        /// <returns>User with Token</returns>
        [HttpPost]
        public async Task<IActionResult> GetToken(UserLogins userLogins)
        {
            try
            {
                var userFound = await _repo.GetAllAsync(u => u.UserName == userLogins.Username);

                if (!userFound.Any())
                {
                    _logger.LogInformation($"User with username {userLogins.Username} is not found");
                    return NotFound($"User with username {userLogins.Username} is not found");
                }

                var user = userFound.FirstOrDefault(x => x.UserName.Equals(userLogins.Username, StringComparison.OrdinalIgnoreCase));

                if (user.Password == userLogins.Password) // Compare passwords
                {
                    var token = JwtHelpers.JwtHelpers.GenTokenkey(new UserTokens()
                    {
                        GuidId = Guid.NewGuid(),
                        Role = user.Role,
                        Username = user.UserName,
                        UserId = user.UserId,
                    }, _jwtSettings);
                    return Ok(token);
                }
                else
                {
                    _logger.LogWarning($"Wrong username/password");
                    return BadRequest($"Wrong username/password");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetToken Exception {ex}");
                throw;
            }
        }
        /// <summary>
        /// Get List of Users
        /// </summary>
        /// <returns>List Of Users</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> GetList()
        {
            var users = await _repo.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get Roles list
        /// </summary>
        /// <returns>List of roles</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetRoles()
        {
            var roles = EnumNamedValues<Role>();
            return Ok(roles);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns>Ok</returns>
        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUp newUser)
        {
            try
            {
                if(newUser != null && newUser.Username != null && newUser.Password != null)
                {

                    var userFound = await _repo.GetAllAsync(u => u.UserName == newUser.Username);

                    if (userFound.Any())
                    {
                        _logger.LogInformation($"Username {newUser.Username} is not available");
                        return BadRequest($"Username {newUser.Username} is not available");
                    }

                    var createUser = new User()
                    {
                        UserName = newUser.Username,
                        Password = newUser.Password,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Role = Role.User
                    };

                    _repo.AddAsync(createUser);
                    return Ok();
                }else
                {
                    _logger.LogWarning($"Username and Password fields are mandetory");
                    return BadRequest($"Username and Password fields are mandetory");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"SignUp Exception {ex}");
                throw ex;
            }
            
        }
    }
}
