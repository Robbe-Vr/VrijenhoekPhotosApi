
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class AccountsController : ProcessingController
    {
        private UsersHandler _users;

        public AccountsController(UsersHandler handler, JsonSerializerSettings jsonSettings) : base(jsonSettings)
        {
            _users = handler;
        }

        [HttpGet]
        [Authorized((int)Rights.Admin)]
        public IActionResult All([FromQuery] int page = 1, [FromQuery] int rpp = 10)
        {
            return Process(() =>
            {
                return _users.All(page, rpp);
            });
        }

        [HttpGet("count")]
        [Authorized((int)Rights.Admin)]
        public IActionResult Count()
        {
            return Process(() =>
            {
                return _users.Count();
            });
        }

        [HttpGet("join-requests")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetJoinRequests()
        {
            return Process(() =>
            {
                return _users.GetJoinRequests();
            });
        }

        [HttpGet("group-invites")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetGroupInvites()
        {
            return Process(() =>
            {
                return _users.GetGroupInvites();
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.None)]
        public IActionResult Update(UserDTO user)
        {
            return Process(() =>
            {
                return _users.UpdateName(user.UserName);
            });
        }

        [HttpPost("{action}/{userId}")]
        [Authorized((int)Rights.Admin)]
        public IActionResult Update(UserDTO user, string userId)
        {
            return Process(() =>
            {
                return _users.UpdateName(user.UserName, userId);
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.None)]
        public IActionResult Remove(UserDTO user)
        {
            return Process(() =>
            {
                return _users.Remove(user);
            });
        }

        [HttpGet("{action}/{nameOrEmail}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Find(string nameOrEmail)
        {
            return Process(() =>
            {
                try
                {
                    UserDTO user = _users.GetByNameOrEmail(nameOrEmail);

                    return Ok(new { Result = user != null, Id = user?.Id });
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while searching user by name {nameOrEmail}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                    return Problem(
                        detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                        title: "Error Occurred!",
                        statusCode: 501
                    );
                }
            });
        }

        [HttpGet("{action}/{nameOrEmail}")]
        public IActionResult ResetPassword(string nameOrEmail)
        {
            return Process(() =>
            {
                try
                {
                    UserDTO user = _users.GetByNameOrEmail(nameOrEmail);

                    if (user == null)
                    {
                        return Problem(
                            title: "Failed to start a password reset!",
                            detail: $"No user found by the given name or email!",
                            statusCode: 401
                        );
                    }

                    return Ok(new { Result = ResetManager.ResetPassword(user), Email = user?.Email });
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while setting up password reset for {nameOrEmail}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                    return Problem(
                        detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                        title: "Error Occurred!",
                        statusCode: 501
                    );
                }
            });
        }

        [HttpPost("{action}")]
        public IActionResult ResetPassword(UserDTO updatedUser, [FromQuery] string resetToken)
        {
            return Process(() =>
            {
                try
                {
                    UserDTO user = null;
                    if ((user = ResetManager.ValidateReset(resetToken)) != null)
                    {
                        Logger.Log($"password reset for {updatedUser.Id} successful!");
                        return Ok(new { Result = _users.UpdatePassword(updatedUser.PasswordHash, user.Id) });
                    }
                    else
                    {
                        Logger.Log($"Error while resetting password for {updatedUser.Id}!");
                        return Problem(
                            title: "Invalid reset token!",
                            detail: "The given reset token is invalid!",
                            statusCode: 401
                        );
                    }
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while resetting password for {updatedUser.Id}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                    return Problem(
                        detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                        title: "Error Occurred!",
                        statusCode: 501
                    );
                }
            });
        }
    }
}
