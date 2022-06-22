namespace TrfrtSbmt.Cdk;

using System;
using System.IO;

public class Utilities
{
    public static string GetDirectory(string directoryName)
    {
        var projectRelativePath = @"";

        // Get currently executing test project path
        var currentDirectory = Directory.GetCurrentDirectory();

        // Find the path to the target folder
        var directoryInfo = new DirectoryInfo(currentDirectory);
        do
        {
            directoryInfo = directoryInfo.Parent;
            if(directoryInfo == null) throw new Exception($"Drop directory could not be found {currentDirectory}.");
            var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
            if (projectDirectoryInfo.Exists)
            {

                var dropDirectoryInfo = new DirectoryInfo(Path.Combine(projectDirectoryInfo.FullName, directoryName));
                if (dropDirectoryInfo.Exists)
                {
                    return Path.Combine(projectDirectoryInfo.FullName, directoryName);
                }
            }
        }
        while (directoryInfo.Parent != null);

        throw new Exception($"Drop directory could not be found {currentDirectory}.");
    }
}
