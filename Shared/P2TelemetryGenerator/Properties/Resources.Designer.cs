﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace P2TelemetryGenerator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("P2TelemetryGenerator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to $:2015-01-28T21:49:18Z,989.6,198.8,13.0,77.6,13.0,2.2,7.5,7.4,0,0,1,0,-3200,-384,17408,-3200,-384,17408,-3200,-384,17408,1.0,46.8301,-119.1643,198.8,6.4,169.5,1,6,0,-0.7,0,0,1,0,0,1000,02:30,*CA
        ///$:2015-01-28T21:49:20Z,988.6,207.2,13.0,77.6,13.0,1.8,7.5,7.4,0,0,1,0,-3712,128,18432,-3712,128,18432,-3712,128,18432,1.0,46.8301,-119.1643,207.2,9.1,169.5,1,5,1,4.2,0,0,1,0,0,1000,02:30,*FA
        ///$:2015-01-28T21:49:22Z,987.3,218.3,13.0,77.8,13.0,1.3,7.5,7.4,0,0,1,0,-5440,832,17728,-5440,832,17728,-5440,832,17728,1.0,46 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CraftTelemetry {
            get {
                return ResourceManager.GetString("CraftTelemetry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string LaunchGroundTelemetry {
            get {
                return ResourceManager.GetString("LaunchGroundTelemetry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string MobileGroundTelemetry {
            get {
                return ResourceManager.GetString("MobileGroundTelemetry", resourceCulture);
            }
        }
    }
}
