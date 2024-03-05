using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

using System.Linq;

namespace Misc
{
    /// <summary>
    /// Manages the editing, creating and opening of csv files
    /// </summary>
    public static class CSVManager
    {
        /// <summary>
        /// Get data from CSV
        /// </summary>
        public static List<string[]> ReadCSV(string filename)
        {
            List<string[]> data = new List<string[]>();

            StreamReader streamReader = new StreamReader(GetPath(filename));
            while (!streamReader.EndOfStream)
            {
                string[] rowData = streamReader.ReadLine().Split(';');
                data.Add(rowData);
            }
            streamReader.Close();

            return data;
        }

        /// <summary>
        /// Reads the whole CSV to one string
        /// </summary>
        public static string ReadCSVToString(string filename)
        {
            string csvData = "";

            StreamReader streamReader = new StreamReader(GetPath(filename));
            csvData = streamReader.ReadToEnd();
            streamReader.Close();

            return csvData;
        }

        /// <summary>
        /// Converts the CSV one-string to a list of string arrays
        /// </summary>
        public static List<string[]> StringToCSVList(string data)
        {
            List<string[]> csv = new List<string[]>();

            string[] rows = data.Split('\n');
            foreach (string row in rows)
            {
                if (string.IsNullOrEmpty(row) || row.All(c => (c==';' || c==' ' || c=='\n'))) break;
                string[] rowData = row.Split(';');
                csv.Add(rowData);
            }

            return csv;
        }

        /// <summary>
        /// Write data to CSV
        /// </summary>
        public static void WriteCSV(string filename, List<string[]> data)
        {
            StreamWriter streamWriter = new StreamWriter(GetPath(filename));

            for (int i = 0; i < data.Count; i++)
            {
                string s = (string.Join(";", data[i])).Trim(new char[] {'\n', '\r'});
                streamWriter.WriteLine(s);
            }
            streamWriter.Close();
        }

        /// <summary>
        /// Open a CSV editor (like Excel) to edit the ECS
        /// </summary>
        public static Process Open(string filename, List<string[]> csv)
        {
            if (!File.Exists(GetPath(filename))) 
            { 
                File.Create(GetPath(filename)).Close(); 
            }
            WriteCSV(filename, csv);

            Process excelProcess = Process.Start(GetPath(filename));
            excelProcess.EnableRaisingEvents = true;
            return excelProcess;
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
        /// Delete file given the path
        /// </summary>
        public static void DeleteFile(string filename) 
        {
            if (File.Exists(GetPath(filename)))
            {
                File.Delete(GetPath(filename));
            }
        }
    }
}
