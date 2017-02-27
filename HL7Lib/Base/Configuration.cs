using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using HL7Lib.Plugin;

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
        /// <param name="logger">an ILogWriter instance</param>
        /// <returns>A list of config items storing the PHI config data</returns>
        public static IEditManager LoadPHI(Message msg, ILogWriter logger) {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PHI_FIELDS_PATH);
            return new EditManager(JsonConvert.DeserializeObject<IEnumerable<ConfigItem>>(File.ReadAllText(path)), msg, logger);
        }
        /// <summary>
        /// Static method to load the PHI field configurations
        /// </summary>
        /// <returns>A list of config items storing the PHI config data</returns>
        public static IEditManager LoadPHI(Message msg) {
            return LoadPHI(msg, LogWriter.Instance);
        }
    }
    /// <summary>
    /// Contract for an EditManager to follow
    /// </summary>
    public interface IEditManager {
        /// <summary>
        /// Flag specifying whether or not a repass is required
        /// </summary>
        bool Repass { get; }
        /// <summary>
        /// Perform additional work after a pass is complete
        /// </summary>
        void CompletePass();
        /// <summary>
        /// Dictionary for replacements to be made in a repass
        /// </summary>
        Dictionary<string, Tuple<IDeIdentifyPluginContext, IDeIdentifiable>> Replacements { get; }
        /// <summary>
        /// Clean any changes to the EditManager
        /// </summary>
        void Cleanup();
        /// <summary>
        /// Check if the EditManager contains a config for this componentId
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns>Whether or not a config exists for this componentId</returns>
        bool Contains(string componentId);
        /// <summary>
        /// Edits a component, determined by the ConfigItem for this component
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns an updated component, modified according to the ConfigItem</returns>
        Component Edit(Component item);
    }
    /// <summary>
    /// IEditManager to manage custom Component editing
    /// </summary>
    public class EditManager : IEditManager {
        /// <summary>
        /// Flag specifying whether or not a repass is required
        /// </summary>
        public bool Repass {
            get {
                return Replacements.Count > 0;
            }
        }
        /// <summary>
        /// Store the current pass - 1 is default
        /// </summary>
        public int PassNumber { get; private set; } = 1;
        /// <summary>
        /// Dictionary for replacements to be made in a repass
        /// </summary>
        public Dictionary<string, Tuple<IDeIdentifyPluginContext, IDeIdentifiable>> Replacements { get; private set; } = new Dictionary<string, Tuple<IDeIdentifyPluginContext, IDeIdentifiable>>();
        /// <summary>
        /// The internal dictionary for checking and retrieving necessary updates
        /// </summary>
        private Dictionary<string, ConfigItem> Items;
        /// <summary>
        /// The ILogWriter to handle any errors
        /// </summary>
        private ILogWriter Logger;
        /// <summary>
        /// A PluginLoader
        /// </summary>
        private HL7Lib.Plugin.DeIdentifyPluginLoader PluginLoader;
        /// <summary>
        /// EditManager constructor
        /// </summary>
        /// <param name="items">an IEnumerable of ConfigItems</param>
        /// <param name="logger">an ILogWriter instance</param>
        public EditManager(IEnumerable<ConfigItem> items, Message msg, ILogWriter logger) {
            PluginLoader = new HL7Lib.Plugin.DeIdentifyPluginLoader();
            ICollection<HL7Lib.Plugin.IDeIdentifyPlugin> plugins = PluginLoader.GetPluginObjects();

            // Load the Generators and Gender for each ConfigItem
            Gender gender = msg.GetGender();
            foreach (ConfigItem item in items) {
                if (item.GeneratorName != null) {
                    item.SetGenerator(plugins.Where(p => p.Name == item.GeneratorName).FirstOrDefault());
                }

                item.Gender = gender;
            }
            
            Items = items.ToDictionary(item => item.Id, item => item);
            Logger = logger;
        }
        /// <summary>
        /// EditManager constructor that will use default ILogWriter
        /// </summary>
        /// <param name="items"></param>
        public EditManager(IEnumerable<ConfigItem> items, Message msg) : this(items, msg, LogWriter.Instance) { }
        /// <summary>
        /// Check if the EditManager contains a config for this componentId
        /// </summary>
        /// <param name="componentId">the id of a component to check</param>
        /// <returns>Whether or not a config exists for this componentId</returns>
        public bool Contains(string componentId) {
            return Items.ContainsKey(componentId);
        }
        /// <summary>
        /// Edits a component, determined by the ConfigItem for this component
        /// </summary>
        /// <param name="item">a Component to be updated according to a ConfigItem for it</param>
        /// <returns></returns>
        public Component Edit(Component item) {
            if (PassNumber == 1) {
                if (Contains(item.ID)) {
                    ConfigItem config = Items[item.ID];

                    if (config.Repass) {
                        Replacements.Add(item.Value, new Tuple<IDeIdentifyPluginContext, IDeIdentifiable>(config, item));
                    }

                    if (config.Generator != null) {
                        try {
                            item.Value = config.Generate(item);
                        }
                        catch (Exception ex) {
                            Logger.LogException(ex);
                        }
                    }
                    else if (config.Static != null) {
                        item.Value = config.GetStatic(item);
                    }
                }

                return item;
            }
            else {      // repass
                return RepassEdit(item);
            }
        }
        /// <summary>
        /// Edits a component, determined by the ConfigItem for this component - as a repass
        /// </summary>
        /// <param name="item">a Component to be updated according to a ConfigItem for it</param>
        /// <returns></returns>
        public Component RepassEdit(Component item) {
            if (!String.IsNullOrEmpty(item.Value)) {
                for (int i = 0; i < Replacements.Keys.Count; i++) {
                    string key = Replacements.Keys.ElementAt(i);

                    if (item.Value.Contains(key)) {
                        int startIndex = item.Value.IndexOf(key);
                        string start = item.Value.Substring(0, startIndex);
                        string end = item.Value.Substring((startIndex + key.Length));
                        string newVal = String.Empty;

                        ConfigItem config = (ConfigItem)Replacements[key].Item1;
                        if (config.Generator != null) {
                            try {
                                newVal = config.Generate(Replacements[key].Item2);
                            }
                            catch (Exception ex) {
                                Logger.LogException(ex);
                            }
                        }
                        else if (config.Static != null) {
                            newVal = config.Static;
                        }

                        item.Value = String.Format("{0}{1}{2}", start, newVal, end);
                        i = Replacements.Keys.Count;
                    }
                }
            }

            return item;
        }
        /// <summary>
        /// Clean any changes to the EditManager
        /// </summary>
        public void Cleanup() {
            Replacements = new Dictionary<string, Tuple<IDeIdentifyPluginContext, IDeIdentifiable>>();
            PassNumber = 1;

            foreach (string key in Items.Keys) {
                Items[key].Cleanup();
            }
        }
        /// <summary>
        /// Perform additional work after a pass is complete
        /// </summary>
        public void CompletePass() {
            PassNumber++;
        }
    }
    /// <summary>
    /// Config items to store config data for components
    /// </summary>
    public class ConfigItem : HL7Lib.Plugin.IDeIdentifyPluginContext {
        /// <summary>
        /// Id for accessing this component from a Message
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Label (or name) for this component
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Type of Component
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The gender of the patient being referenced
        /// </summary>
        public Gender Gender { get; set; } = Gender.unknown;
        /// <summary>
        /// Generator to use for modifying this component
        /// </summary>
        public HL7Lib.Plugin.IDeIdentifyPlugin Generator { get; private set; }
        /// <summary>
        /// Generator name to use
        /// </summary>
        public string GeneratorName { get; set; }
        /// <summary>
        /// Static replacement for this component (used if the Generator is null)
        /// </summary>
        public string Static { get; set; }
        /// <summary>
        /// If true, a repass of the message will replace all instances of this item's original value (from all other components)
        /// </summary>
        public bool Repass { get; set; } = false;
        /// <summary>
        /// Contains replacement patterns to perform after retrieving the ConfigItem's Message component but before using the ConfigItem
        /// </summary>
        public IEnumerable<Replacement> PreReplace { get; set; } = new List<Replacement>();
        /// <summary>
        /// The old value provided before generating it
        /// </summary>
        public string OldValue { get; private set; } = null;
        /// <summary>
        /// Get the modified value for the related component
        /// </summary>
        /// <returns></returns>
        public string GetValue() {
            if (GeneratorName != null) {
                return "GENERATED";
            }
            else if (Static != null) {
                return Static;
            }
            else {
                return Configuration.PHI_DEFAULT_VALUE;
            }
        }
        /// <summary>
        /// Method to set the Generator plugin to use
        /// </summary>
        /// <param name="plugin">The plugin to use</param>
        public void SetGenerator(HL7Lib.Plugin.IDeIdentifyPlugin plugin) {
            Generator = plugin;
        }
        /// <summary>
        /// Method to generate a new value from this config's Generator
        /// </summary>
        /// <param name="item">An IdeIdentiable item to generate from</param>
        /// <returns></returns>
        public string Generate(HL7Lib.Plugin.IDeIdentifiable item) {
            OldValue = item.Value;
            return Generator.Generate(item, this);
        }
        /// <summary>
        /// Method to get the static value and set the old values
        /// </summary>
        /// <param name="item">An IdeIdentiable item to generate from</param>
        /// <returns></returns>
        public string GetStatic(HL7Lib.Plugin.IDeIdentifiable item) {
            OldValue = item.Value;
            return Static;
        }
        /// <summary>
        /// Cleanup any old values present with this config
        /// </summary>
        public void Cleanup() {
            OldValue = null;
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
        /// The string to replace a match with
        /// </summary>
        public string Replace { get; set; }
        /// <summary>
        /// A boolean that tells whether or not Match must exactly match the string to replace
        /// </summary>
        public bool ExactMatch { get; set; } = true;

        //
    }
}
