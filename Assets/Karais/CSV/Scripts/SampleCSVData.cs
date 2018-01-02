namespace Karais.CSV
{
    using System.Collections.Generic;
    using UnityEngine;
    
    // TODO : Auto Class Generater
    public class SampleCSVData
    {
        public class CSVRow
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ItemCount { get; set; }
            public int Price { get; set; }
            public float BonusRate { get; set; }
            public bool IsUnique { get; set; }
        }

        public List<CSVRow> rows = new List<CSVRow>();

        public SampleCSVData(TextAsset csv)
            : this(csv.text)
        {
            // empty
        }

        public SampleCSVData(string text)
        {
            rows.Clear();
            var parserData = SimpleCSVParser.Parser(text);

            // header(첫번째 행)를 제외하고 저장한다.
            for (int i = 1; i < parserData.Count; i++)
            {
                CSVRow row = new CSVRow();
                row.Id = int.Parse(parserData[i][0]);
                row.Name = parserData[i][1];
                row.ItemCount = int.Parse(parserData[i][2]);
                row.Price = int.Parse(parserData[i][3]);
                row.BonusRate = float.Parse(parserData[i][4]);
                row.IsUnique = bool.Parse(parserData[i][5]);
                rows.Add(row);
            }
        }
    }
}
