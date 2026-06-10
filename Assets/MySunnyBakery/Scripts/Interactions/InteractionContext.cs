using UnityEngine;

namespace MySunnyBakery.Interactions {
	public readonly struct InteractionContext {
		public readonly GameObject Invoker;

		public InteractionContext(GameObject invoker) {
			Invoker = invoker;
		}
	}
}
