using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Engine
{
    public static class StringExtensions
    {
        public static string First(this string src, int len)
        {
            if (string.IsNullOrWhiteSpace(src)) return "";
            return src.Substring(0, len >= src.Length ? src.Length : len);
        }
    }
}
