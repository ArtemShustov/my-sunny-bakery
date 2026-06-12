using MySunnyBakery.Interactions;
using UnityEngine;
using UnityEngine.Localization;

namespace MySunnyBakery.Characters {
	[RequireComponent(typeof(Character))]
	public class Hands: MonoBehaviour {
		[SerializeField] private Character _character;
		[SerializeField] private PlayerInteractor _interactor;
		private Pickable _item;
		private DropItemInteraction _dropItemInteraction;
		
		private static readonly int Empty = Animator.StringToHash("Empty");
		
		public bool IsFree => _item == null;
		public Pickable Item => _item;

		private void Awake() {
			_dropItemInteraction = new DropItemInteraction(this);
		}

		public void Take(Pickable pickable) {
			if (!IsFree) {
				Drop();
			}
			if (pickable == null) {
				return;
			}
			
			_item = pickable;
			var config = _item.InHand;
			
			_item.transform.SetParent(_character.Model.RightHandSlot);
			_item.transform.localPosition = config.Offset;
			_item.transform.localEulerAngles = config.Rotation;
			if (string.IsNullOrEmpty(config.Animation)) {
				SetAnim(config.Animation);
			}
			
			_item.OnPicked();
		}
		public void Drop() {
			if (_item == null) {
				return;
			}
			
			_item.transform.SetParent(null);
			_item.OnDropped();
			_item = null;
			ClearAnim();
		}
		public void Clear() {
			if (_item == null) {
				return;
			}

			_item = null;
			ClearAnim();
		}

		private void SetAnim(string id) {
			_character.Model.Animator.CrossFadeInFixedTime(id, 0.1f, 1);
		}
		private void ClearAnim() {
			_character.Model.Animator.CrossFadeInFixedTime(Empty, 0.1f, 1);
		}
		
		private void OnEnable() {
			_interactor.AddDefaultHoldInteraction(_dropItemInteraction);
		}
		private void OnDisable() {
			_interactor.RemoveDefaultHoldInteraction(_dropItemInteraction);
		}
		
		private class DropItemInteraction: IHoldInteraction {
			private readonly Hands _hands;
			
			public DropItemInteraction(Hands hands) {
				_hands = hands;
			}
			
			public void HoldInteract(InteractionContext context) {
				_hands.Drop();
			}
			public bool CanHoldInteract(InteractionContext context) {
				return !_hands.IsFree;
			}
			public LocalizedString GetHoldHint(InteractionContext context) {
				throw new System.NotImplementedException();
			}
		}
	}
}
