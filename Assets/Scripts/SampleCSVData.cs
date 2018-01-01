using System.Collections.Generic;
using UnityEngine;

// TODO : Auto Class Generater
public class SampleCSVData
{
    public class CsvRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ItemCount { get; set; }
        public int Price { get; set; }
        public float BonusRate { get; set; }
        public bool IsUnique { get; set; }
    }

    public List<CsvRow> rows = new List<CsvRow>();

    public void Load(TextAsset csv)
    {
        rows.Clear();
        
        // TODO : csv parser
        
        // TODO : Add List row Data
        CsvRow row = new CsvRow();
        rows.Add(row);
    }
}
