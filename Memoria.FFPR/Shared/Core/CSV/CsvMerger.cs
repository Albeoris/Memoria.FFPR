﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FFPR.Core;

public sealed class CsvMerger
{
    private readonly CsvParser _parser;
    private readonly HashSet<Int32> _removedRows = new HashSet<Int32>();

    public CsvMerger(String csvContent)
    {
        if (csvContent is null) throw new ArgumentNullException(nameof(csvContent));
        if (csvContent == String.Empty) throw new ArgumentException(nameof(csvContent));

        _parser = new CsvParser(csvContent);
    }

    public void MergeFiles(IReadOnlyList<String> filePaths)
    {
        if (filePaths is null) throw new ArgumentNullException(nameof(filePaths));

        foreach (String fullPath in filePaths)
        {
            try
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[Mod] Merging data from {shortPath}");
                
                using (StreamReader sr = File.OpenText(fullPath))
                    MergeFile(sr);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to merge data from {fullPath}", nameof(fullPath), ex);
            }
        }
    }

    private void MergeFile(StreamReader sr)
    {
        if (!CsvParser.TryReadContent(sr, out String[] parts))
            return;
        
        if (parts[0] != "id")
            throw new NotSupportedException($"Not supported CSV-format. Unexpected first column: [{parts[0]}]. Expected: [id]");

        StringBuilder sb = new();

        String[] columnNames = parts;
        Int32[] columnIndices = new Int32[columnNames.Length];
        HashSet<String> processedColumns = new HashSet<String>();
        for (Int32 i = 0; i < columnNames.Length; i++)
        {
            String columnName = columnNames[i];
            if (!processedColumns.Add(columnName))
                throw new FormatException($"The header contains several columns with the same name: [{columnName}]"); 
            
            if (!_parser.ColumnNameIndices.TryGetValue(columnName, out Int32 columnIndex))
                throw new FormatException($"Cannot find index of [{columnName}] column in the full CSV-file.");

            columnIndices[i] = columnIndex;
        }

        while (CsvParser.TryReadContent(sr, out parts))
        {
            Boolean toRemove = false;
            Int32 id = Int32.Parse(parts[0], CultureInfo.InvariantCulture);
            if (id < 0)
            {
                toRemove = true;
                id *= -1;
            }
            
            if (!_parser.RowIndices.TryGetValue(id, out var rowIndex))
            {
                if (toRemove)
                {
                    ModComponent.Log.LogWarning($"[Mod] Cannot find row with id [{id}] to remove it.");
                    continue;
                }
                
                if (parts.Length != _parser.ColumnNames.Length)
                    throw new FormatException($"Cannot add row with id [{id}]. Expected {_parser.ColumnNames.Length} columns, but there is {parts.Length}.");
                
                String[] row = new String[_parser.ColumnNames.Length];
                for (Int32 i = 0; i < row.Length; i++)
                {
                    Int32 columnIndex = columnIndices[i];
                    row[columnIndex] = parts[i];
                }

                _parser.AddNewRow(id, row);
                ModComponent.Log.LogInfo($"[Mod] Added new row: {String.Join(",", row)}.");
                continue;
            }

            if (toRemove)
            {
                if (_removedRows.Add(rowIndex))
                {
                    String[] row = _parser.Rows[rowIndex];
                    ModComponent.Log.LogInfo($"[Mod] Removed existing row [{id}]. {String.Join(",", row)}.");
                }

                continue;
            }

            for (Int32 i = 1; i < parts.Length; i++)
            {
                Int32 columnIndex = columnIndices[i];
                String columnName = columnNames[i];
                String[] row = _parser.Rows[rowIndex];
                String oldValue = row[columnIndex];
                String newValue = parts[i];
                if (oldValue != newValue)
                {
                    row[columnIndex] = newValue;
                    sb.Append($" {columnName} ({oldValue} -> {newValue})");
                }
            }

            if (sb.Length > 0)
            {
                ModComponent.Log.LogInfo($"[Mod] Changed row [{id}]. {sb.ToString()}");
                sb.Clear();
            }
        }
    }

    public String BuildContent()
    {
        using (StringWriter sw = new StringWriter())
        {
            Boolean newLine = true;
            foreach (String columnName in _parser.ColumnNames)
            {
                if (!newLine)
                    sw.Write(CsvParser.Separator);
                
                sw.Write(columnName);
                newLine = false;
            }

            sw.WriteLine();
            newLine = true;
            
            for (int i = 0; i < _parser.Rows.Count; i++)
            {
                if (_removedRows.Contains(i))
                    continue;

                String[] row = _parser.Rows[i];
                foreach (String data in row)
                {
                    if (!newLine)
                        sw.Write(CsvParser.Separator);
                    sw.Write(data);
                    newLine = false;
                }

                sw.WriteLine();
                newLine = true;
            }

            sw.Flush();
            return sw.ToString();
        }
    }
}