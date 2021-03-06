﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.NetworkInformation;
using System.Windows;

namespace QM
{
    /// <summary>
    /// Stores all application settings.
    /// </summary>
    [Serializable]
    public class ApplicationSettings
    {
        /// <summary>
        /// Location of server the folder containing all server files - main html page, JavaScript and CSS files
        /// and Json files created via TestManager class - <see cref="TestManager.GenerateTests"/>
        /// </summary>
        public string ServerContentFolderPath { get; set; }
        /// <summary>
        /// Location of the backup folder for created tests.
        /// </summary>
        public string TestStorageFolderPath { get; set; }
        /// <summary>
        /// Location of the log folder.
        /// </summary>
        public string LogFolderPath { get; set; }
        /// <summary>
        /// For future features.
        /// </summary>
        public string DefaultSaveFolderPath { get; set; }
        /// <summary>
        /// Prefixes for HttpListener class.
        /// <see cref="System.Net.HttpListener.Prefixes"/>
        /// </summary>
        public string[] Prefixes { get; set; }
        /// <summary>
        /// AutoSave feature - save format information.
        /// </summary>
        public bool SaveAsCSV { get; set; }
        /// <summary>
        /// AutoSave feature - save format information.
        /// </summary>
        public bool SaveAsXLSL { get; set; }
        /// <summary>
        /// AutoSave feature - IsEnabled
        /// </summary>
        public bool AutoSave { get; set; }
        /// <summary>
        /// Is Fair Play System disabled.
        /// </summary>
        public bool IgnoreCheating { get; set; }
        /// <summary>
        /// Allow multiple records with same Name and Surname. <see cref="Record"/>
        /// </summary>
        public bool IgnoreDuplicate { get; set; }
        /// <summary>
        /// Determines if information about recived points should be send back to the client.
        /// </summary>
        public bool HideResult { get; set; }
        /// <summary>
        /// Maximum number of groups that can be generated by TestManager class - <see cref="AutoGroupsHelper.ShuffleTest"/>
        /// </summary>
        public int GroupsLimit { get; set; }
        /// <summary>
        /// Network interface type of current device.
        /// Required to get current IPv4 address - <see cref="Server.GetLocalIPv4"/>
        /// </summary>
        public NetworkInterfaceType NetworkInterfaceType { get; set; }
        /// <summary>
        /// Determines the way of creating new groups for tests with AutoGroups option set to true.
        /// </summary>
        /// <see cref="AutoGroupsHelper.ShuffleTest(bool, bool)"/>
        public bool MixQuestions { get; set; }
        /// <summary>
        /// Determines the way of creating new groups for tests with AutoGroups option set to true.
        /// </summary>
        /// <see cref="AutoGroupsHelper.ShuffleTest(bool, bool)"/>
        public bool MixAnswers { get; set; }

        public ApplicationSettings()
        {
            Prefixes = new string[] { "http://localhost:51111/" , "http://+:80/" };
            NetworkInterfaceType = NetworkInterfaceType.Wireless80211;
            SaveAsCSV = false;
            SaveAsXLSL = true;
            ServerContentFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Resources" ,"Server");
            DefaultSaveFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Saved Records");
            LogFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Logs");
            TestStorageFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Test Storage");
            AutoSave = true;
            IgnoreCheating = false;
            IgnoreDuplicate = false;
            HideResult = false;
            GroupsLimit = 10;
            MixAnswers = true;
            MixQuestions = true;
        }

        public ApplicationSettings LoadSettings()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.bin")))
            {
                FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.bin"), FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                object settingsDeserialized;
                try
                {
                    settingsDeserialized = formatter.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return this;
                }
                finally
                {
                    fs.Close();
                }
                return settingsDeserialized is ApplicationSettings ? (ApplicationSettings)settingsDeserialized : this;
            }
            else
            {
                return this;
            }
        }

        public void SaveSettings()
        {
            FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.bin"), FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            fs.Close();
        }
    }
}