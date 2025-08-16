using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.API.LazadaAPI
{
    public class CommonLazadaAPI
    {
        public static string FormatDateTimeWithOffset(DateTime date)
        {
            string raw = date.ToString("yyyy-MM-ddTHH:mm:sszzz");
            return raw.Remove(raw.Length - 3, 1); // Xóa dấu ':' trong offset
        }
    }
}
