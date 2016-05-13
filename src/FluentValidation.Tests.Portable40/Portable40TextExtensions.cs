namespace FluentValidation.Internal {
	using System;
	using System.Reflection;

	public static class Portable40TextExtensions {
		public static MethodInfo GetDeclaredMethod(this Type type, string name)
		{
#if PORTABLE || CoreCLR
			return type.GetTypeInfo().GetDeclaredMethod(name);
#else
			return type.GetMethod(name, new Type[0]);
#endif
		}

	}
}