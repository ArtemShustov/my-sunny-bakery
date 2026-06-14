using System;

namespace MySunnyBakery.Utils {
	public class ReactiveProperty<T> {
		private T _value;

		public event Action<T> ValueChanged;

		public T Value {
			get => _value;
			set {
				_value = value;
				ValueChanged?.Invoke(_value);
			}
		}

		public ReactiveProperty(T value = default) {
			_value = value;
		}
	}
}
