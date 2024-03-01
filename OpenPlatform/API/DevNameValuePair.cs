using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVCPlayWithMe.OpenPlatform.API
{
    /// <summary>
    /// Cặp name/value
    /// </summary>
    public class DevNameValuePair
    {
        public string name { get; set; }
        public string value { get; set; }

        public DevNameValuePair(string inputName, string inputValue)
        {
            name = inputName;
            value = inputValue;
        }

        /// <summary>
        /// Từ danh sách name/value sinh ra query string
        /// </summary>
        /// <param name="ls">danh sách name/value</param>
        /// <returns>query string. Ex: ?name1=value1&name2=value2</returns>
        static public string GetQueryString(List<DevNameValuePair> ls)
        {
            if (ls == null)
                return string.Empty;

            int num = ls.Count();
            if (num == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < num; i++)
            {
                if(i == 0)
                    sb.Append("?" + ls[i].name + "=" + ls[i].value);
                else
                    sb.Append("&" + ls[i].name + "=" + ls[i].value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Từ danh sách name/value sinh ra query string
        /// </summary>
        /// <param name="ls">danh sách name/value</param>
        /// <returns>query string. Ex: &name1=value1&name2=value2</returns>
        static public string GetQueryStringWithAndPrefix(List<DevNameValuePair> ls)
        {
            if(ls == null)
                return string.Empty;

            int num = ls.Count();
            if (num == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                    sb.Append("&" + ls[i].name + "=" + ls[i].value);
            }
            return sb.ToString();
        }
    }
}
