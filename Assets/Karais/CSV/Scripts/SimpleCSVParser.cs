namespace Karais.CSV
{
    /*
     * 단순히 csv를 읽고 파싱하기 위해 만든 클래스
     * 유니티 게임 제작 입문 서적 참조
     * step
     * 1. 텍스트 데이터 전체를 통째로 읽어온다.
     * 2. 행 단위로 배열 lines에 저장해 간다.
     * 3. 각 행을 워드 단위로 나누어 각 워드를 배열 words에 저장해 간다.
     */

    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class SimpleCSVParser
    {
        // new line regex
        private static readonly string LINE_SPLIT_REGEX = @"\r\n|\n\r|\n|\r";
        // https://stackoverflow.com/questions/1757065/java-splitting-a-comma-separated-string-but-ignoring-commas-in-quotes
        // 쌍 따옴표 안에 콤마는 무시한다.
        private static readonly string WORD_SPLIT_REGEX = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; 
        // 쌍 따옴표 제거
        private static readonly char[] TRIM_CHARS = { '\"' };
        
        // 파싱한 데이터를 2차원 List의 string 형태로 반환 한다.
        public static List<List<string>> Parser(string text)
        {
            List<List<string>> allLines = new List<List<string>>();
            // 개행 코드 마다 분할해서 문자열 배열에 집어 넣는다.
            string[] lines = Regex.Split(text, LINE_SPLIT_REGEX);
            for (int i = 0; i < lines.Length; i++)
            {
                // 행 내의 워드를 배열에 저장 한다.
                string[] words = Regex.Split(lines[i], WORD_SPLIT_REGEX);
                List<string> cachedWords = new List<string>();
                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];
                    // 쌍 따옴표를 제거 한다.
                    word = word.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    cachedWords.Add(word);
                }

                allLines.Add(cachedWords);
            }

            return allLines;
        }
    }
}