using System;
using MySunnyBakery.Characters;
using UnityEngine;
using UnityEngine.Localization;

namespace MySunnyBakery.Interactions {
	public class Pickable: MonoBehaviour, IInteraction {
		[field: SerializeField] public InHandConfig InHand { get; private set; }
		[Space]
		[SerializeField] private LocalizedString _hint;
		[SerializeField] private Rigidbody _rigidbody;

		public event Action<LocalizedString> HintChanged;

		public void Interact(InteractionContext context) {
			context.Invoker.GetComponent<Hands>()?.Take(this);
		}
		public bool CanInteract(InteractionContext context) {
			return !_rigidbody.isKinematic 
			       && context.Invoker.TryGetComponent<Hands>(out var hands) 
			       && hands.IsFree;
		}
		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}

		public virtual void OnPicked() {
			_rigidbody.isKinematic = true;
		}
		public virtual void OnDropped() {
			transform.localRotation = Quaternion.identity;
			_rigidbody.isKinematic = false;
		}
		
		[Serializable]
		public struct InHandConfig {
			public Vector3 Offset;
			public Vector3 Rotation;
			public string Animation;
		}
	}
}