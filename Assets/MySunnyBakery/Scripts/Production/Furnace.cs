using UnityEngine;

namespace MySunnyBakery.Production {
	public class Furnace: MonoBehaviour, IMachine {
		[SerializeField] private Transform _slotRoot;
		[SerializeField] private float _bakeDuration = 1f;
		private Bakeable _slot;
		
		public bool CanReceive(GameObject item) {
			return _slot == null && item.TryGetComponent<Bakeable>(out _);
		}
		public void Receive(GameObject item) {
			if (_slot != null || !item.TryGetComponent<Bakeable>(out var bakeable)) {
				return;
			}
			
			_slot = bakeable;
			item.transform.SetParent(_slotRoot);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
		}

		private void Update() {
			if (_slot != null) {
				_slot.Progress += Time.deltaTime / _bakeDuration;
			}
		}

		public bool CanTake() {
			return _slot != null;
		}
		public GameObject Take() {
			var item = _slot.gameObject;
			_slot = null;
			return item;
		}
	}
}
