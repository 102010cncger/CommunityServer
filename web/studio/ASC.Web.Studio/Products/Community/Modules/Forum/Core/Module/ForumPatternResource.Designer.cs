/*
 *
 * (c) Copyright Ascensio System Limited 2010-2016
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.36323
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASC.Web.Community.Forum.Core.Module {
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
    public class ForumPatternResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ForumPatternResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ASC.Web.Community.Modules.Forum.Core.Module.ForumPatternResource", typeof(ForumPatternResource).Assembly);
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
        ///   Looks up a localized string similar to h1.New Post in Forum Topic: &quot;$TopicTitle&quot;:&quot;$PostURL&quot;
        ///
        ///$Date &quot;$UserName&quot;:&quot;$UserURL&quot; has created a new post in topic:
        ///
        ///$PostText
        ///
        ///&quot;Read More&quot;:&quot;$PostURL&quot;
        ///
        ///Your portal address: &quot;$__VirtualRootPath&quot;:&quot;$__VirtualRootPath&quot;
        ///
        ///&quot;Edit subscription settings&quot;:&quot;$RecipientSubscriptionConfigURL&quot;.
        /// </summary>
        public static string pattern_PostInTopicEmailPattern {
            get {
                return ResourceManager.GetString("pattern_PostInTopicEmailPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to h1.New Topic in Forums: &quot;$TopicTitle&quot;:&quot;$TopicURL&quot;
        ///
        ///$Date &quot;$UserName&quot;:&quot;$UserURL&quot; has created a new topic:
        ///
        ///$PostText
        ///
        ///&quot;Read More&quot;:&quot;$PostURL&quot;
        ///
        ///Your portal address: &quot;$__VirtualRootPath&quot;:&quot;$__VirtualRootPath&quot;
        ///
        ///&quot;Edit subscription settings&quot;:&quot;$RecipientSubscriptionConfigURL&quot;.
        /// </summary>
        public static string pattern_TopicInForumEmailPattern {
            get {
                return ResourceManager.GetString("pattern_TopicInForumEmailPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Community. New Post in Forum Topic: $TopicTitle.
        /// </summary>
        public static string subject_PostInTopicEmailPattern {
            get {
                return ResourceManager.GetString("subject_PostInTopicEmailPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Community. New Topic in Forums: $TopicTitle.
        /// </summary>
        public static string subject_TopicInForumEmailPattern {
            get {
                return ResourceManager.GetString("subject_TopicInForumEmailPattern", resourceCulture);
            }
        }
    }
}
