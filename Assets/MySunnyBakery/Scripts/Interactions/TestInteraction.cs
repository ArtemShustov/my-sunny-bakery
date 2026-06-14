using System;
using UnityEngine;
using UnityEngine.Localization;

namespace MySunnyBakery.Interactions {
	public class TestInteraction: MonoBehaviour, IInteraction {
		[SerializeField] private Behaviour _behaviour = Behaviour.Always;
		[SerializeField] private float _cooldownDuration = 5f;
		[SerializeField] private LocalizedString _hint;

		private bool _used;
		private float _lastInteractionTime;

		public event Action<LocalizedString> HintChanged;

		public void Interact(InteractionContext context) {
			switch (_behaviour) {
				case Behaviour.Always:
					AlwaysInteract();
					break;
				case Behaviour.Once:
					OnceInteract();
					break;
				case Behaviour.Cooldown:
					CooldownInteract();
					break;
			}
		}

		public bool CanInteract(InteractionContext context) {
			switch (_behaviour) {
				case Behaviour.Always:
					return true;
				case Behaviour.Once:
					return !_used;
				case Behaviour.Cooldown:
					return Time.time - _lastInteractionTime >= _cooldownDuration;
				default:
					return false;
			}
		}

		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}

		private void AlwaysInteract() {
			Debug.Log($"[TestInteraction] Interacted with '{gameObject.name}' (Always)", gameObject);
		}

		private void OnceInteract() {
			if (_used) {
				return;
			}

			_used = true;
			Debug.Log($"[TestInteraction] Interacted with '{gameObject.name}' (Once)", gameObject);
		}

		private void CooldownInteract() {
			_lastInteractionTime = Time.time;
			Debug.Log($"[TestInteraction] Interacted with '{gameObject.name}' (Cooldown)", gameObject);
		}
		
		public enum Behaviour {
			Always,
			Once,
			Cooldown
		}
	}
}