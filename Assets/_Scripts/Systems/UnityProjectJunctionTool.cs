#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public static class UnityProjectJunctionTool
{
    [MenuItem("Unity Project Junction/Create Junction Unity Project")]
    private static void CreateJunctionUnityProject()
    {
        DirectoryInfo projectDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

        string junctionDirectory = $"{projectDirectoryInfo.Name}-Junction";
        string junctionPath = Path.Combine(projectDirectoryInfo.Parent.FullName, junctionDirectory);

        if (Directory.Exists(junctionPath))
        {
            if (EditorUtility.DisplayDialog("Junction already exists",
                    $"Junction project directory already exists at {junctionPath}.\n\nWould you like to create another junction project?",
                    "Cancel", "Yes, create another"))
            {
                return;
            }
            else
            {
                int junctionIndex = 1;

                do
                {
                    junctionDirectory = $"{projectDirectoryInfo.Name}-Junction-{junctionIndex}";
                    junctionPath = Path.Combine(projectDirectoryInfo.Parent.FullName, junctionDirectory);

                    junctionIndex++;
                } while (Directory.Exists(junctionPath));
            }
        }

        DirectoryInfo junctionDirectoryInfo = Directory.CreateDirectory(junctionPath);

        string linkAssets = Path.Combine(junctionDirectoryInfo.FullName, "Assets");
        string linkProjectSettings = Path.Combine(junctionDirectoryInfo.FullName, "ProjectSettings");
        string linkPackages = Path.Combine(junctionDirectoryInfo.FullName, "Packages");

        string targetAssets = Path.Combine(projectDirectoryInfo.FullName, "Assets");
        string targetProjectSettings = Path.Combine(projectDirectoryInfo.FullName, "ProjectSettings");
        string targetPackages = Path.Combine(projectDirectoryInfo.FullName, "Packages");

        CreateJunction(linkAssets, targetAssets);
        CreateJunction(linkProjectSettings, targetProjectSettings);
        CreateJunction(linkPackages, targetPackages);

        EditorUtility.DisplayDialog("Complete",
            $"Created junction project at {junctionPath}.\n\nYou may now open this new project in a separate Unity Editor instance.",
            "Ok");

        EditorUtility.RevealInFinder(junctionDirectoryInfo.FullName);
    }

    private static void CreateJunction(string link, string target)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            LinkWinOS(link, target);
        }
        else if (Application.platform == RuntimePlatform.OSXEditor ||
                 Application.platform == RuntimePlatform.LinuxEditor)
        {
            LinkMacLinOS(link, target);
        }
    }

    public static void LinkWinOS(string link, string target)
    {
        string command = $"/C mklink /J \"{link}\" \"{target}\"";
        Process.Start("cmd.exe", command);
    }

    public static void LinkMacLinOS(string link, string target)
    {
        string command = $"ln -s \"{target}\" \"{link}\"";

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + command.Replace("\"", "\"\"") + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            }
        };

        proc.Start();
        proc.WaitForExit();
    }
}
#endif