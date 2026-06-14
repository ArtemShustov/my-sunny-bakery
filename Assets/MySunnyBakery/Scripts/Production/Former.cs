using System;
using UnityEngine;
using MySunnyBakery.Interactions;
using MySunnyBakery.Items;

namespace MySunnyBakery.Production {
	public class Former : MonoBehaviour, IMachine {
		[Header("Settings")]
		[SerializeField] private float _duration = 2f;
		[Space]
		[SerializeField] private ItemDefinition _requiredA;
		[SerializeField] private ItemDefinition _requiredB;
		[SerializeField] private ItemDefinition _output;

		private Item _slotA;
		private Item _slotB;
		private Item _outputSlot;
		private float _progress;
		private bool _isWorking;

		public event Action WorkStarted;
		public event Action WorkStopped;
		public event Action ItemTaken;
		public event Action<GameObject> InputReceived;
		public event Action<GameObject> OutputSpawned;

		private void Update() {
			if (_isWorking) {
				_progress += Time.deltaTime / _duration;
				if (_progress >= 1f) {
					FinishWork();
				}
			}
		}

		public bool CanReceive(GameObject item) {
			if (_isWorking || _outputSlot != null) {
				return false;
			}

			if (!item.TryGetComponent<Item>(out var candidate)) {
				return false;
			}

			var def = candidate.Definition;

			if (_slotA == null && def == _requiredA) {
				return true;
			}

			if (_slotB == null && def == _requiredB) {
				return true;
			}

			return false;
		}

		public void Receive(GameObject item) {
			if (_isWorking) {
				return;
			}

			if (!item.TryGetComponent<Item>(out var inputItem)) {
				return;
			}

			var def = inputItem.Definition;

			if (_slotA == null && def == _requiredA) {
				_slotA = inputItem;
			} else if (_slotB == null && def == _requiredB) {
				_slotB = inputItem;
			} else {
				return;
			}

			InputReceived?.Invoke(item);

			if (_slotA != null && _slotB != null) {
				StartWork();
			}
		}

		private void StartWork() {
			_progress = 0f;
			_isWorking = true;

			if (_outputSlot != null) {
				Destroy(_outputSlot.gameObject);
				_outputSlot = null;
			}

			WorkStarted?.Invoke();
		}

		private void FinishWork() {
			_isWorking = false;
			
			_slotA = null;
			_slotB = null;

			SpawnOutput();
			WorkStopped?.Invoke();
		}

		private void SpawnOutput() {
			if (_output == null) {
				return;
			}

			var spawnedObject = _output.Instantiate(null);
			if (spawnedObject == null) {
				Debug.LogWarning("Spawned item is null!");
				return;
			}

			_outputSlot = spawnedObject;
			OutputSpawned?.Invoke(spawnedObject.gameObject);

			if (spawnedObject.TryGetComponent<Pickable>(out var pickable)) {
				pickable.OnPicked();
			}
		}

		public bool CanTake() {
			return _outputSlot != null && !_isWorking;
		}

		public GameObject Take() {
			if (_outputSlot == null) {
				return null;
			}

			var item = _outputSlot.gameObject;
			_outputSlot = null;
			ItemTaken?.Invoke();
			return item;
		}
	}
}
