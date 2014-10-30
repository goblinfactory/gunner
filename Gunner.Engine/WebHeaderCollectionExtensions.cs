using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public static class WebHeaderCollectionExtensions
    {
        public static string ToDisplayText(this WebHeaderCollection headers)
        {
            var sb= new StringBuilder();
            foreach (HttpRequestHeader rh in headers)
            {
                sb.Append(string.Format("{0}:{1},", rh.ToString(),headers[rh]));
            }
            return sb.ToString().TrimEnd(new char[] {','});
        }
    }
}
