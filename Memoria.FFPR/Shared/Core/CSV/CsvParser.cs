using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Memoria.FFPR.Core;

public sealed class CsvParser
{
    public const Char Separator = ',';

    public readonly String[] ColumnNames;
    public readonly Dictionary<String, Int32> ColumnNameIndices;
    public readonly List<String[]> Rows;
    public readonly Dictionary<Int32, Int32> RowIndices;
    
    public CsvParser(String csvContent)
    {
        if (csvContent is null) throw new ArgumentNullException(nameof(csvContent));
        if (csvContent == String.Empty) throw new ArgumentException(nameof(csvContent));

        using (var sr = new StringReader(csvContent))
        {
            if (!TryReadContent(sr, out String[] parts))
                parts = Array.Empty<String>();
            else if (parts[0] != "id")
                throw new NotSupportedException($"Not supported CSV-format. Unexpected first column: [{parts[0]}]. Expected: [id]");

            HashSet<String> processedColumns = new();
            ColumnNames = parts;
            ColumnNameIndices = new(ColumnNames.Length);
            for (Int32 i = 0; i < ColumnNames.Length; i++)
            {
                String columnName = ColumnNames[i];
                if (!processedColumns.Add(columnName))
                    throw new FormatException($"The header contains several columns with the same name: [{columnName}]");
                
                ColumnNameIndices.Add(columnName, i);
            }

            Rows = new List<String[]>();
            RowIndices = new Dictionary<Int32, Int32>();
            while (TryReadContent(sr, out parts))
            {
                Int32 id = Int32.Parse(parts[0], CultureInfo.InvariantCulture);
                AddNewRow(id, parts);
            }
        }
    }

    public static Boolean TryReadContent(TextReader reader, out String[] parts)
    {
        while (true)
        {
            String line = reader.ReadLine();
            if (line is null)
            {
                parts = null;
                return false;
            }

            if (!String.IsNullOrWhiteSpace(line))
            {
                parts = line.Split(Separator);
                return true;
            }
        }
    }

    public void AddNewRow(Int32 id, String[] row)
    {
        RowIndices.Add(id, Rows.Count);
        Rows.Add(row);
    }
    
    public String GetValue(Int32 conditionId, String columnName)
    {
        if (RowIndices.TryGetValue(conditionId, out Int32 rowIndex))
        {
            if (ColumnNameIndices.TryGetValue(columnName, out Int32 columnIndex))
                return Rows[rowIndex][columnIndex];
        }

        return null;
    }
}