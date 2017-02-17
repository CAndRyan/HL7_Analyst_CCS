using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Lib.Plugin {
    /// <summary>
    /// Plugin interface to be implemented by a DeIdentify plugin
    /// </summary>
    public interface IDeIdentifyPlugin : IPlugin {
        /// <summary>
        /// Public method for access through this application
        /// </summary>
        /// <param name="item">A de-identifiable item</param>
        /// <param name="context">Some context for generating data from the provided item</param>
        /// <returns></returns>
        string Generate(IDeIdentifiable item, IDeIdentifyPluginContext context);
    }
    /// <summary>
    /// Interface for any item that can be de-identifiable
    /// </summary>
    public interface IDeIdentifiable {
        /// <summary>
        /// The current value of this DeIdentifiable object
        /// </summary>
        string Value { get; }
    }
    /// <summary>
    /// Plugin context interface for providing IDeIdentifyPlugin with information
    /// </summary>
    public interface IDeIdentifyPluginContext {
        /// <summary>
        /// Type of DeIdentifiable item - used for generation
        /// </summary>
        string Type { get; }
    }
    /// <summary>
    /// Plugin loader for deIdentify plugins
    /// </summary>
    public class DeIdentifyPluginLoader : PluginLoaderBase {
        /// <summary>
        /// A default directory for plugins
        /// </summary>
        internal override string _DefaultPluginDirectory {
            get {
                var assem = System.Reflection.Assembly.GetExecutingAssembly();
                string loc = assem.Location;
                string nam = System.IO.Path.GetDirectoryName(loc);
                string comb = System.IO.Path.Combine(nam, "plugins\\deidentify");
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "plugins\\deidentify");
            }
        }
        /// <summary>
        /// Empty constructor which calls the base constructor with an IDeIdentifyPlugin Type
        /// </summary>
        public DeIdentifyPluginLoader() : base(typeof(IDeIdentifyPlugin)) { }
        /// <summary>
        /// Wrapper for the base GetPluginObjects method
        /// </summary>
        /// <param name="pluginTypes"></param>
        /// <returns></returns>
        public ICollection<IDeIdentifyPlugin> GetPluginObjects(ICollection<Type> pluginTypes) {
            return base.GetPluginObjects<IDeIdentifyPlugin>(pluginTypes);
        }
        /// <summary>
        /// Wrapper for the base GetPluginObjects method - the most direct endpoint for this object
        /// </summary>
        /// <param name="pluginDir">The directory to search</param>
        /// <returns></returns>
        public ICollection<IDeIdentifyPlugin> GetPluginObjects(string pluginDir) {
            return base.GetPluginObjects<IDeIdentifyPlugin>(pluginDir);
        }
        /// <summary>
        /// Wrapper for the base GetPluginObjects method, using the default directory - the most direct endpoint for this object
        /// </summary>
        /// <returns></returns>
        public ICollection<IDeIdentifyPlugin> GetPluginObjects() {
            return base.GetPluginObjects<IDeIdentifyPlugin>();
        }
    }
}
