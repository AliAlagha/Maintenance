using Maintenance.Core.Dtos;
using Maintenance.Core.Exceptions;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Maintenance.Infrastructure.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStringLocalizer<Messages> _localizedMessages;

        public ExceptionHandler(RequestDelegate next,
            IServiceProvider serviceProvider, IStringLocalizer<Messages> localizedMessages)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _localizedMessages = localizedMessages;
        }

        public async Task Invoke(HttpContext context)
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature.Error;
            var response = new JsonResponse();
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            switch (exception)
            {
                case BusinessException:
                    var message = _localizedMessages[((BusinessException)exception).Message];
                    response.msg = $"e:{message}";
                    response.close = 0;
                    response.status = 0;
                    break;
                default:
                    var requestBody = string.Empty;
                    var req = context.Request;
                    req.EnableBuffering();
                    if (req.Body.CanSeek)
                    {
                        req.Body.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(
                            req.Body,
                            Encoding.UTF8,
                            false,
                            8192,
                            true))
                        {
                            requestBody = await reader.ReadToEndAsync();
                        }
                        req.Body.Seek(0, SeekOrigin.Begin);
                    }

                    var extraMessage = $"<p>Request URL: {context.Request.GetDisplayUrl()}</p>" +
                                   $"<p>Request body: {requestBody}</p>";
                    var innerException = "";
                    if (exception.InnerException != null)
                    {
                        innerException = exception.InnerException.Message;
                    }
                    response.msg = $"e:{exception.Message}, {innerException}, {extraMessage}";
                    response.close = 0;
                    response.status = 0;
                    break;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(response)
            );
        }
    }
}
