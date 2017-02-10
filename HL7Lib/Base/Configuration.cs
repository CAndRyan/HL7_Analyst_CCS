using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace HL7Lib.Base {
    /// <summary>
    /// Configuration class: loads configuration details
    /// </summary>
    public static class Configuration {
        /// <summary>
        /// File name for loading PHI field configurations
        /// </summary>
        public const string PHI_FIELDS_PATH = "PHI_Fields.json";
        /// <summary>
        /// Default value to return for a component
        /// </summary>
        public const string PHI_DEFAULT_VALUE = "DEFAULT";
        /// <summary>
        /// Static method to load the PHI field configurations
        /// </summary>
        /// <returns>A list of config items storing the PHI config data</returns>
        public static IEnumerable<ConfigItem> LoadPHI() {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PHI_FIELDS_PATH);
            return JsonConvert.DeserializeObject<IEnumerable<ConfigItem>>(File.ReadAllText(path));
        }
    }
    /// <summary>
    /// Config items to store config data for components
    /// </summary>
    public class ConfigItem {
        /// <summary>
        /// Id for accessing this component from a Message
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Label (or name) for this component
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Generator to use for modifying this component
        /// </summary>
        public string Generator { get; set; }
        /// <summary>
        /// Static replacement for this component (used if the Generator is null)
        /// </summary>
        public string Static { get; set; }
        /// <summary>
        /// Contains replacement patterns to perform after retrieving the ConfigItem's Message component but before using the ConfigItem
        /// </summary>
        public IEnumerable<Replacement> PreReplace { get; set; } = new List<Replacement>();
        /// <summary>
        /// Get the modified value for the related component
        /// </summary>
        /// <returns></returns>
        public string GetValue() {
            if (Generator != null) {
                return "GENERATED";
            }
            else if (Static != null) {
                return Static;
            }
            else {
                return Configuration.PHI_DEFAULT_VALUE;
            }
        }
    }
    /// <summary>
    /// Replacement class: contains details for replacing string values with another
    /// </summary>
    public class Replacement {
        /// <summary>
        /// The string to match for replacement
        /// </summary>
        public string Match { get; set; }
        /// <summary>
        /// The string to replce a match with
        /// </summary>
        public string Replace { get; set; }
        /// <summary>
        /// A boolean that tells whether or not Match must exactly match the string to replace
        /// </summary>
        public bool ExactMatch { get; set; } = true;

        //
    }
}
