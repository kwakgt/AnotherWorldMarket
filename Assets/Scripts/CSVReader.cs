using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset; //CSV 파일 가져오기

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);  //data.text 문장을 한 줄 단위로 나누기

        if (lines.Length <= 1) return list;                 //문장의 길이가 한 줄 이하이면 문장이 없거나, 헤더만 있는 경우이니 종료

        var header = Regex.Split(lines[0], SPLIT_RE);       //헤더 저장
        for (var i = 1; i < lines.Length; i++)              //한 줄씩 순회
        { 
            var values = Regex.Split(lines[i], SPLIT_RE);   //한 줄을 각각의 단어로 나누기
            if (values.Length == 0 || values[0] == "") continue;    //한 줄에 아무것도 없으면 건너뛰기 

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];

                //String.TrimStart(Char[])  : 현재 문자열에서 배열에 지정된 문자 집합의 선행 항목을 모두 제거합니다.
                //String.TrimEnd(Char[])    : 현재 문자열에서 배열에 지정된 문자 집합의 후행 항목을 모두 제거합니다
                //String.Replace()          : 현재 문자열에서 발견되는 지정된 유니코드 문자 또는 String을 모두 지정된 다른 유니코드 문자 또는 String으로 바꾼 새 문자열을 반환합니다.
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", ""); //value의 선행,후행 항목을 제거하고 \\을 빈 문자로 치환한다.
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))         //숫자의 문자열 표현을 해당하는 32비트 부호 있는 정수로 변환합니다. 반환 값은 작업의 성공 여부를 나타냅니다.
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))  //숫자의 문자열 표현을 해당하는 단정밀도 부동 소수점 숫자로 변환합니다. 반환 값은 변환의 성공 여부를 나타냅니다.
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}
