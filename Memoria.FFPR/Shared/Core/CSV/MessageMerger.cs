using System;
using System.Collections.Generic;
using System.IO;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FFPR.Core;

public sealed class MessageMerger
{
    private const Char Separator = '\t';
    private readonly OrderedDictionary<String, String> _rows = new();

    public MessageMerger(String txtContent)
    {
        if (txtContent is null) throw new ArgumentNullException(nameof(txtContent));
        if (txtContent == String.Empty) throw new ArgumentException(nameof(txtContent));

        using (var sr = new StringReader(txtContent))
        {
            while (TryReadContent(sr, out String key, out String value))
                _rows.AddOrUpdate(key, value);
        }
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
    
    public String BuildContent()
    {
        using (StringWriter sw = new StringWriter())
        {
            foreach ((String key, String value) in _rows)
                sw.WriteLine($"{key}{Separator}{value}");

            sw.Flush();
            return sw.ToString();
        }
    }
    
    private void MergeFile(StreamReader sr)
    {
        while (TryReadContent(sr, out String key, out String value))
        {
            if (_rows.TryReplace(key, value, out var previousValue))
            {
                if (previousValue != value)
                    ModComponent.Log.LogInfo($"[Mod] Changed message [{key}]: [{previousValue}] -> [{value}]");
                continue;
            }

            if (_rows.TryAdd(key, value))
            {
                ModComponent.Log.LogInfo($"[Mod] Added new message [{key}]: [{value}]");
                continue;
            }
            
            throw new InvalidOperationException("Collection is out of sync.");
        }
    }
    
    private Boolean TryReadContent(TextReader reader, out String key, out String value)
    {
        while (true)
        {
            String line = reader.ReadLine();
            if (line is null)
                break;

            if (String.IsNullOrWhiteSpace(line))
                continue;
            
            String[] parts = line.Split(Separator);
            switch (parts.Length)
            {
                case 1:
                {
                    key = parts[0];
                    value = String.Empty;
                    break;
                }
                case 2:
                {
                    key = parts[0];
                    value = parts[1];
                    break;
                }
                default:
                {
                    throw new FormatException($"Unexpected line in .txt file: {line}");
                }
            }

            return true;
        }

        key = null;
        value = null;
        return false;
    }
}