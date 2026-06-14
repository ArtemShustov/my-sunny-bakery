using System;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MySunnyBakery.Utils {
	public static class LocalizedStringExtensions {
		public static void Set(this LocalizedString localizedString, string key, LocalizedString value) {
			if (localizedString == null) {
				return;
			}
			localizedString[key] = value;
		}
		public static void Set(this LocalizedString localizedString, string key, string value) {
			localizedString?.Set(key, value, v => new StringVariable { Value = value });
		}
		public static void Set(this LocalizedString localizedString, string key, int value) {
			localizedString?.Set(key, value, v => new IntVariable { Value = value });
		}

		public static void Set<T>(this LocalizedString localizedString, string key, T value) {
			localizedString?.Set(key, value, v => new Variable<T> { Value = value });
		}
		public static void Set<T>(this LocalizedString localizedString, string key, T value, Func<T, IVariable> factory) {
			if (localizedString == null) {
				return;
			}
			if (localizedString.TryGetValue(key, out var variable) && variable is Variable<T> tVar) {
				tVar.Value = value;
			} else {
				localizedString[key] = factory(value);
			}
		}
	}
}
