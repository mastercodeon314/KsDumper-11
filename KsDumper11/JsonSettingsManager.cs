using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace KsDumper11
{
    public class JsonSettingsManager
    {
        public JsonSettings JsonSettings { get; set; }

        private string settingsFilePath;
        public JsonSettingsManager()
        {
            settingsFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.json";
            CreateOrLoadSettingsJson();
        }

        private void CreateOrLoadSettingsJson()
        {
            if (File.Exists(settingsFilePath))
            {
                // Load settings json
                string settingsJsonText = File.ReadAllText(settingsFilePath);
                JsonSettings = JsonConvert.DeserializeObject<JsonSettings>(settingsJsonText);
            }
            else
            {
                // Populate and save default settings json
                JsonSettings = new JsonSettings();
                JsonSettings.enableAntiAntiDebuggerTools = false;
                JsonSettings.closeDriverOnExit = false;

                string settingsJsonText = JsonConvert.SerializeObject(JsonSettings, Formatting.Indented);
                File.WriteAllText(settingsFilePath, settingsJsonText);
            }
        }

        public void Save()
        {
            string settingsJsonText = JsonConvert.SerializeObject(JsonSettings, Formatting.Indented);
            File.WriteAllText(settingsFilePath, settingsJsonText);
        }
    }
}
