using System;
using System.Text.RegularExpressions;

namespace GoogleDriveDirectLink;

public class DriveUrlEditor
{
    private const string DirectPathAndQuery = "uc?id=";

    private static readonly Regex _sharingUrlTemplate = new(
        """
        ^
        (?'DriveHost'https:\/\/drive\.google\.com\/) # https://drive.google.com/
        (?:file\/d\/)                                # file/d/ - can be changes by Google in the future
        (?'FileId'.+)                                # your file Id - 33 symbols at the moment
        \/(\S+)                                      # /view?usp=sharing - ending
        $
        """,
        RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(50));

    public static bool TryConvertToDirectLink(string url, out string directLink)
    {
        directLink = string.Empty;
        var match = _sharingUrlTemplate.Match(url);
        if (!match.Success)
        {
            return false;
        }
        directLink = match.Groups["DriveHost"].Value + DirectPathAndQuery + match.Groups["FileId"].Value;
        return true;
    }
}
