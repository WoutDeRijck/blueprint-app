using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace Misc
{

    /// <summary>
    /// This class provides functionality for content-driven game development (JSON).
    /// GOAL: store the game's content separately from the game's code.
    /// </summary>
    public static class FileHandler
    {
        /// <summary>
        /// Saves Object of type T to json string
        /// </summary>
        /// <returns> json string </returns>
        public static string SaveToJSONString<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            string content = JsonConvert.SerializeObject(obj);
            return content;
        }

        /// <summary>
        /// Saves Object of type T to .json file
        /// </summary>
        public static void SaveToJSONFile<T>(T obj, string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException();
            }
            string content = SaveToJSONString<T>(obj);
            Debug.Log(GetPath(filename));
            WriteFile(GetPath(filename), content);
        }

        /// <summary>
        /// Reads json string and parses it into object of type T
        /// </summary>
        /// <returns> Object of type T </returns>
        public static T ReadFromJSONString<T>(string content)
        {
            if (string.IsNullOrEmpty(content) || content == "{}")
            {
                throw new ArgumentException();
            }

            T res = JsonConvert.DeserializeObject<T>(content);

            return res;
        }

        /// <summary>
        /// Reads .json file and parses it into object of type T
        /// </summary>
        /// <returns> Object of type T </returns>
        public static T ReadFromJSONFile<T>(string filename)
        {
            string content = ReadFile(GetPath(filename));
            T res = ReadFromJSONString<T>(content);
            return res;
        }

        /// <summary>
        /// Creates path from filename, from the persistent data path
        /// </summary>
        /// <returns> path </returns>
        private static string GetPath(string filename)
        {
            return Application.persistentDataPath + "/" + filename;
        }

        /// <summary>
        /// Writes file to path
        /// </summary>
        private static void WriteFile(string path, string content)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(content);
            }
        }

        /// <summary>
        /// Reads file from path
        /// </summary>
        /// <returns> string </returns>
        private static string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string content = reader.ReadToEnd();
                    return content;
                }
            }
            return "";
        }
    }
}