﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BisPlatformWeb.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class LogAttribute: ActionFilterAttribute
    {
        private string LogFlag { get; set; }
        private string ActionArguments { get; set; }
        private Stopwatch Stopwatch { get; set; }

        public LogAttribute(string logFlag)
        {
            LogFlag = logFlag;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            ActionArguments = Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments);

            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            Stopwatch.Stop();

            string url = context.HttpContext.Request.Host + context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            string method = context.HttpContext.Request.Method;

            string qs = ActionArguments;

            dynamic result = context.Result.GetType().Name == "EmptyResult" ? new { Value = "无返回结果" } : context.Result as dynamic;

            string res = "在返回结果前发生了异常";
            try
            {
                if (result != null)
                {
                    res = Newtonsoft.Json.JsonConvert.SerializeObject(result.Value);
                }
            }
            catch (System.Exception)
            {
                res = "日志未获取到结果，返回的数据无法序列化";
            }
            NlogHelper.InfoLog($"\n 方法：{LogFlag} \n " +
                $"地址：{url} \n " +
                $"方式：{method} \n " +
                $"参数：{qs}\n " +
                $"结果：{res}\n " +
                $"耗时：{Stopwatch.Elapsed.TotalMilliseconds} 毫秒（指控制器内对应方法执行完毕的时间）");

        }
    }
}
