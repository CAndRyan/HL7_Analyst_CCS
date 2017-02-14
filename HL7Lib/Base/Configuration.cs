﻿using System;
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
        /// <param name="logger">an ILogWriter instance</param>
        /// <returns>A list of config items storing the PHI config data</returns>
        public static IEditManager LoadPHI(ILogWriter logger) {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PHI_FIELDS_PATH);
            return new EditManager(JsonConvert.DeserializeObject<IEnumerable<ConfigItem>>(File.ReadAllText(path)), logger);
        }
        /// <summary>
        /// Static method to load the PHI field configurations
        /// </summary>
        /// <returns>A list of config items storing the PHI config data</returns>
        public static IEditManager LoadPHI() {
            return LoadPHI(LogWriter.Instance);
        }
    }
    /// <summary>
    /// Contract for an EditManager to follow
    /// </summary>
    public interface IEditManager {
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
        /// The internal dictionary for checking and retrieving necessary updates
        /// </summary>
        private Dictionary<string, ConfigItem> Items;
        /// <summary>
        /// The ILogWriter to handle any errors
        /// </summary>
        private ILogWriter Logger;
        /// <summary>
        /// EditManager constructor
        /// </summary>
        /// <param name="items">an IEnumerable of ConfigItems</param>
        /// <param name="logger">an ILogWriter instance</param>
        public EditManager(IEnumerable<ConfigItem> items, ILogWriter logger) {
            Items = items.ToDictionary(item => item.Id, item => item);
            Logger = logger;
        }
        /// <summary>
        /// EditManager constructor that will use default ILogWriter
        /// </summary>
        /// <param name="items"></param>
        public EditManager(IEnumerable<ConfigItem> items) : this(items, LogWriter.Instance) { }
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
            if (Contains(item.ID)) {
                ConfigItem config = Items[item.ID];
                
                if (config.Generator != null) {
                    try {
                        //fixme - Add support for Generators
                    }
                    catch (Exception ex) {
                        Logger.LogException(ex);
                    }
                }
                else if (config.Static != null) {
                    item.Value = config.Static;
                }
                
            }
            
            return item;
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
        /// Type of Component
        /// </summary>
        public string Type { get; set; }
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
