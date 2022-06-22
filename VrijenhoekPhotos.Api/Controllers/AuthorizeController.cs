using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Api.Authorization;
using VrijenhoekPhotos.Api.Filters;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Controllers
{
    [EnableCors("VrijenhoekPhotosCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ProcessingController
    {
        private AuthorizationHandler _authorization;
        public AuthorizeController(AuthorizationHandler authorization, JsonSerializerSettings jsonSettings) : base(jsonSettings)
        {
            _authorization = authorization;
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.None)]
        public IActionResult Validate(TokenModel model)
        {
            return Process(() =>
            {
                try
                {
                    UserDTO user = AuthManager.GetUser(model.VrijenhoekPhotos_AccessToken);

                    string accessToken = "";
                    if (!string.IsNullOrEmpty(user?.Id))
                        accessToken = AuthManager.GetToken(user.Id);

                    return Ok(new TokenModel()
                    {
                        Result = accessToken != model.VrijenhoekPhotos_AccessToken ? accessToken.Length == 45 ? "Invalid AccessToken!" : accessToken : "Success."
                    });
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while validating token {model.VrijenhoekPhotos_AccessToken}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                    return Problem(
                        detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                        title: "Error Occurred!",
                        statusCode: 501
                    );
                }
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.None)]
        public IActionResult Refresh(TokenModel model)
        {
            return Process(() =>
            {
                try
                {
                    string refreshedAccessToken = AuthManager.RefreshToken(model.VrijenhoekPhotos_RefreshToken);

                    if (refreshedAccessToken != "Not found.")
                    {
                        UserDTO user = AuthManager.GetUser(refreshedAccessToken);

                        return Ok(new AuthorizationResultDTO()
                        {
                            AccessToken = refreshedAccessToken,
                            Id = user.Id,
                            UserName = user.UserName,
                            Rights = user.Rights,
                            Success = true
                        });
                    }
                    else
                    {
                        Logger.Log($"Error while refreshing token {model.VrijenhoekPhotos_RefreshToken}!");
                        return Ok(new AuthorizationResultDTO()
                        {
                            Success = false
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while refreshing token {model.VrijenhoekPhotos_RefreshToken}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                    return Problem(
                        detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                        title: "Error Occurred!",
                        statusCode: 501
                    );
                }
            });
        }

        [HttpPost("{action}")]
        public IActionResult LogIn(LoginModel model)
        {
            return Process(() =>
            {
                try
                {
                    var result = _authorization.Login(
                        new AuthorizationDTO()
                        {
                            UserName = model.UserName,
                            Password = model.Password,
                        }
                    );

                    if (result.Success)
                    {
                        UserDTO user = _authorization.GetNewUser(model.UserName);
                        result.RefreshToken = AuthManager.SignIn(
                            user
                        );
                        result.AccessToken = AuthManager.GetToken(result.Id);
                        result.Rights = user.Rights;

                        Logger.Log($"logged in user successfully!\n{JsonConvert.SerializeObject(new { result.Id, Name = result.UserName }, Formatting.None)}");
                        return Ok(result);
                    }
                    else
                    {
                        Logger.Log($"Error while logging in user {model.UserName}!\nError: {result.Error}\n{(result != null ? $"\nObject: {JsonConvert.SerializeObject(result, Formatting.None)}" : "")}");
                        return Problem(
                            title: result.Error,
                            detail: result.Error
                        );
                    }
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while logging in user {model.UserName}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                    return Problem(
                        detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                        title: "Error Occurred!",
                        statusCode: 501
                    );
                }
            });
        }

        [HttpPost("{action}")]
        public IActionResult Register(RegisterModel model)
        {
            return Process(() =>
            {
                try
                {
                    var result = _authorization.Register(
                        new AuthorizationDTO()
                        {
                            Email = model.Email,
                            UserName = model.UserName,
                            Password = model.Password,
                        },
                        model.ConfirmPassword,
                        model.CreationDate == default ? DateTime.UtcNow : model.CreationDate
                    );

                    if (result.Success)
                    {
                        UserDTO user = _authorization.GetNewUser(model.UserName);
                        result.RefreshToken = AuthManager.SignIn(
                            user
                        );
                        result.AccessToken = AuthManager.GetToken(result.Id);
                        result.Rights = user.Rights;

                        Logger.Log($"Registered new user successfully!\n{JsonConvert.SerializeObject(new { result.Id, Name = result.UserName, Email = result.Email }, Formatting.None)}");
                        return Ok(result);
                    }
                    else
                    {
                        result.Password = result.Password.Length + " chars password.";
                        Logger.Log($"Error while registering new user!\nError: {result.Error}\n{(result != null ? $"\nObject: {JsonConvert.SerializeObject(result, Formatting.None)}" : "")}");
                        return Problem(
                            title: result.Error,
                            detail: result.Error
                        );
                    }
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while registering new user!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                    return Problem(
                        detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                        title: "Error Occurred!",
                        statusCode: 501
                    );
                }
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult LogOut()
        {
            return Process(() =>
            {
                string accessToken = HttpContext.Request.Headers["VrijenhoekPhotos_AccessToken"].ToString();

                UserDTO user;
                if ((user = AuthManager.GetUser(accessToken)) == null)
                {
                    return Problem(
                        title: "Failed to log out!",
                        detail: $"No user found logged in with given access token!"
                    );
                }

                if (!AuthManager.SignOut(user.Id))
                {
                    return Problem(
                        title: "Failed to log out!",
                        detail: $"An error occured while attempting to log out user with id: '{user.Id}'"
                    );
                }

                return Ok(new { Result = true, Message = "Successfully logged out." });
            });
        }
    }
}
