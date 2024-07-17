using EPPlus.Core.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUploaderUtils.Models
{
    public class ErrorMessage
    {
        public ErrorMessage(string heading, string data, string details)
        {
            Heading = heading;
            Data = data;
            Details = details;
        }

        public ErrorMessage(string heading, Exception ex)
        {
            Heading = heading;
            Data = ex.Message;
            Details = ex.StackTrace?? "No Stack";
        }

        [ExcelTableColumn("Error", true)] public string Heading { get; }
        [ExcelTableColumn("Message", true)] public string Data { get; }
        [ExcelTableColumn("Details", true)] public string Details { get; }
    }
}
