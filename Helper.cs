using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anoteitor {
    public static class Helper {
        public static List<int> GetIndexes(string pText, string pSearchText, bool pCaseSensitive) {
            var Indexes = new List<int>();

            var eStringComparison = pCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

            var StartIndex = 0;

            while (true) {
                var Index = pText.IndexOf(pSearchText, StartIndex, eStringComparison);
                if (Index == -1) break;
                Indexes.Add(Index);
                StartIndex = Index + pSearchText.Length;
            }

            return Indexes;
        }

        public static string ExtractDateFromFileName(string fileName)
        {
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(fileName, @"\d{2}-\d{2}-\d{4}");
            return match.Success ? match.Value : "Data desconhecida";
        }
    }
}
