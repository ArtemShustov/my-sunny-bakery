using UnityEngine;
using MySunnyBakery.Interactions;

namespace MySunnyBakery.Production {
	public class Mill: MonoBehaviour, IMachine {
		[Header("Visual")]
		[SerializeField] private Transform _inputSlotRoot;
		[SerializeField] private Transform _outputSlotRoot;

		[Header("Settings")]
		[SerializeField] private float _grindDuration = 1f;
		[Space]
		[SerializeField] private string _inputId;
		[SerializeField] private GameObject _outputPrefab;

		private Item _inputSlot;
		private Item _outputSlot;
		private float _progress;

		public bool CanReceive(GameObject item) {
			if (_inputSlot != null) {
				return false;
			}
			return item.TryGetComponent<Item>(out var candidate) && candidate.Id == _inputId;
		}

		public void Receive(GameObject item) {
			if (_inputSlot != null || !item.TryGetComponent<Item>(out var inputItem) || inputItem.Id != _inputId) {
				return;
			}

			_inputSlot = inputItem;
			item.transform.SetParent(_inputSlotRoot);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			_progress = 0f;
		}

		private void Update() {
			if (_inputSlot == null || _outputSlot != null) {
				return;
			}

			_progress += Time.deltaTime / _grindDuration;

			if (_progress >= 1f) {
				SpawnOutput();
				Destroy(_inputSlot.gameObject);
				_inputSlot = null;
			}
		}

		private void SpawnOutput() {
			if (_outputPrefab == null) {
				return;
			}

			var spawned = Instantiate(_outputPrefab, _outputSlotRoot);
			spawned.transform.localPosition = Vector3.zero;
			spawned.transform.localRotation = Quaternion.identity;

			if (spawned.TryGetComponent<Item>(out var item)) {
				_outputSlot = item;
			}

			if (spawned.TryGetComponent<Pickable>(out var pickable)) {
				pickable.OnPicked();
				Debug.Log("Picked " + pickable.gameObject.name);
			}
		}

		public bool CanTake() {
			return _outputSlot != null;
		}

		public GameObject Take() {
			if (_outputSlot == null) {
				return null;
			}

			var item = _outputSlot.gameObject;
			_outputSlot = null;
			return item;
		}
	}
}
