// Star Forge desktop launcher.
// The game HTML is embedded inside this .exe. On launch it is unpacked to
// %LOCALAPPDATA%\StarForge and opened in a chromeless Edge "app" window
// with its own profile, so it looks like a normal desktop program and
// save data persists between runs.
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

[assembly: AssemblyTitle("Star Forge")]
[assembly: AssemblyProduct("Star Forge")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

static class Launcher
{
    [STAThread]
    static void Main()
    {
        try
        {
            string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StarForge");
            string gameDir = Path.Combine(root, "game");
            string profileDir = Path.Combine(root, "profile");
            Directory.CreateDirectory(gameDir);
            Directory.CreateDirectory(profileDir);

            // Unpack the embedded game (overwrite so updates take effect).
            string htmlPath = Path.Combine(gameDir, "star-forge.html");
            using (Stream res = Assembly.GetExecutingAssembly().GetManifestResourceStream("star-forge.html"))
            using (FileStream fs = File.Create(htmlPath))
                res.CopyTo(fs);

            string url = new Uri(htmlPath).AbsoluteUri;
            string edge = FindEdge();

            if (edge != null)
            {
                string args = string.Format(
                    "--app=\"{0}\" --user-data-dir=\"{1}\" --no-first-run --no-default-browser-check --window-size=1100,720",
                    url, profileDir);
                Process.Start(new ProcessStartInfo(edge, args) { UseShellExecute = false });
            }
            else
            {
                // No Edge: fall back to the default browser.
                Process.Start(new ProcessStartInfo(htmlPath) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Star Forge could not start:\n\n" + ex.Message, "Star Forge",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    static string FindEdge()
    {
        string[] candidates = {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft\Edge\Application\msedge.exe"),
        };
        foreach (string c in candidates)
            if (File.Exists(c)) return c;
        return null;
    }
}
