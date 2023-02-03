using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pccl.Audit
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditProvider auditProvider)
        {
            if (auditProvider?.Options?.IsEnabledRequestAudit == true)
            {
                var now = DateTime.Now;
                var watch = new Stopwatch();
                watch.Start();
                Exception exception = null;
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    watch.Stop();

                    if (exception != null || watch.ElapsedMilliseconds > auditProvider.Options.RequestAuditMinExecutionDuration)
                    {
                        string body = null;
                        if (context.Request.HasFormContentType)
                        {
                            body = JsonConvert.SerializeObject(context.Request.Form.Keys.Select(v => new { Key = v, Value = context.Request.Form[v] }));
                        }
                        if (context.Request.HasJsonContentType())
                        {
                            context.Request.EnableBuffering();
                            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                            {
                                body = await reader.ReadToEndAsync();
                                context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
                            }
                        }

                        var auditLog = new RequestAuditLog((int)watch.ElapsedMilliseconds, now, UriHelper.GetDisplayUrl(context.Request), context.Request.Method, context.Response.StatusCode, body, context.Connection.RemoteIpAddress.ToString(), context.Request?.Headers?["User-Agent"]);
                        auditProvider.Trace(auditLog, exception);
                    }

                    if (exception != null)
                        await HandleExceptionAsync(auditProvider.GetRequestId(), context, exception);
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private async Task HandleExceptionAsync(string requestId, HttpContext context, Exception exception)
        {
            if (exception == null)
                return;

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "服务器异常,请尝试重新操作..";
            if (exception is IBusinessException)
            {
                message = exception.Message;
                statusCode = HttpStatusCode.OK;
            }
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { success = false, message = message, requestId = requestId });
        }
    }
}
