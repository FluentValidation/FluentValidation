namespace FluentValidation.Resources {
	using System;
	using System.Reflection;
	using Internal;

	/// <summary>
	/// Defines an accessor for localization resources
	/// </summary>
    public class ResourceAccessor {
        /// <summary>
        /// Function that can be used to retrieve the resource
        /// </summary>
        public Func<string> Accessor { get; set; }
        /// <summary>
        /// Resource type
        /// </summary>
        public Type ResourceType { get; set; }
        /// <summary>
        /// Resource name
        /// </summary>
        public string ResourceName { get; set; }
    }

}