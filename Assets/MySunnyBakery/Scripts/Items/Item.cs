using MySunnyBakery.Utils;
using UnityEngine;

namespace MySunnyBakery.Items {
	public class Item : MonoBehaviour {
		[ReadOnly]
		[SerializeField] private ItemDefinition _definition;

		public ItemDefinition Definition => _definition;

		public void Init(ItemDefinition definition) {
			_definition = definition;
		}
	}
}
