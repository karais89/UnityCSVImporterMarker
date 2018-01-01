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
    private static readonly char WORD_SPLIT = ',';
    
    // 파싱한 데이터를 2차원 List의 string 형태로 반환 한다.
    public static List<List<string>> Parser(string text)
    {
        List<List<string>> allLines = new List<List<string>>();
        // 개행 코드 마다 분할해서 문자열 배열에 집어 넣는다.
        string[] lines = Regex.Split(text, LINE_SPLIT_REGEX);

        // header를 제외하고 저장한다.
        for (int i = 1; i < lines.Length; i++)
        {
            // 행 내의 워드를 배열에 저장 한다.
            string[] words = lines[i].Split(WORD_SPLIT);
            List<string> cachedWords = new List<string>();
            for (int j = 0; j < words.Length; j++)
            {
                cachedWords.Add(words[j]);
            }
            allLines.Add(cachedWords);
        }
        return allLines;
    }
}