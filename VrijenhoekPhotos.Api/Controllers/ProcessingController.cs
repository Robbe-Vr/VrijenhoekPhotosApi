using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Core.Handlers;

namespace VrijenhoekPhotos.Api.Controllers
{
    public class ProcessingController : ControllerBase
    {
        private JsonSerializerSettings _jsonSettings;

        public ProcessingController(JsonSerializerSettings jsonSettings)
        {
            _jsonSettings = jsonSettings;
        }

        public IActionResult Process(Func<IActionResult> function, string actionName = "")
        {
            try
            {
                return function();
            }
            catch (Exception e)
            {
                Logger.Log($"Error while executing {actionName}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                return Problem(
                    detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                    title: "Error Occurred!",
                    statusCode: 501
                );
            }
        }

        public IActionResult Process(Func<ResponseResult> function, string actionName = null)
        {
            try
            {
                ResponseResult result = function();
                if (result.Error)
                {
                    Logger.Log($"Error while {actionName ?? $"processing {HttpContext.Request.Method} request to {HttpContext.Request.Path}"}!\nError: {result.ErrorMsg}\nat {result.Context}{(result.ObjectInAction != null ? $"\nObject: {JsonConvert.SerializeObject(result.ObjectInAction, _jsonSettings)}" : "")}");
                    return Problem(
                        title: result.ErrorTitle,
                        detail: result.ErrorMsg,
                        instance: result.Context.ToString()
                    );
                }
                else
                {
                    if (!String.IsNullOrEmpty(actionName))
                        Logger.Log($"{actionName} successful!\n{JsonConvert.SerializeObject(result.Result, _jsonSettings)}");

                    return Ok(result.Result);
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error while {actionName}!\nError: {e.Message}\nStackTrace:\n{e.StackTrace}");
                return Problem(
                    detail: $"Error Occurred while processing your request!\nError: {e.Message}",
                    title: "Error Occurred!",
                    statusCode: 501
                );
            }
        }
    }
}
