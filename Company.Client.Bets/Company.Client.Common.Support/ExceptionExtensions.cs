using System;
using System.Text;

namespace Company.Client.Common.Extensions
{
    public static class ExceptionExtensions
    {
        const int MAX_ERROR_DEPTH = 15;

        public static string GetAllErrors(this Exception ex)
        {
            var curentDepth = 0;
            var errors = new StringBuilder();
            var curent = ex;

            while (curent != null)
            {
                if (++curentDepth > 15)
                {
                    errors.AppendLine("......");
                    break;
                }
                errors.AppendLine(curent.Message);
                curent = curent.InnerException;
            }
            return errors.Length > 0 ? errors.ToString() : "Ошибка";
        }

        public static string GetLastError(this Exception ex)
        {
            var curentDepth = 0;
            var curent = ex;
            while (curent?.InnerException != null)
            {
                if (++curentDepth > 15)
                    break;
                curent = curent.InnerException;
            }

            return $"{curent.Message}{(curentDepth == MAX_ERROR_DEPTH ? "\n......" : string.Empty)}" ?? "Ошибка";
        }
    }

}
