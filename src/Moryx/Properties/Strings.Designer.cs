﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Moryx.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Moryx.Properties.Strings", typeof(Strings).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rejoins the incoming paths into a single output.
        /// </summary>
        public static string JoinWorkplanStep_Description {
            get {
                return ResourceManager.GetString("JoinWorkplanStep_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Join.
        /// </summary>
        public static string JoinWorkplanStep_Name {
            get {
                return ResourceManager.GetString("JoinWorkplanStep_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Splits the incoming path into multiple outputs.
        /// </summary>
        public static string SplitWorkplanStep_Description {
            get {
                return ResourceManager.GetString("SplitWorkplanStep_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Split.
        /// </summary>
        public static string SplitWorkplanStep_Name {
            get {
                return ResourceManager.GetString("SplitWorkplanStep_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sub-Workplan.
        /// </summary>
        public static string SubworkflowStep_Description {
            get {
                return ResourceManager.GetString("SubworkflowStep_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nests a sub-workplan in the current workplan.
        /// </summary>
        public static string SubworkflowStep_Name {
            get {
                return ResourceManager.GetString("SubworkflowStep_Name", resourceCulture);
            }
        }
    }
}
