using System;
using UnityEngine;

namespace MySunnyBakery.Production {
	public class Furnace : Machine {
		[SerializeField] private Transform _slotRoot;
		[SerializeField] private float _bakeDuration = 1f;
		private Bakeable _slot;
		
		public event Action WorkStarted;
		public event Action WorkStopped;
		public event Action<float> ProgressChanged;

		private void Update() {
			if (_slot == null) {
				return;
			}

			_slot.Progress += Time.deltaTime / _bakeDuration;
			ProgressChanged?.Invoke(_slot.Progress);
		}

		public override bool CanReceive(GameObject item) {
			return _slot == null && item.TryGetComponent<Bakeable>(out _);
		}
		public override void Receive(GameObject item) {
			if (_slot != null || !item.TryGetComponent<Bakeable>(out var bakeable)) {
				return;
			}

			_slot = bakeable;
			item.transform.SetParent(_slotRoot);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			WorkStarted?.Invoke();
		}

		public override bool CanTake() {
			return _slot != null;
		}
		public override GameObject Take() {
			if (_slot == null) {
				return null;
			}
			var item = _slot.gameObject;
			_slot = null;
			WorkStopped?.Invoke();
			return item;
		}
	}
}
