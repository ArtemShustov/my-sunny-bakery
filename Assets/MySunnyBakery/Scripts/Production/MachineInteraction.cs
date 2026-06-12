using MySunnyBakery.Characters;
using MySunnyBakery.Interactions;
using UnityEngine;
using UnityEngine.Localization;

namespace MySunnyBakery.Production {
	[RequireComponent(typeof(IMachine))]
	public class MachineInteraction : MonoBehaviour, IInteraction {
		[SerializeField] private LocalizedString _hint;
		
		private IMachine _machine;

		private void Awake() {
			_machine = GetComponent<IMachine>();
		}

		public void Interact(InteractionContext context) {
			var hands = context.Invoker.GetComponent<Hands>();
			if (hands == null) {
				return;
			}

			if (hands.IsFree && _machine.CanTake()) {
				var item = _machine.Take();
				var pickable = item.GetComponent<Pickable>();
				hands.Take(pickable);
			} else if (!hands.IsFree && _machine.CanReceive(hands.Item.gameObject)) {
				_machine.Receive(hands.Item.gameObject);
				hands.Clear();
			}
		}

		public bool CanInteract(InteractionContext context) {
			var hands = context.Invoker.GetComponent<Hands>();
			if (hands == null) {
				return false;
			}

			return (!hands.IsFree && _machine.CanReceive(hands.Item.gameObject)) || _machine.CanTake();
		}

		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}
	}
}
