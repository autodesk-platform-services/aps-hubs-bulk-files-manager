
using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Uploader_Electron.Models
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

        [Column("Error")] public string Heading { get; }
        [Column("Message")] public string Data { get; }
        [Column("Details")] public string Details { get; }
    }
}
