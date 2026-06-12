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
		[SerializeField] private Collider _collider;
		
		public void Interact(InteractionContext context) {
			context.Invoker.GetComponent<Hands>()?.Take(this);
		}
		public bool CanInteract(InteractionContext context) {
			return context.Invoker.TryGetComponent<Hands>(out var hands) && hands.IsFree;
		}
		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}

		public virtual void OnPicked() {
			_rigidbody.isKinematic = true;
			if (_collider != null) {
				_collider.enabled = false;
			}
		}
		public virtual void OnDropped() {
			transform.localRotation = Quaternion.identity;
			_rigidbody.isKinematic = false;
			if (_collider != null) {
				_collider.enabled = true;
			}
		}
		
		[Serializable]
		public struct InHandConfig {
			public Vector3 Offset;
			public Vector3 Rotation;
			public string Animation;
		}
	}
}
