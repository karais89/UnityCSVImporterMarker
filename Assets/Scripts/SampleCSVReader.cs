using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCSVReader : MonoBehaviour 
{
	// sample.csv를 클래스화 시킴
	private class SampleCSVData
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int ItemCount { get; set; }
		public int Price { get; set; }
		public float BonusRate { get; set; }
		public bool IsUnique { get; set; }
	}
	
	// Use this for initialization
	void Start () 
	{
		ReadCSV();
	}

	private void ReadCSV()
	{
		// Sample.csv 파일 읽기
		TextAsset asset = Resources.Load<TextAsset>("Sample");
//		Debug.Log(asset.text);
		
		// Sample.csv 파일 파싱
		// unity csv reader
		var data = CSVReader.Read("Sample");
		for (int i = 0; i < data.Count; i++)
		{
			Debug.Log(data[i]["Id"] + " " + data[i]["Name"] + " " + data[i]["ItemCount"] + " " + data[i]["Price"] + " " + data[i]["BonusRate"] + " " + data[i]["IsUnique"]);
		}
	}
}
