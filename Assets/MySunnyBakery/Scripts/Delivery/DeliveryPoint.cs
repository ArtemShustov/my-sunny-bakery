using System;
using MySunnyBakery.Characters;
using MySunnyBakery.Interactions;
using MySunnyBakery.Items;
using UnityEngine;
using UnityEngine.Localization;
using VContainer;

namespace MySunnyBakery.Delivery {
	public class DeliveryPoint : MonoBehaviour, IInteraction {
		[SerializeField] private LocalizedString _hint;

		private DeliveryService _service;

		public State CurrentState { get; private set; }
		public ItemDefinition RequiredItem { get; private set; }

		public event Action<DeliveryPoint> Delivered;
		public event Action<LocalizedString> HintChanged;

		[Inject]
		private void Construct(DeliveryService deliveryService) {
			_service = deliveryService;
		}

		private void Awake() {
			_service?.RegisterPoint(this);
		}

		private void OnDestroy() {
			_service?.UnregisterPoint(this);
		}

		public void Activate(ItemDefinition requiredItem) {
			RequiredItem = requiredItem;
			CurrentState = State.Active;
		}
		public void Deactivate() {
			CurrentState = State.Inactive;
			RequiredItem = null;
		}

		public void Interact(InteractionContext context) {
			if (CurrentState != State.Active) {
				return;
			}
			if (!context.Invoker.TryGetComponent<Hands>(out var hands) || hands.IsFree) {
				return;
			}
			
			CurrentState = State.Completed;
			Destroy(hands.Item.gameObject);
			hands.Clear();
			
			Delivered?.Invoke(this);
		}

		public bool CanInteract(InteractionContext context) {
			if (CurrentState != State.Active) {
				return false;
			}
			if (!context.Invoker.TryGetComponent<Hands>(out var hands) || hands.IsFree) {
				return false;
			}
			if (!hands.Item.TryGetComponent<Item>(out var item)) {
				return false;
			}
			return item.Definition == RequiredItem;
		}

		public LocalizedString GetHint(InteractionContext context) {
			return CurrentState != State.Active ? null : _hint;
		}

		public enum State {
			Inactive,
			Active,
			Completed
		}
	}
}
