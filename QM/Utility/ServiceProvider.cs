using System.IO;
using System.Net;

namespace QM
{
    /// <summary>
    /// Provides easy access to crucial elements of the application (e.g. ApplicationSettings).
    /// </summary>
    public static class ServiceProvider
    {
        public static ApplicationSettings GetApplicationSettings { get; set; }

        public static bool CheckProgramFilesConsistency()
        {
            if (Directory.Exists(GetApplicationSettings.LogFolderPath) 
                && Directory.Exists(GetApplicationSettings.TestStorageFolderPath)
                && Directory.Exists(GetApplicationSettings.ServerContentFolderPath)
                && Directory.Exists(GetApplicationSettings.DefaultSaveFolderPath)
                && Directory.Exists(Path.Combine(GetApplicationSettings.ServerContentFolderPath, "Json")))
            {
                if (File.Exists(Path.Combine(GetApplicationSettings.ServerContentFolderPath,"index.html")))
                {
                    ///Detete old temporary .json files
                    try
                    {
                        string[] files = Directory.GetFiles(Path.Combine(GetApplicationSettings.ServerContentFolderPath, "Json"));
                        foreach (var oldDataFile in files)
                        {
                            if (oldDataFile.StartsWith("data"))
                            {
                                File.Delete(oldDataFile);
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public static bool CheckMinRequirements()
        {
            if (!HttpListener.IsSupported)
            {
                return false;
            }

            return true;
        }

        static ServiceProvider()
        {
            GetApplicationSettings = new ApplicationSettings();
            GetApplicationSettings = GetApplicationSettings.LoadSettings();
        }
    }
}