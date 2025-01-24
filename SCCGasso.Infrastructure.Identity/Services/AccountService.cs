using Azure;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SCC_Gasso.Core.Application.Dtos.Account;
using SCC_Gasso.Core.Application.Enums;
using SCC_Gasso.Core.Application.Helpers;
using SCC_Gasso.Core.Application.Interfaces.Services;
using SCCGasso.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static SCCGasso.Infrastructure.Identity.Services.AccountService;

namespace SCCGasso.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        //Local
        //private readonly string UrlPage = "https://localhost:7093/";




        //Produccion
        //private readonly string UrlPage = "https://aplicacion.gasso.local/";

        public AccountService(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        #region publicMethods

        #region Microsoft
        public async Task<AuthenticationResponse> AuthenticateWebApiByMicrosoftSSOAsync(LoginByMicrosoftSSODto authentication, string origin)
        {
            AuthenticationResponse response = new();

            bool isAuthenticated = false;

            if (authentication != null && authentication.Token != null)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(authentication.Mail);

                    if (user != null)
                    {
                        string previousLogin = user.LastLogin;

                        user.LastLogin = GetDateTime.GetDateTimeInString();

                        JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);

                        response.Id = user.Id;
                        response.Email = user.Email;
                        response.UserName = user.UserName;

                        var roleList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                        response.Role = roleList.FirstOrDefault();

                        response.IsVerified = user.EmailConfirmed;
                        response.FirstName = user.FirstName;
                        response.LastName = user.LastName;
                        response.UserStatus = true;

                        response.LastLogin = previousLogin;

                        await _userManager.UpdateAsync(user);

                        isAuthenticated = true;
                        return response;
                    }
                    else
                    {
                        var userSave = new ApplicationUser
                        {
                            Id = authentication.Id,
                            Email = authentication.Mail,
                            FirstName = authentication.GivenName,
                            LastName = authentication.Surname,
                            IsActive = true,
                            PhoneNumber = authentication.BusinessPhones.FirstOrDefault(),
                            UserName = authentication.GivenName,
                            IdRoleAppPermission = 5,
                        };

                        string userRoles = RolesEnum.Administrador.ToString();
                        string password = "@Gasso-2024!";

                        var result = await _userManager.CreateAsync(userSave, password);
                        if (!result.Succeeded)
                        {
                            response.HasError = true;
                            response.Error = "Failed to create user";
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(userSave, userRoles);
                            var verificationURI = await SendVerificationUri(userSave, origin);

                            if (userRoles == RolesEnum.Administrador.ToString())
                            {
                                userSave.EmailConfirmed = true;
                                response.Error = "Cuenta Registrada";
                                await _userManager.UpdateAsync(userSave);

                                await _emailService.SendAsync(new Core.Application.Dtos.Email.EmailRequest()
                                {
                                    To = userSave.Email,
                                    Subject = "Registro Exitoso en la App de Activos Tecnológicos",
                                    Body = $@"
                                        <!DOCTYPE html>
                                        <html lang='en'>
                                        <head>
                                            <meta charset='UTF-8'>
                                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                            <style>
                                                body {{
                                                    font-family: Arial, sans-serif;
                                                    margin: 0;
                                                    padding: 0;
                                                    background-color: #f4f9f4; /* Fondo suave verde */
                                                }}
                                                .email-container {{
                                                    max-width: 600px;
                                                    margin: 20px auto;
                                                    background-color: #ffffff;
                                                    border: 1px solid #e3e3e3;
                                                    border-radius: 8px;
                                                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                                }}
                                                .header {{
                                                    background-color: #28a745; /* Verde oscuro */
                                                    color: white;
                                                    padding: 20px;
                                                    text-align: center;
                                                    border-top-left-radius: 8px;
                                                    border-top-right-radius: 8px;
                                                }}
                                                .header img {{
                                                    max-width: 320px;
                                                    margin-bottom: 10px;
                                                    border-radius: 10px; 
                                                }}
                                                .content {{
                                                    padding: 20px;
                                                    color: #333333;
                                                    line-height: 1.6;
                                                }}
                                                .footer {{
                                                    background-color: #f0f8f0; /* Verde muy claro */
                                                    text-align: center;
                                                    padding: 10px;
                                                    font-size: 14px;
                                                    color: #666666;
                                                    border-bottom-left-radius: 8px;
                                                    border-bottom-right-radius: 8px;
                                                }}
                                                .footer a {{
                                                    color: #28a745;
                                                    text-decoration: none;
                                                }}
                                                .highlight-green  {{color: #28a745; 
                                                    font-weight: bold;
                                                }}
                                            </style>
                                        </head>
                                        <body>
                                            <div class='email-container'>
                                                <div class='header'>
                                                    <img src='https://assets.gomarket.com.do/rails/active_storage/blobs/proxy/eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaHBBMDFXSEE9PSIsImV4cCI6bnVsbCwicHVyIjoiYmxvYl9pZCJ9fQ==--819c28b19917b73e8f1d32193b448ceb5f0a4727/Cintillo%20Gass%C3%B3%20-%20Go%20Market.jpg' alt='Gasso Logo'>
                                                    <h1>¡Bienvenido(a) a la App de Activos Tecnológicos!</h1>
                                                </div>
                                                <div class='content'>
                                                    <p>Hola, {userSave.FirstName}</p>
                                                    <p>Nos complace informarte que has sido registrado exitosamente como <strong>""Miembro""</strong> en nuestra App de Activos Tecnológicos del Grupo J. Gassó Gassó SAS.</p>
                                                    <p>Ahora podrás acceder a nuestras herramientas y recursos diseñados para optimizar la gestión de activos tecnológicos.</p>
                                                    <p>Si tienes alguna duda o necesitas ayuda, no dudes en contactarnos.</p>
                                                    <p>Gracias,</p>
                                                    <p>El equipo de Tecnología de J. Gassó Gassó SAS</p>
                                                    <br />
                                                    <p>Sus credenciales son las siguientes: </p>
                                                    <p><strong class=""highlight-green"">Usuario: {userSave.Email}</strong></p>
                                                    <p><strong class=""highlight-green"">Contraseña: {password}</strong></p>
                                                        <br />
                                                    <div style=""text-align: center; margin-top: 20px;"">
                                                    <a href=""{UrlPage}"" style=""text-decoration: none;"">
                                                        <button style=""background-color: #28a745; color: white; padding: 10px 20px; border: none; border-radius: 5px; font-size: 16px; cursor: pointer;"">
                                                        Ir al sitio web
                                                        </button>
                                                    </a>
                                                    </div>
                                                </div>
                                                <div class='footer'>
                                                    <p>¿Necesitas ayuda? <a href='mailto:soportetecnico@gasso.com.do'>Contáctanos</a></p>
                                                    <p>&copy; 2024 J. Gassó Gassó SAS Dept. de Tecnología. Juntos, construyendo un mejor mañana.</p>
                                                </div>
                                            </div>
                                        </body>
                                        </html>
                                    "
                                });
                            }

                            response = await AuthenticateWebApiByMicrosoftSSOAsync(authentication, origin);
                            return response;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.Error = ex.Message;
                }
            }

            return response;
        }

        #endregion



        public async Task<AuthenticationResponse> AuthenticateWebApiAsync(AuthenticationRequest request)
        {
            AuthenticationResponse response = new();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.HasError = true;
                response.Error = $"No accounts registered under Email {request.Email}";
                return response;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Error = $"Invalid Credential for {request.Email}";
                return response;
            }
            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.Error = $"Account not confirmed for {request.Email}";
                return response;
            }
            if (user.IsActive == false)
            {
                response.HasError = true;
                response.Error = $"Your account user {request.Email} is not active please get in contact with a manager";
                return response;
            }

            string previousLogin = user.LastLogin;

            user.LastLogin = GetDateTime.GetDateTimeInString();

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);

            response.Id = user.Id;
            response.Email = user.Email;
            response.UserName = user.UserName;

            var roleList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Role = roleList.FirstOrDefault();

            response.IsVerified = user.EmailConfirmed;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.UserStatus = true;

            response.LastLogin = previousLogin;

            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var refreshToken = GenerateRefreshToken();
            response.RefreshToken = refreshToken.Token;

            await _userManager.UpdateAsync(user);

            return response;
        }

        //SINGOUT
        public async Task SingOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        //REGISTER USER

        public async Task<Response<int>> RegisterUserAsync(RegisterRequest request, string origin, string UserRoles)
        {
            Response<int> response = new();
            response.Succeeded = true;

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                response.Succeeded = false;
                response.Message = $"El correo {request.Email} ya está registrado";
                return response;
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = false,
                PhoneNumber = request.PhoneNumber,
                UserName = request.FirstName,
                IdRoleAppPermission = request.IdRoleAppPermission
            };

            if (UserRoles == RolesEnum.Administrador.ToString() || UserRoles == RolesEnum.Asistente.ToString() || UserRoles == RolesEnum.Supervisor.ToString())
            {
                user.IsActive = true;
            }

            try
            {
                UserRoles = RolesEnum.Administrador.ToString();
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    response.Succeeded = false;
                    response.Message = "Failed to create user";
                    response.Errors = result.Errors.Select(e => e.Description).ToList();
                }
                else
                {
                    var registeredUser = await _userManager.FindByEmailAsync(user.Email);


                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles);

                        var verificationURI = await SendVerificationUri(user, origin);
                        if (UserRoles == RolesEnum.Supervisor.ToString() || UserRoles == RolesEnum.Asistente.ToString() || UserRoles == RolesEnum.Administrador.ToString())
                        {
                            user.EmailConfirmed = true;
                            response.Message = "Cuenta Registrada";
                            var registeredUserForEmailConfirm = await _userManager.FindByEmailAsync(user.Email);
                            await _userManager.UpdateAsync(registeredUserForEmailConfirm);

                            await _emailService.SendAsync(new Core.Application.Dtos.Email.EmailRequest()
                            {
                                To = user.Email,
                                Subject = "Registro Exitoso en la App de Activos Tecnológicos",
                                Body = $@"
                                            <!DOCTYPE html>
                                            <html lang='en'>
                                            <head>
                                                <meta charset='UTF-8'>
                                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                                <style>
                                                    body {{
                                                        font-family: Arial, sans-serif;
                                                        margin: 0;
                                                        padding: 0;
                                                        background-color: #f4f9f4; /* Fondo suave verde */
                                                    }}
                                                    .email-container {{
                                                        max-width: 600px;
                                                        margin: 20px auto;
                                                        background-color: #ffffff;
                                                        border: 1px solid #e3e3e3;
                                                        border-radius: 8px;
                                                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                                    }}
                                                    .header {{
                                                        background-color: #28a745; /* Verde oscuro */
                                                        color: white;
                                                        padding: 20px;
                                                        text-align: center;
                                                        border-top-left-radius: 8px;
                                                        border-top-right-radius: 8px;
                                                    }}
                                                    .header img {{
                                                        max-width: 320px;
                                                        margin-bottom: 10px;
                                                        border-radius: 10px; 
                                                    }}
                                                    .content {{
                                                        padding: 20px;
                                                        color: #333333;
                                                        line-height: 1.6;
                                                    }}
                                                    .footer {{
                                                        background-color: #f0f8f0; /* Verde muy claro */
                                                        text-align: center;
                                                        padding: 10px;
                                                        font-size: 14px;
                                                        color: #666666;
                                                        border-bottom-left-radius: 8px;
                                                        border-bottom-right-radius: 8px;
                                                    }}
                                                    .footer a {{
                                                        color: #28a745;
                                                        text-decoration: none;
                                                    }}

                                                    .highlight-green  {{color: #28a745; 
                                                        font-weight: bold;
                                                    }}
                                                </style>
                                            </head>
                                            <body>
                                                <div class='email-container'>
                                                    <div class='header'>
                                                        <img src='https://assets.gomarket.com.do/rails/active_storage/blobs/proxy/eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaHBBMDFXSEE9PSIsImV4cCI6bnVsbCwicHVyIjoiYmxvYl9pZCJ9fQ==--819c28b19917b73e8f1d32193b448ceb5f0a4727/Cintillo%20Gass%C3%B3%20-%20Go%20Market.jpg' alt='Gasso Logo'>
                                                        <h1>¡Bienvenido(a) a la App de Activos Tecnológicos!</h1>
                                                    </div>
                                                    <div class='content'>
                                                        <p>Hola, {user.FirstName}</p>
                                                        <p>Nos complace informarte que has sido registrado exitosamente como <strong>parte de nuestro equipo</strong> en nuestra App de Activos Tecnológicos del Grupo J. Gassó Gassó SAS.</p>
                                                        <p>Ahora podrás acceder a nuestras herramientas y recursos diseñados para optimizar la gestión de activos tecnológicos.</p>
                                                        <p>Si tienes alguna duda o necesitas ayuda, no dudes en contactarnos.</p>
                                                        <p>Gracias,</p>
                                                        <p>El equipo de Tecnología de J. Gassó Gassó SAS</p>
                                                        <br />
                                                        <p>Sus credenciales son las siguientes: </p>
                                                        <p><strong class=""highlight-green"">Usuario: {user.Email}</strong></p>
                                                        <p><strong class=""highlight-green"">Contraseña: {request.Password}</strong></p>
                                                        <br />
                                                        <div style=""text-align: center; margin-top: 20px;"">
                                                        <a href=""{UrlPage}"" style=""text-decoration: none;"">
                                                            <button style=""background-color: #28a745; color: white; padding: 10px 20px; border: none; border-radius: 5px; font-size: 16px; cursor: pointer;"">
                                                            Ir al sitio web
                                                            </button>
                                                        </a>
                                                        </div>
                                                    </div>
                                                    <div class='footer'>
                                                        <p>¿Necesitas ayuda? <a href='mailto:soportetecnico@gasso.com.do'>Contáctanos</a></p>
                                                        <p>&copy; 2024 J. Gassó Gassó SAS Dept. de Tecnología. Juntos, construyendo un mejor mañana.</p>
                                                    </div>
                                                </div>
                                            </body>
                                            </html>
                                        "
                            });

                            return response;
                        }
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.Message = $"An error occurred trying to register the user.";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {

                response.Succeeded = false;
                response.Message = "An error occurred while creating the user";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }

        //RESETPASSWORD

        public async Task<Response<int>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            Response<int> response = new();
            response.Succeeded = true;
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.Succeeded = false;
                response.Message = $"No Accounts registered with {request.Email}";
                return response;
            }

            request.Token = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

            if (!result.Succeeded)
            {
                response.Succeeded = false;
                response.Message = $"An error occurred while reset password";
                return response;
            }

            return response;
        }


        //GETBYID
        public async Task<DtoAccounts> GetByIdAsync(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            DtoAccounts dtoaccount = new()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber,
                Password = user.PasswordHash,
                IdRoleAppPermission = user.IdRoleAppPermission
            };
            return dtoaccount;
        }



        //CONFIRMACCOUNT
        public async Task<string> ConfirmAccountAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return $"No user register under this {user.Email} account";
            }

            token = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return $"Account confirm for {user.Email} you can now  use the app";
            }
            else
            {
                return $"An error occurred wgile confirming {user.Email}.";
            }
        }

        //FORGOTPASSWORD
        public async Task<Response<int>> ForgotPassswordAsync(ForgotPasswordRequest request, string origin)
        {
            Response<int> response = new();
            response.Succeeded = true;
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.Succeeded = false;
                response.Message = $"No Accounts registered with {request.Email}";
                return response;
            }

            string defaultPassword = "@Gasso-2024!";
            string newPassword = GenerateRandomPassword(defaultPassword);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!passwordResetResult.Succeeded)
            {
                response.Succeeded = false;
                response.Message = "Error resetting password";
                return response;
            }


            await _emailService.SendAsync(new Core.Application.Dtos.Email.EmailRequest()
            {
                To = user.Email,
                Subject = "Nueva Contraseña Generada",
                Body = $@"
                        <!DOCTYPE html>
                        <html lang='en'>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <style>
                                body {{
                                    font-family: Arial, sans-serif;
                                    margin: 0;
                                    padding: 0;
                                    background-color: #f4f9f4; /* Fondo suave verde */
                                }}
                                .email-container {{
                                    max-width: 600px;
                                    margin: 20px auto;
                                    background-color: #ffffff;
                                    border: 1px solid #e3e3e3;
                                    border-radius: 8px;
                                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                }}
                                .header {{
                                    background-color: #28a745; /* Verde oscuro */
                                    color: white;
                                    padding: 20px;
                                    text-align: center;
                                    border-top-left-radius: 8px;
                                    border-top-right-radius: 8px;
                                }}
                                .header img {{
                                    max-width: 320px;
                                    margin-bottom: 10px;
                                    border-radius: 10px; 
                                }}
                                .content {{
                                    padding: 20px;
                                    color: #333333;
                                    line-height: 1.6;
                                }}
                                .content a {{
                                    color: #28a745; /* Verde oscuro */
                                    text-decoration: none;
                                    font-weight: bold;
                                }}
                                .footer {{
                                    background-color: #f0f8f0; /* Verde muy claro */
                                    text-align: center;
                                    padding: 10px;
                                    font-size: 14px;
                                    color: #666666;
                                    border-bottom-left-radius: 8px;
                                    border-bottom-right-radius: 8px;
                                }}
                                .footer a {{
                                    color: #28a745;
                                    text-decoration: none;
                                }}

                                .highlight-green  {{color: #28a745; 
                                    font-weight: bold;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='email-container'>
                                <div class='header'>
                                    <img src='https://assets.gomarket.com.do/rails/active_storage/blobs/proxy/eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaHBBMDFXSEE9PSIsImV4cCI6bnVsbCwicHVyIjoiYmxvYl9pZCJ9fQ==--819c28b19917b73e8f1d32193b448ceb5f0a4727/Cintillo%20Gass%C3%B3%20-%20Go%20Market.jpg' alt='Gasso Logo'>
                                    <h1>Nueva Contraseña</h1>
                                </div>
                                <div class='content'>
                                    <p>Hola,</p>
                                    <p>Hemos recibido una solicitud para restablecer tu contraseña. Esta es nueva contraseña: </p>

                                    <p><strong class=""highlight-green"">{newPassword}</strong></p>

                                    <p>Si no solicitaste este cambio, ignora este mensaje.</p>
                                    <p>Gracias,</p>
                                    <p>El equipo de Tecnología de J. Gasso Gasso SAS</p>
                                </div>
                                <div class='footer'>
                                    <p>¿Necesitas ayuda? <a href='mailto:soportetecnico@gasso.com.do'>Contáctanos</a></p>
                                    <p>&copy; 2024 J. Gasso Gasso SAS Dept. de Tecnología. Juntos, construyendo un mejor mañana.</p>
                                </div>
                            </div>
                        </body>
                        </html>
                    "
            });
            return response;
        }


        private string GenerateRandomPassword(string defaultPassword)
        {
            Random random = new Random();

            char[] characters = defaultPassword.ToCharArray();

            for (int i = 0; i < characters.Length; i++)
            {
                if (char.IsDigit(characters[i]))
                {
                    characters[i] = random.Next(0, 10).ToString()[0];
                }
            }

            return new string(characters.OrderBy(c => random.Next()).ToArray());
        }


        public async Task<DtoAccounts> GetByEmail(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            DtoAccounts dtoaccount = new()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber,
            };
            return dtoaccount;
        }


        //DELETE USER
        public async Task Remove(DtoAccounts account)
        {
            Response<int> response = new();

            var user = await _userManager.FindByIdAsync(account.Id);
            if (user == null)
            {
                response.Succeeded = false;
                response.Message = $"This user does not exist now";
            }
            await _userManager.DeleteAsync(user);
        }

        //USERS GETALL

        public async Task<List<DtoAccounts>> GetAllUsers()
        {
            var userList = await _userManager.Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToListAsync();
            List<DtoAccounts> DtoUserList = new();

            foreach (var user in userList)
            {
                var userDto = new DtoAccounts();
                userDto.FirstName = user.FirstName;
                userDto.LastName = user.LastName;
                userDto.IsActive = user.IsActive;
                userDto.Email = user.Email;
                userDto.Id = user.Id;
                userDto.IdRoleAppPermission = user.IdRoleAppPermission;

                var roles = _userManager.GetRolesAsync(user).Result.ToList();
                userDto.Role = roles.FirstOrDefault();


                DtoUserList.Add(userDto);
            }
            return DtoUserList;
        }



        public async Task<DtoAccounts> FindUserWithFilters(FilterFindUserDto user)
        {
            var applicationUser = _userManager.Users.FirstOrDefault(u => u.Id == user.IdUser);
            var userDto = new DtoAccounts();

            if (applicationUser != null)
            {
                userDto.Email = applicationUser.Email;
                userDto.FirstName = applicationUser.FirstName;
                userDto.LastName = applicationUser.LastName;
                userDto.IsActive = applicationUser.IsActive;
                userDto.Email = applicationUser.Email;
                userDto.Id = applicationUser.Id;
                userDto.IdRoleAppPermission = applicationUser.IdRoleAppPermission;

                if (userDto.IsActive == false)
                {
                    return null;
                }
                else
                {
                    return userDto;
                }
            }
            else
            {
                return null;
            }
        }

        //CHANGE USER STATUS
        public async Task<Response<int>> ChangeUserStatus(RegisterRequest request)
        {
            Response<int> response = new();
            response.Succeeded = true;
            var userget = await _userManager.FindByIdAsync(request.Id);
            {
                userget.IsActive = request.IsActive;
            }
            var result = await _userManager.UpdateAsync(userget);
            if (!result.Succeeded)
            {
                response.Succeeded = false;
                response.Message = $"There was an error while trying to update the user{userget.UserName}";
            }
            return response;
        }

        //EDITUSER
        public async Task<Response<int>> UpdateUserAsync(RegisterRequest request)
        {
            Response<int> response = new();
            response.Succeeded = true;

            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                response.Succeeded = false;
                response.Message = $"User with ID {request.Id} not found.";
                return response;
            }
            var emailExists = await _userManager.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != request.Id);
            if (emailExists)
            {
                response.Succeeded = false;
                response.Message = $"El correo {request.Email} esta siendo usado por otro usuario.";
                return response;
            }

            user.PhoneNumber = request.PhoneNumber;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.IsActive = request.IsActive;
            user.IdRoleAppPermission = request.IdRoleAppPermission;

            if (!string.IsNullOrEmpty(request.ConfirmPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!result.Succeeded)
                {
                    response.Succeeded = false;
                    response.Message = $"Failed to reset password for user {user.UserName}.";
                    return response;
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                response.Succeeded = false;
                response.Message = $"Failed to update user {user.UserName}.";
            }

            return response;
        }

        public async Task<bool> ValidateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region PrivateMethods

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email,user.Email),
            new Claim("uid", user.Id)
        }
            .Union(userClaims)
            .Union(roleClaims);

            var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredetials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
            claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredetials);

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var ramdomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(ramdomBytes);

            return BitConverter.ToString(ramdomBytes).Replace("-", "");
        }


        private async Task<string> SendVerificationUri(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/Account/confirm-email";
            var Uri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(Uri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", code);

            return verificationUri;
        }
        private async Task<string> SendForgotPasswordUri(ApplicationUser user, string origin)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "User/ResetPassword";
            var Uri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(Uri.ToString(), "token", code);

            return verificationUri;
        }


        #endregion
    }
}
