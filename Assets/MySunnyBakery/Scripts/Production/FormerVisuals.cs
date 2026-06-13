using System.Collections.Generic;
using UnityEngine;

namespace MySunnyBakery.Production {
	public class FormerVisuals : MonoBehaviour {
		[SerializeField] private Former _former;

		[Header("References")]
		[SerializeField] private Animator _animator;
		[SerializeField] private Transform _inputSlotRoot;
		[SerializeField] private Transform _outputSlotRoot;

		[Header("Settings")]
		[SerializeField] private float _inputRadius = 0.5f;

		private static readonly int IsWorking = Animator.StringToHash("IsWorking");
		private readonly List<GameObject> _inputItems = new List<GameObject>();

		private void PlaceInputItems() {
			var count = _inputItems.Count;
			for (var i = 0; i < count; i++) {
				var angle = i * (360f / count) * Mathf.Deg2Rad;
				var offset = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * _inputRadius;
				var item = _inputItems[i];
				item.transform.SetParent(_inputSlotRoot);
				item.transform.localPosition = offset;
				item.transform.localRotation = Quaternion.LookRotation(-offset);
			}
		}

		private void DestroyInputItems() {
			foreach (var item in _inputItems) {
				if (item != null) {
					Destroy(item);
				}
			}

			_inputItems.Clear();
		}

		private void OnEnable() {
			_former.WorkStarted += OnWorkStarted;
			_former.WorkStopped += OnWorkStopped;
			_former.ItemTaken += OnItemTaken;
			_former.InputReceived += OnInputReceived;
			_former.OutputSpawned += OnOutputSpawned;
		}

		private void OnDisable() {
			_former.WorkStarted -= OnWorkStarted;
			_former.WorkStopped -= OnWorkStopped;
			_former.ItemTaken -= OnItemTaken;
			_former.InputReceived -= OnInputReceived;
			_former.OutputSpawned -= OnOutputSpawned;
		}

		private void OnWorkStarted() {
			_animator.SetBool(IsWorking, true);
		}

		private void OnWorkStopped() {
			_animator.SetBool(IsWorking, false);
		}

		private void OnItemTaken() {
		}

		private void OnInputReceived(GameObject item) {
			if (item == null) {
				return;
			}

			_inputItems.Add(item);
			PlaceInputItems();
		}

		private void OnOutputSpawned(GameObject item) {
			if (item == null) {
				return;
			}

			item.transform.SetParent(_outputSlotRoot);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
		}

		public void OnLidClosed() {
			DestroyInputItems();
		}

		public void OnLidOpened() {
		}

		private void OnDrawGizmosSelected() {
			if (_inputSlotRoot == null) {
				return;
			}

			Gizmos.color = Color.yellow;

			var segments = 32;
			var center = _inputSlotRoot.position;
			var prevPoint = center + _inputSlotRoot.TransformDirection(new Vector3(0f, 0f, _inputRadius));

			for (var i = 1; i <= segments; i++) {
				var angle = i * (360f / segments) * Mathf.Deg2Rad;
				var localOffset = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * _inputRadius;
				var nextPoint = center + _inputSlotRoot.TransformDirection(localOffset);
				Gizmos.DrawLine(prevPoint, nextPoint);
				prevPoint = nextPoint;
			}
		}
	}
}
