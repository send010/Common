using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;

namespace Utility
{
    public class ThreadExceptionHandler
    {
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Application_ThradException(object sender, ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            RunError(str);
            Environment.Exit(0);
        }

        /// <summary>
        /// 本域异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            RunError(str);
            Environment.Exit(0);
        }

        /// <summary>
        /// 运行异常收集
        /// </summary>
        /// <param name="error"></param>
        private void RunError(string error)
        {
            string stmp = Assembly.GetExecutingAssembly().Location;

            stmp = stmp.Substring(0, stmp.LastIndexOf('\\'));
            StreamWriter sw = new StreamWriter(stmp + "\\error." + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + ".log", false, Encoding.UTF8);
            sw.Write(error);
            sw.Close();

            Debug.WriteLine(error);
        }


        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【进程ID】：" + System.Diagnostics.Process.GetCurrentProcess().Id);
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }
    }
}
