using EMS.Authentication.Controller.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF.Authentication.Controller.DTO;
using TF.Authentication.Entities;
using TF.Authentication.Services;

namespace TF.Authentication.Controller
{
    [Route("api/TF/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private IUserService userService;
        public AuthenticationController(IUserService _userService)
        {
            this.userService = _userService;
        }

        #region Login
        [AllowAnonymous]
        [Route("login"), HttpPost]
        public async Task<LoginResultDTO> Login([FromBody] LoginDTO loginDTO)
        {
            UserFilter userFilter = new UserFilter()
            {
                Username = loginDTO.Username,
                Password = loginDTO.Password
            };
            User user = await this.userService.Login(userFilter);
            Response.Cookies.Append("token", user.Jwt);
            return new LoginResultDTO()
            {
                Username = user.Username,
                Token = user.Jwt,
                ExpiredTime = user.ExpiredTime,
                FirstLogin = user.FirstLogin,
                IsAdmin = user.IsAdmin
            };
        }
        #endregion

        #region Change Password
        [Route("change-password"), HttpPost]
        public async Task<bool> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            UserFilter userFilter = new UserFilter()
            {
                Username = changePasswordDTO.Username,
                Password = changePasswordDTO.OldPassword
            };
            User user = await this.userService.ChangePassword(userFilter, changePasswordDTO.NewPassword);
            return user != null;
        }
        #endregion

        #region Forgot Password
        [AllowAnonymous]
        [Route("forgot-password"), HttpPost]
        public async Task<bool> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            UserFilter userFilter = new UserFilter()
            {
                Username = forgotPasswordDTO.Username,
                Email = forgotPasswordDTO.Email
            };
            await this.userService.ForgotPassword(userFilter);
            return true;
        }
        #endregion
    }
}
