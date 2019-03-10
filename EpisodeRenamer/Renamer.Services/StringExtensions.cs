using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Renamer.Services {
    public static class StringExtensions {

        public static bool IsAnArticle(this string str) {
            string[] articles = { "the", "a" };
            return articles.Contains(str.ToLower());
        }

        public static string RemoveFirstWordArticlesFromTitle(this string title) {
            string[] splits = title.Split(' ');
            if (splits[0].IsAnArticle() && splits.Length > 1) {
                title = title.Substring(splits[0].Length + 1);
            }
            return title;
        }

        public static string ReplaceSpaces(this string str, char replace = '.') {
            return str.Replace(' ', replace);
        }

        public static bool IsNumeric(this string source) {
            return int.TryParse(source, out int i);
        }

        public static int ToInt(this string source) {
            int result = 0;
            if (source.IsNumeric()) {
                bool b = int.TryParse(source, out result);
            }
            return result;
        }
    }
}
