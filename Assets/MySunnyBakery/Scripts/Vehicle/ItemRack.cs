using System.Collections.Generic;
using MySunnyBakery.Items;
using UnityEngine;

namespace MySunnyBakery.Vehicles {
	public class ItemRack {
		public ItemDefinition Definition;
		public Vector2Int Size = Vector2Int.one;
		public List<GameObject> Items = new List<GameObject>();

		public bool IsFull => Items.Count >= Definition.MaxStack;

		public bool CanAdd() {
			return Items.Count < Definition.MaxStack;
		}
		public void Add(GameObject item) {
			if (item == null || IsFull) {
				return;
			}

			Items.Add(item);
		}

		public bool CanTake() {
			return Items.Count > 0;
		}
		public GameObject Take() {
			if (Items.Count == 0) {
				return null;
			}

			var lastItem = Items[^1];
			Items.RemoveAt(Items.Count - 1);
			return lastItem;
		}
	}
}
