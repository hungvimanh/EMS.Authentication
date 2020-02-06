using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TF.Authentication.Commons;
using TF.Authentication.Entities;
using TF.Authentication.Repositories;

namespace TF.Authentication.Services
{
    public interface IUserService : IServiceScoped
    {
        Task<User> Login(UserFilter userFilter);
        Task<User> ChangePassword(UserFilter userFilter, string newPassword);
        Task<bool> ForgotPassword(UserFilter userFilter);
    }
    public class UserService : IUserService
    {
        private string Unauthorized = "Tài khoản hoặc mật khẩu không chính xác. Vui lòng thử lại";
        private string ChangePasswordFalse = "Đổi mật khẩu không thành công!";
        private string LengthOfPasswordNotEnough = "Độ dài mật khẩu tối thiểu là 10 kí tự";
        private AppSettings appSettings;
        private IDateTimeService DateTimeService;
        private readonly IUOW UOW;
        public UserService(IUOW _UOW, IDateTimeService dateTimeService, IOptions<AppSettings> options)
        {
            this.UOW = _UOW;
            this.DateTimeService = dateTimeService;
            this.appSettings = options.Value;
        }

        public async Task<User> Login(UserFilter userFilter)
        {
            User user = await this.Verify(userFilter);
            //Nếu tài khoản đăng nhập lần đầu tiên, hệ thống sẽ yêu cầu đổi mật khẩu để bảo vệ tài khoản
            if (userFilter.Password == user.Password)
            {
                user.FirstLogin = true;
            }
            else
            {
                user.FirstLogin = false;
            }
            user = await this.GenerateJWT(user, appSettings.JWTSecret, appSettings.JWTLifeTime);

            return user;
        }
        public async Task<User> ChangePassword(UserFilter userFilter, string newPassword)
        {
            //Khi đổi mật khẩu sẽ có thêm Salt
            // mỗi lần đổi salt sẽ được generate lại
            User user = await this.Verify(userFilter);

            if (newPassword.Length < 10)
            {
                throw new BadRequestException(LengthOfPasswordNotEnough);
            }
            user.Password = newPassword;
            //Generate Salt Random
            user.Salt = Convert.ToBase64String(CryptographyExtentions.GenerateSalt());
            user.Password = newPassword.HashHMACSHA256(user.Salt);
            bool IsValid = await this.UOW.UserRepository.ChangePassword(user);
            if (!IsValid)
            {
                throw new BadRequestException(ChangePasswordFalse);
            }
            return user;
        }

        public async Task<bool> ForgotPassword(UserFilter userFilter)
        {
            //Quên mật khẩu
            //Nếu thí sinh nhập đúng Identify và Email, hệ thống sẽ generate lại mật khẩu và gửi về email
            User user = await UOW.UserRepository.Get(userFilter);
            if (user == null) throw new BadRequestException("Id không tồn tại");
            if (!userFilter.Email.Equals(user.Email)) throw new BadRequestException("Email không đúng!");
            user.Password = CryptographyExtentions.GeneratePassword();
            user.Salt = null;
            await UOW.UserRepository.ChangePassword(user);
            MailSenderService mailSender = new MailSenderService(user);
            Thread sendMailThread = new Thread(new ThreadStart(mailSender.RecoveryPasswordMail));
            sendMailThread.Start();
            return true;
        }

        private async Task<User> Verify(UserFilter userFilter)
        {
            User user = await UOW.UserRepository.Get(userFilter);
            if (user == null) throw new BadRequestException(Unauthorized);

            //compare password + salt
            if (!CompareSaltHashedPassword(user.Password, userFilter.Password, user.Salt))
            {
                throw new BadRequestException(Unauthorized);
            }
            return user;
        }

        private bool CompareSaltHashedPassword(string source, string password, string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                return password.Equals(source);
            }
            var hashedPassword = password.HashHMACSHA256(salt);
            if (!hashedPassword.Equals(source))
            {
                return false;
            }
            return true;
        }

        private async Task<User> GenerateJWT(User user, string Secret, int LifeTime)
        {
            if (string.IsNullOrEmpty(Secret) || LifeTime <= 0)
                throw new BadRequestException("Fail to generate token, please try again.");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(nameof(user.StudentId), user.StudentId.ToString()),
                    new Claim("IsAdmin", user.IsAdmin.ToString())
                }),
                Expires = this.DateTimeService.UtcNow.AddSeconds(LifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            user.Jwt = token;
            user.ExpiredTime = tokenDescriptor.Expires;
            return user;
        }
    }
}
