using NFine.Code;
using System;
using System.Web;
using System.Web.Mvc;

namespace NFine.Web
{
    public class HandlerErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            WriteLog(context);
            if (context.ExceptionHandled)
                return;
            base.OnException(context);
            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = 200;
            context.Result = new ContentResult { Content = new AjaxResult { state = ResultType.error.ToString(), message = context.Exception.Message }.ToJson() };
        }
        private void WriteLog(ExceptionContext context)
        {
            var log = LogFactory.GetLogger(context.Controller.ToString());
            Exception error = context.Exception;
            LogMessage logMessage = new LogMessage();
            logMessage.OperationTime = DateTime.Now;
            logMessage.Url = HttpContext.Current.Request.RawUrl;
            logMessage.Class = context.Controller.ToString();
            logMessage.Ip = Net.Ip;
            logMessage.IpAddressName = Net.GetLocation(logMessage.Ip);
            logMessage.Host = Net.Host;
            logMessage.Browser = Net.Browser;
            logMessage.Mac = Net.GetClientMac();
            logMessage.UserName = OperatorProvider.Provider.GetCurrent()?.UserName;
            logMessage.InputParameters = HttpMethods.GetRequestValues(HttpContext.Current.Request.HttpMethod);
            logMessage.ExceptionInfo = error.InnerException == null ? error.Message : error.InnerException.Message;
            logMessage.ExceptionSource = error.Source;
            logMessage.ExceptionRemark = error.StackTrace;
            string body = logMessage.ExceptionFormat();
            log.Error(body);

        }
    }
}