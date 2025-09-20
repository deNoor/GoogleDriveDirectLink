using System;
using System.Windows;
using static GoogleDriveDirectLink.DriveUrlEditor;

namespace GoogleDriveDirectLink;

internal class Program
{
    private static void Main()
    {
        if (!TryReplaceLinkInClipboard())
        {
            Console.ReadKey(); // Let user read console output.
        }
    }

    private static bool TryReplaceLinkInClipboard()
    {
        using var staThread = new StaThreadProvider();
        var url = staThread.Run(Clipboard.GetText);
        if (string.IsNullOrWhiteSpace(url))
        {
            Console.WriteLine("There is no url in clipboard.");
            return false;
        }
        if (!TryConvertToDirectLink(url, out var directLink))
        {
            Console.WriteLine($"Failed to convert {url}");
            return false;
        }
        staThread.Run(() => Clipboard.SetText(directLink));
        return true;
    }
}
