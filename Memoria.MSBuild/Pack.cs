using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;
using Microsoft.Build.Framework;

namespace Memoria.MSBuild;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class Pack : ITask
{
    public IBuildEngine BuildEngine { get; set; } = null!;
    public ITaskHost HostObject { get; set; } = null!;

    [Required] public String PublishDirectory { get; set; } = null!;
    [Required] public String Configuration { get; set; } = null!;

    public bool Execute()
    {
        // if (!Debugger.IsAttached)
        //     Debugger.Launch();

        Stopwatch sw = Stopwatch.StartNew();
        using (ZipArchive zip = CreateZipArchive())
        {
            foreach (String filePath in EnumerateFilesToPack())
            {
                String entryName = Path.GetFileName(filePath);
                if (IsNotCurrentConfigurationFile(entryName))
                    continue;

                PackFile(zip, filePath, entryName);
            }
        }

        sw.Stop();

        BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"{Configuration} packed in {sw.Elapsed}", null, nameof(Pack), MessageImportance.High));
        return true;
    }

    private ZipArchive CreateZipArchive()
    {
        String zipName = $"{Configuration}_v{DateTime.Now:yyyy.MM.dd}.zip";
        String zipPath = Path.Combine(PublishDirectory, zipName);

        ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create, Encoding.UTF8);
        return zip;
    }

    private IEnumerable<String> EnumerateFilesToPack()
    {
        return Directory.EnumerateFiles(PublishDirectory, "*.dll");
    }

    private Boolean IsNotCurrentConfigurationFile(String fileName)
    {
        return fileName.StartsWith("Memoria.") && !fileName.EndsWith($"{Configuration}.dll");
    }

    private static void PackFile(ZipArchive zip, String filePath, String entryName)
    {
        using (FileStream input = File.OpenRead(filePath))
        {
            ZipArchiveEntry entry = zip.CreateEntry($"BepInEx/plugins/{entryName}", CompressionLevel.Optimal);
            using (Stream output = entry.Open())
                input.CopyTo(output);
        }
    }
}