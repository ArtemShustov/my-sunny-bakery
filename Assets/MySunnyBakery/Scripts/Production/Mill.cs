using System;
using UnityEngine;
using MySunnyBakery.Interactions;
using MySunnyBakery.Items;

namespace MySunnyBakery.Production {
	public class Mill: MonoBehaviour, IMachine {
		[Header("Visual")]
		[SerializeField] private Transform _inputSlotRoot;
		[SerializeField] private Transform _outputSlotRoot;

		[Header("Settings")]
		[SerializeField] private float _grindDuration = 1f;
		[Space]
		[SerializeField] private ItemDefinition _inputDefinition;
		[SerializeField] private ItemDefinition _outputDefinition;

		private Item _inputSlot;
		private Item _outputSlot;
		private float _progress;

		public event Action WorkStarted;
		public event Action WorkStopped;
		public event Action ItemTaken;

		private void Update() {
			if (_inputSlot == null || _outputSlot != null) {
				return;
			}

			_progress += Time.deltaTime / _grindDuration;

			if (_progress >= 1f) {
				SpawnOutput();
				Destroy(_inputSlot.gameObject);
				_inputSlot = null;
				WorkStopped?.Invoke();
			}
		}

		public bool CanReceive(GameObject item) {
			if (_inputSlot != null || _outputSlot != null) {
				return false;
			}
			return item.TryGetComponent<Item>(out var candidate) && candidate.Definition == _inputDefinition;
		}

		public void Receive(GameObject item) {
			if (_inputSlot != null || !item.TryGetComponent<Item>(out var inputItem) || inputItem.Definition != _inputDefinition) {
				return;
			}

			_inputSlot = inputItem;
			item.transform.SetParent(_inputSlotRoot);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			_progress = 0f;
			WorkStarted?.Invoke();
		}

		private void SpawnOutput() {
			if (_outputDefinition == null) {
				return;
			}

			var spawned = _outputDefinition.Instantiate(_outputSlotRoot);
			if (spawned == null) {
				Debug.LogWarning("Spawned item is null!");
				return;
			}
			
			_outputSlot = spawned;
			if (spawned.TryGetComponent<Pickable>(out var pickable)) {
				pickable.OnPicked();
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
			ItemTaken?.Invoke();
			return item;
		}
	}
}
