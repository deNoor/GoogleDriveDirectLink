using System;
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
        using var clipboard = new ClipboardAccessor();
        var url = clipboard.GetText();
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

        clipboard.SetText(directLink);
        return true;
    }
}
