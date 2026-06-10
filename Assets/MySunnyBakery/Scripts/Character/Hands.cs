using MySunnyBakery.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MySunnyBakery.Characters {
	[RequireComponent(typeof(Character))]
	public class Hands: MonoBehaviour {
		private Character _character;
		private Pickable _item;

		private static readonly int Empty = Animator.StringToHash("Empty");
		
		public bool IsFree => _item == null;
		public Pickable Item => _item;
		
		private void Awake() {
			_character = GetComponent<Character>();
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
			
			_item.OnPicked(this);
		}
		public void Drop() {
			if (_item == null) {
				return;
			}
			
			_item.transform.SetParent(null);
			_item.OnDropped(this);
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
			_character.Input.UseAlt += OnUse;
		}
		private void OnDisable() {
			_character.Input.UseAlt -= OnUse;
		}
		private void OnUse(InputAction.CallbackContext context) {
			if (context.performed) {
				Drop();
			}
		}
	}
}
