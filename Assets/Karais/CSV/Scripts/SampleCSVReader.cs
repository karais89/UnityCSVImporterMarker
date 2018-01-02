namespace Karais.CSV
{
	/*
	 * 오직 테스트 용도
	 * Sample.csv 파일의 데이터 다루기
	 */
	using UnityEngine;

	public class SampleCSVReader : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{
			ReadCSV();
		}

		private void ReadCSV()
		{
			SampleCSVData data = new SampleCSVData(Resources.Load<TextAsset>("Sample"));

			for (int i = 0; i < data.rows.Count; i++)
			{
				Debug.LogFormat("{0}, {1}, {2}, {3}, {4}, {5}",
					data.rows[i].Id,
					data.rows[i].Name,
					data.rows[i].ItemCount,
					data.rows[i].Price,
					data.rows[i].BonusRate,
					data.rows[i].IsUnique);
			}
		}
	}
}