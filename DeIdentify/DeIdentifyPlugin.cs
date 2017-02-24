using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL7Lib.Plugin;

namespace DeIdentify {
    /// <summary>
    /// A DeIdentify Plugin
    /// </summary>
    public class TestGenerator1 : IDeIdentifyPlugin {
        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name { get; } = "Test_Generator1";
        /// <summary>
        /// Method to generate a value from the provided context
        /// </summary>
        /// <param name="item">The item to de-identify</param>
        /// <param name="context">The context of this item. Provides additional details for making decisions on string generation</param>
        public string Generate(IDeIdentifiable item, IDeIdentifyPluginContext context) {
            return String.Format("{0}_{1}", item.Value, context.Type);
        }
    }
    /// <summary>
    /// A DeIdentify Plugin
    /// </summary>
    public class TestGenerator2 : IDeIdentifyPlugin {
        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name { get; } = "Test_Generator2";
        /// <summary>
        /// Method to generate a value from the provided context
        /// </summary>
        /// <param name="item">The item to de-identify</param>
        /// <param name="context">The context of this item. Provides additional details for making decisions on string generation</param>
        public string Generate(IDeIdentifiable item, IDeIdentifyPluginContext context) {
            return String.Format("{0}_{1}_{2}", "Xa", item.Value, "aX");
        }
    }
    /// <summary>
    /// A DeIdentify Plugin for generating random names
    /// </summary>
    public class RandomFirstName : IDeIdentifyPlugin {
        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name { get; } = "RandomFirstName";
        /// <summary>
        /// Method to generate a value from the provided context
        /// </summary>
        /// <param name="item">The item to de-identify</param>
        /// <param name="context">The context of this item. Provides additional details for making decisions on string generation</param>
        /// <returns></returns>
        public string Generate(IDeIdentifiable item, IDeIdentifyPluginContext context) {
            return HL7Lib.Base.Helper.RandomFirstName(context.Gender, item.Value.GetHashCode());
        }
    }
    /// <summary>
    /// A DeIdentify Plugin for generating random names
    /// </summary>
    public class RandomLastName : IDeIdentifyPlugin {
        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name { get; } = "RandomLastName";
        /// <summary>
        /// Method to generate a value from the provided context
        /// </summary>
        /// <param name="item">The item to de-identify</param>
        /// <param name="context">The context of this item. Provides additional details for making decisions on string generation</param>
        /// <returns></returns>
        public string Generate(IDeIdentifiable item, IDeIdentifyPluginContext context) {
            return HL7Lib.Base.Helper.RandomLastName();
        }
    }
}
