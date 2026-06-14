using System;
using MySunnyBakery.Characters;
using MySunnyBakery.Core;
using MySunnyBakery.Interactions;
using MySunnyBakery.Items;
using MySunnyBakery.Utils;
using UnityEngine;
using UnityEngine.Localization;
using VContainer;

namespace MySunnyBakery.Market {
	public class Stall : MonoBehaviour, IInteraction {
		[SerializeField] private ItemDefinition _item;
		[SerializeField, Min(0)] private int _price;
		[SerializeField, Min(0)] private int _count;
		[SerializeField] private LocalizedString _hint;

		[Inject] private PlayerData _playerData;

		public event Action Purchased;
		public event ValueChanged<int> PriceChanged;
		public event ValueChanged<int> CountChanged;

		public int Price {
			get => _price;
			set {
				var old = _price;
				_price = Mathf.Max(0, value);
				PriceChanged?.Invoke(old, _price);
			}
		}

		public int Count {
			get => _count;
			set {
				var old = _count;
				_count = Mathf.Max(0, value);
				CountChanged?.Invoke(old, _count);
			}
		}

		public void Interact(InteractionContext context) {
			if (!CanInteract(context)) {
				return;
			}

			_playerData.Money.Value -= Price;
			Count--;

			var instance = _item.Instantiate(context.Invoker.transform);
			if (context.Invoker.TryGetComponent(out Hands hands)) {
				hands.Take(instance.GetComponent<Pickable>());
			}
			
			Purchased?.Invoke();
		}

		public bool CanInteract(InteractionContext context) {
			if (Count <= 0) {
				return false;
			}

			if (!context.Invoker.TryGetComponent(out Hands hands)) {
				return false;
			}

			return hands.IsFree && _playerData.Money.Value >= Price;
		}

		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}
	}
}
