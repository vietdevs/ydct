using Abp.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newPSG.PMS
{

    /// <summary>
    /// Return của các Service 
    /// </summary>
    /// <typeparam name="T">Kiểu giá trị muốn trả ra </typeparam>
    public class ServiceReturn<T>
    {
        public string Message { get; set; }
        public T Value { get; set; }
        public LogSeverity Severity { get; set; }

        public ServiceReturn(string mess, LogSeverity severity, T value)
        {
            this.Message = mess;
            this.Severity = severity;
            this.Value = value;
        }
        public ServiceReturn(string mess, LogSeverity severity)
        {
            this.Message = mess;
            this.Severity = severity;
        }
        public ServiceReturn(T value)
        {
            this.Value = value;
            this.Severity = LogSeverity.Info;
        }
        public ServiceReturn(string mess)
        {
            this.Message = mess;
            this.Severity = LogSeverity.Info;
        }
        public ServiceReturn()
        {
            this.Message = "Xử lý thành công!";
            this.Severity = LogSeverity.Info;
        }
    }
}