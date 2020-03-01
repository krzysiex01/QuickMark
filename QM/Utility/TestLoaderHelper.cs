using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QM
{
    /// <summary>
    /// Provides a list of test files stored in test storage folder.
    /// </summary>
    public class TestLoaderHelper
    {
        public List<TestDataPreview> TestDataPreviews { get; set; }

        public TestLoaderHelper()
        {
            TestDataPreviews = new List<TestDataPreview>();
        }

        /// <summary>
        /// Scans directory and update the list of found test files.
        /// </summary>
        public void RefreshPreviewsList()
        {
            TestDataPreviews = new List<TestDataPreview>();

            if (Directory.Exists(ServiceProvider.GetApplicationSettings.TestStorageFolderPath))
            {
                string[] files = Directory.GetFiles(ServiceProvider.GetApplicationSettings.TestStorageFolderPath).Where(file => Path.GetExtension(file) == ".qm").ToArray();
                foreach (string file in files)
                {
                    TestDataPreviews.Add(new TestDataPreview()
                    {
                        FullPath = file,
                        Name = Path.GetFileNameWithoutExtension(file),
                        LastAccessTime = File.GetLastAccessTime(file),
                        CreationTime = File.GetCreationTime(file),
                    });
                }
            }
        }
    }

    /// <summary>
    /// Contains basic information about file. Used by TestLoaderHelper class.
    /// </summary>
    public class TestDataPreview
    {
        public DateTime LastAccessTime { get; set; }
        public DateTime CreationTime { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
    }
}
