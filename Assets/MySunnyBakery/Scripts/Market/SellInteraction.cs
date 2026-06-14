using System;
using MySunnyBakery.Characters;
using MySunnyBakery.Core;
using MySunnyBakery.Interactions;
using MySunnyBakery.Items;
using UnityEngine;
using UnityEngine.Localization;
using VContainer;

namespace MySunnyBakery.Market {
	public class SellInteraction : MonoBehaviour, IInteraction {
		[SerializeField] private SellableItem[] _items;
		[SerializeField] private LocalizedString _hint;

		[Inject] private PlayerData _playerData;

		public event Action Sold;
		public event Action<LocalizedString> HintChanged;

		public void Interact(InteractionContext context) {
			if (!CanInteract(context)) {
				return;
			}

			if (!context.Invoker.TryGetComponent(out Hands hands)) {
				return;
			}

			var pickable = hands.Item;
			if (pickable == null) {
				return;
			}

			var item = pickable.GetComponent<Item>();
			if (item == null) {
				return;
			}

			foreach (var sellable in _items) {
				if (sellable.Item == item.Definition) {
					hands.Drop();
					_playerData.Money.Value += sellable.Price;
					Destroy(pickable.gameObject);
					Sold?.Invoke();
					return;
				}
			}
		}

		public bool CanInteract(InteractionContext context) {
			if (!context.Invoker.TryGetComponent(out Hands hands)) {
				return false;
			}

			if (hands.IsFree) {
				return false;
			}

			var pickable = hands.Item;
			if (pickable == null) {
				return false;
			}

			var item = pickable.GetComponent<Item>();
			if (item == null) {
				return false;
			}

			foreach (var sellable in _items) {
				if (sellable.Item == item.Definition) {
					return true;
				}
			}

			return false;
		}

		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}
		
		[Serializable]
		public struct SellableItem {
			public ItemDefinition Item;
			public int Price;
		}
	}
}
