using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace HL7Lib.Plugin {
    /// <summary>
    /// Generic plugin interface - to be inherited by specific interfaces - used for generic plugin loaders
    /// </summary>
    public interface IPlugin {
        /// <summary>
        /// Plugin name
        /// </summary>
        string Name { get; }
    }
    /// <summary>
    /// Interface for a plugin loader class
    /// </summary>
    public interface IPluginLoader {
        /// <summary>
        /// A default directory for plugins
        /// </summary>
        string DefaultPluginDirectory { get; }
        /// <summary>
        /// Contains the Type to be loaded from plugins
        /// </summary>
        Type PluginType { get; }
        /// <summary>
        /// Return the DLL paths from the plugin directory
        /// </summary>
        /// <param name="pluginDir">The directory to search</param>
        /// <returns></returns>
        IEnumerable<string> GetLibraryPaths(string pluginDir);
        /// <summary>
        /// Return the DLL paths from the plugin directory
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetLibraryPaths();
        /// <summary>
        /// A method to load plugin assemblies
        /// </summary>
        /// <param name="dllPaths">The library paths to load from</param>
        /// <returns></returns>
        ICollection<Assembly> LoadAssemblies(IEnumerable<string> dllPaths);
        /// <summary>
        /// Return the Types implementing the PluginType interface from the provided Assembly objects
        /// </summary>
        /// <param name="assemblies">A collection of Assembly objects to retrieve Types from</param>
        /// <returns></returns>
        ICollection<Type> GetPluginTypes(ICollection<Assembly> assemblies);
        /// <summary>
        /// Return a collection of object instances of the provided plugin Types
        /// </summary>
        /// <typeparam name="T">Provide the PluginType property of the loader</typeparam>
        /// <param name="pluginTypes">A collection of Types to be instantiated from the plugin libraries</param>
        /// <returns></returns>
        ICollection<T> GetPluginObjects<T>(ICollection<Type> pluginTypes);
        /// <summary>
        /// Return a collection of object instances, loaded from the provided plugin directory - the most direct endpoint for this object
        /// </summary>
        /// <typeparam name="T">Provide the PluginType property of the loader</typeparam>
        /// <param name="pluginDir">The directory to search</param>
        /// <returns></returns>
        ICollection<T> GetPluginObjects<T>(string pluginDir);
        /// <summary>
        /// Return a collection of object instances, loaded from the provided plugin directory - the most direct endpoint for this object
        /// </summary>
        /// <typeparam name="T">Provide the PluginType property of the loader</typeparam>
        /// <returns></returns>
        ICollection<T> GetPluginObjects<T>();
    }
    /// <summary>
    /// A base plugin loader class
    /// </summary>
    public class PluginLoaderBase : IPluginLoader {
        /// <summary>
        /// A default directory for plugins
        /// </summary>
        internal virtual string _DefaultPluginDirectory {
            get {
                Assembly assem = Assembly.GetExecutingAssembly();
                string loc = assem.Location;
                string nam = Path.GetDirectoryName(loc);
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "plugins");
            }
        }
        /// <summary>
        /// A default directory for plugins, retrieval will create the directory if it does not exist
        /// </summary>
        public virtual string DefaultPluginDirectory {
            get {
                if (!Directory.Exists(_DefaultPluginDirectory)) {
                    try {
                        Directory.CreateDirectory(_DefaultPluginDirectory);
                    }
                    catch (Exception ex) {
                        //
                    }
                }
                return _DefaultPluginDirectory;
            }
        }
        /// <summary>
        /// Contains the interface Type to be loaded from plugins
        /// </summary>
        public Type PluginType { get; private set; }
        /// <summary>
        /// Base constructor - uses the default IPlugin Type
        /// </summary>
        public PluginLoaderBase() {
            PluginType = typeof(IPlugin);
        }
        /// <summary>
        /// Constructor accepting the plugin Type
        /// </summary>
        /// <param name="pluginType"></param>
        public PluginLoaderBase(Type pluginType) {
            if (pluginType.IsInterface) {
                PluginType = pluginType;
            }
            else {
                throw new Exception("Provided PluginType is not an interface");
            }
        }
        /// <summary>
        /// Return the DLL paths from the plugin directory
        /// </summary>
        /// <param name="pluginDir">The directory to search</param>
        /// <returns></returns>
        public IEnumerable<string> GetLibraryPaths(string pluginDir) {
            string[] dllPaths = null;
            if (Directory.Exists(pluginDir)) {
                dllPaths = Directory.GetFiles(pluginDir, "*.dll");
            }
            return dllPaths;
        }
        /// <summary>
        /// Return the DLL paths from the plugin directory
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLibraryPaths() {
            string[] dllPaths = null;
            if (Directory.Exists(DefaultPluginDirectory)) {
                dllPaths = Directory.GetFiles(DefaultPluginDirectory, "*.dll");
            }
            return dllPaths;
        }
        /// <summary>
        /// A method to load plugin assemblies
        /// </summary>
        /// <param name="dllPaths">The library paths to load from</param>
        /// <returns></returns>
        public ICollection<Assembly> LoadAssemblies(IEnumerable<string> dllPaths) {
            ICollection<Assembly> assemblies = (dllPaths == null) ? null : new List<Assembly>(dllPaths.Count());
            foreach (string dll in dllPaths) {
                AssemblyName name = AssemblyName.GetAssemblyName(dll);
                Assembly assembly = Assembly.Load(name);
                assemblies.Add(assembly);
            }
            return assemblies;
        }
        /// <summary>
        /// Return the Types implementing the PluginType interface from the provided Assembly objects
        /// </summary>
        /// <param name="assemblies">A collection of Assembly objects to retrieve Types from</param>
        /// <returns></returns>
        public ICollection<Type> GetPluginTypes(ICollection<Assembly> assemblies) {
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies) {
                if (assembly != null) {
                    foreach (Type type in assembly.GetTypes()) {
                        if (type.IsInterface || type.IsAbstract) {
                            continue;
                        }
                        else if (type.GetInterface(PluginType.FullName) != null) {
                            pluginTypes.Add(type);
                        }
                    }
                }
            }
            return pluginTypes;
        }
        /// <summary>
        /// Return a collection of object instances of the provided plugin Types
        /// </summary>
        /// <typeparam name="T">Provide the PluginType property of the loader</typeparam>
        /// <param name="pluginTypes">A collection of Types to be instantiated from the plugin libraries</param>
        /// <returns></returns>
        public ICollection<T> GetPluginObjects<T>(ICollection<Type> pluginTypes) {
            ICollection<T> plugins = new List<T>(pluginTypes.Count);
            foreach (Type type in pluginTypes) {
                T plugin = (T)Activator.CreateInstance(type);
                plugins.Add(plugin);
            }
            return plugins;
        }
        /// <summary>
        /// Return a collection of object instances, loaded from the provided plugin directory - the most direct endpoint for this object
        /// </summary>
        /// <typeparam name="T">Provide the PluginType property of the loader</typeparam>
        /// <param name="pluginDir">The directory to search</param>
        /// <returns></returns>
        public ICollection<T> GetPluginObjects<T>(string pluginDir) {
            IEnumerable<string> dllPaths = GetLibraryPaths(pluginDir);
            ICollection<Assembly> assemblies = LoadAssemblies(dllPaths);
            ICollection<Type> pluginTypes = GetPluginTypes(assemblies);
            ICollection<T> plugins = GetPluginObjects<T>(pluginTypes);

            return plugins;
        }
        /// <summary>
        /// Return a collection of object instances, loaded from the plugin directory - the most direct endpoint for this object
        /// </summary>
        /// <typeparam name="T">Provide the PluginType property of the loader</typeparam>
        /// <returns></returns>
        public ICollection<T> GetPluginObjects<T>() {
            IEnumerable<string> dllPaths = GetLibraryPaths();
            ICollection<Assembly> assemblies = LoadAssemblies(dllPaths);
            ICollection<Type> pluginTypes = GetPluginTypes(assemblies);
            ICollection<T> plugins = GetPluginObjects<T>(pluginTypes);

            return plugins;
        }
    }
}
