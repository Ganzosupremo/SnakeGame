using SnakeGame.Debuging;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

namespace SnakeGame.SaveAndLoadSystem
{
    public class FileDataHandler
    {
        private readonly string dataDirPath = "";
        private readonly string dataFileName = "";
        private readonly bool useEncryption = false;
        private readonly string codeWord = "Ma snake is very big";
        private readonly string backupExtension = ".bak";

        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
            this.useEncryption = useEncryption;
        }

        /// <summary>
        /// Loads the game data using a profileID
        /// </summary>
        /// <param name="profileID"></param>
        /// <param name="allowRestoreFromBackup"></param>
        /// <returns></returns>
        public GameData Load(string profileID, bool allowRestoreFromBackup = true)
        {
            // If the profile ID is null, return
            if (profileID == null)
            {
                return null;
            }

            // Use the Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
            GameData loadedData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    // Load the serialized data from the Json file
                    string dataToLoad = "";
                    using (FileStream stream = new(fullPath, FileMode.Open))
                    {
                        using StreamReader reader = new(stream);
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Optionally decrypt the data
                    if (useEncryption)
                    {
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    }

                    // Deserealized the Json file back to a C# class
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

                }
                catch (Exception e)
                {
                    // We only try to load from backup a single time, otherwise it could end in an infinite loop
                    if (allowRestoreFromBackup)
                    {
                        Debuger.LogWarning($"Failed to load file. Attempting to roll back. \n {e}");
                        bool rollBackSucceded = AttemptRollBack(fullPath);

                        if (rollBackSucceded)
                        {
                            // Try to load data again
                            loadedData = Load(profileID, false);
                        }
                    }
                    else
                    {
                        this.LogError($"Error ocurred when trying to load file from: {fullPath} and backup didn't work. \n {e}");
                    }
                }
            }
            return loadedData;
        }

        /// <summary>
        /// Loads the game data without using a profileID
        /// </summary>
        /// <param name="allowRestoreFromBackup"></param>
        /// <returns></returns>
        public GameData Load(bool allowRestoreFromBackup = true)
        {
            // Use the Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            GameData loadedData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    // Load the serialized data from the Json file
                    string dataToLoad = "";
                    using (FileStream stream = new(fullPath, FileMode.Open))
                    {
                        using StreamReader reader = new(stream);
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Optionally decrypt the data
                    if (useEncryption)
                        dataToLoad = EncryptDecrypt(dataToLoad);

                    // Deserealize the Json file back to a C# class
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    // We only try to load from backup a single time, otherwise it could end in an infinite loop
                    if (allowRestoreFromBackup)
                    {
                        this.LogWarning($"Failed to load file. Attempting to roll back. \n {e}");
                        bool rollBackSucceded = AttemptRollBack(fullPath);

                        if (rollBackSucceded)
                        {
                            // Load the data from backup just once
                            loadedData = Load(false);
                        }
                    }
                    else
                    {
                        this.LogError($"Error ocurred when trying to load file from: {fullPath} and backup didn't work. \n {e}");
                    }
                }
            }
            return loadedData;
        }

        /// <summary>
        /// Saves the data to disk using a profileID
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="profileID"></param>
        public void Save(GameData gameData, string profileID)
        {
            //If the profile ID is null, return
            if (profileID == null)
            {
                return;
            }

            //Use the Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
            string backupPath = fullPath + backupExtension;

            try
            {
                //Create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                //Serialize the C# game data into a Json
                string dataToStore = JsonUtility.ToJson(gameData, true);

                //Optionally use encryption on the Json file
                if (useEncryption)
                {
                    dataToStore = EncryptDecrypt(dataToStore);
                }

                //Write the serialized data to the Json file
                using (FileStream stream = new(fullPath, FileMode.Create))
                {
                    using StreamWriter writer = new(stream);
                    writer.Write(dataToStore);
                }

                // Verify the newly created data isn't corrupted
                GameData verifiedGameData = Load(profileID);

                // If the data can be verified back it up
                if (verifiedGameData != null)
                {
                    File.Copy(fullPath, backupPath, true);
                }
                // Otherwise, something went wrong, so throw an exception
                else
                {
                    throw new Exception("Data couldn't be verified and backep up.");
                }
            }
            catch (Exception e)
            {
                this.LogError($"Error while saving the file on: {fullPath}.\nException: {e}");
            }
        }

        /// <summary>
        /// Saves the data to disk without using a profileID
        /// </summary>
        /// <param name="gameData"></param>
        public void Save(GameData gameData)
        {
            // Use the Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            string backupPath = fullPath + backupExtension;

            try
            {
                //Create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                //Serialize the C# game data into a Json
                string dataToStore = JsonUtility.ToJson(gameData, true);

                //Optionally use encryption on the Json file
                if (useEncryption)
                {
                    dataToStore = EncryptDecrypt(dataToStore);
                }

                //Write the serialized data to the Json file
                using (FileStream stream = new(fullPath, FileMode.Create))
                {
                    using StreamWriter writer = new(stream);
                    writer.Write(dataToStore);
                }

                // Verify the newly created data isn't corrupted
                GameData verifiedGameData = Load();

                // If the data can be verified back it up
                if (verifiedGameData != null)
                {
                    File.Copy(fullPath, backupPath, true);
                }
                // Otherwise, something went wrong, so throw an exception
                else
                {
                    throw new Exception("Data couldn't be verified and backep up.");
                }
            }
            catch (Exception e)
            {
                this.LogError($"Error while saving the file on: {fullPath}. \nException: {e}");
            }
        }

        /// <summary>
        /// Loads all save data and shows it in the game UI
        /// </summary>
        /// <returns>Returns all the loaded saves in a Dictionary</returns>
        public Dictionary<string, GameData> LoadAllSaveProfiles()
        {
            Dictionary<string, GameData> profileDictionary = new();

            // Loop over all directory names in the data directory path of the PC
            IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

            foreach (DirectoryInfo dirInfo in dirInfos)
            {
                string profileID = dirInfo.Name;

                // Check if the data path exist
                // If it doesn't, then this folder isn't a save file and should be skipped
                string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);

                if (!File.Exists(fullPath))
                {
                    this.LogWarning($"Skipping this directory, because is not a save file: {profileID}");
                    continue;
                }

                // Load this save data for this profile and put it in the dictionary
                GameData profileData = Load(profileID);

                // Ensure the game data isn't null
                // if it is null, log an error so we let ourselves know that something went wrong
                if (profileData != null)
                {
                    profileDictionary.Add(profileID, profileData);
                }
                else
                {
                    this.LogError($"Tried to load data but something went wrong. ProfileID: {profileID}");
                }

            }

            return profileDictionary;
        }

        /// <summary>
        /// Get the most recent profile ID that was saved 
        /// </summary>
        /// <returns>Returns the most recent profile ID</returns>
        public string GetMostRecentProfileID()
        {
            string mostRecentProfileID = null;

            Dictionary<string, GameData> profilesGameData = LoadAllSaveProfiles();

            foreach (KeyValuePair<string, GameData> keyValue in profilesGameData)
            {
                string profileID = keyValue.Key;
                GameData profileData = keyValue.Value;

                // Skip if null
                if (profileData == null)
                {
                    continue;
                }

                // If it's the first data we come across
                if (mostRecentProfileID == null)
                {
                    mostRecentProfileID = profileID;
                }
                // Otherwise, compare to see which date is the most recent one
                else
                {
                    DateTime mostRecentTime = DateTime.FromBinary(profilesGameData[mostRecentProfileID].LastUpdated);
                    DateTime newTime = DateTime.FromBinary(profileData.LastUpdated);

                    if (newTime > mostRecentTime)
                    {
                        mostRecentProfileID = profileID;
                    }
                }
            }
            return mostRecentProfileID;
        }


        /// <summary>
        /// This method encrypts the data with XOR encryption 
        /// </summary>
        private string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);
            }
            return modifiedData;
        }

        /// <summary>
        /// Attemps to roll back from the backup file, if the standard file becomes corrupted
        /// </summary>
        /// <param name="fullPath">The full path where the file is stored</param>
        /// <returns>Returns true if the roll back succeded, false otherwise</returns>
        private bool AttemptRollBack(string fullPath)
        {
            bool success = false;
            string backupFilePath = fullPath + backupExtension;

            try
            {
                // If the backup exist roll back from it
                if (File.Exists(backupFilePath))
                {
                    File.Copy(fullPath, backupFilePath, true);
                    success = true;
                    this.Log("Had to roll back from backup file");
                }
                // Otherwise, we don't yet have a backup file, so there's nothing to load from
                else
                {
                    throw new Exception("No backup file to roll back to.");
                }
            }
            catch (Exception e)
            {
                this.LogError($"Error ocurred when trying to roll back from the backup file: {backupFilePath}.\nException: {e}");
            }

            return success;
        }
    }
}