# UnityCSVImporterMarker

유니티에서 CSV 파일을 읽고, 쓰기 쉽게 변환해 주도록 만들어 주기

- Unity 2017.3.0f3

## 목표

csv 파일을 읽고, 자동으로 클래스 맵핑 해주는 툴 만들기.

1. csv viwer 만들기(의미는 없지만 테스트 용도)
2. csv 데이터를 읽어와 형을 자동으로 판단 해주는 툴(아래 자동 생성을 위해 필요)
2. csv template를 이용해서 자동으로 class를 만들어주기

## 시작

1. csv 파일을 읽고, 데이터를 다룰 수 있는가?

## 생각

엑셀의 경우 바이너리 파일이라 형상 관리 측면에서 좋지 않다고 한다.

게임에서 다루는 데이터 포맷에 대한 선택. 가장 일반적인 방법을 사용하는게 좋다.

- json
- xml
- csv
- 자체 텍스트 포맷

간단한 데이터들은 csv로 다루는게 가장 편할 것 같다.

### 자료형 판단 방법

자료형 판단 방법. 헤더 아래의 row를 읽는다.

csv를 파서 한 결과인 해당 row 데이터는 무조건 string 값이다. 해당 string 값에서 자료형을 판단한다.

간단한 트릭을 사용

https://stackoverflow.com/questions/606365/c-sharp-doubt-finding-the-datatype/606381#606381

```
object ParseString(string str)
{
    int intValue;
    double doubleValue;
    char charValue;
    bool boolValue;

    // Place checks higher if if-else statement to give higher priority to type.
    if (int.TryParse(str, out intValue))
        return intValue;
    else if (double.TryParse(str, out doubleValue))
        return doubleValue;
    else if (char.TryParse(str, out charValue))
        return charValue;
    else if (bool.TryParse(str, out boolValue))
        return boolValue;

    return null;
}
```

## 참조

- [github/unityboot/assets/Scripts/Util/CsvParser](https://github.com/YacL/unityboot)
- 유니티 게임 제작 입문
- [클리커 게임 함께 만들기 12](http://blog.naver.com/moibios/220740010492)
- [lightweight-csv-reader-for-unity](https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity)
- [github/unity-excel-importer-marker](https://github.com/tsubaki/Unity-Excel-Importer-Maker)
- [csv2table](https://www.assetstore.unity3d.com/kr/#!/content/36443)