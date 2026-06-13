using UnityEngine;

namespace MySunnyBakery.Production {
	public class FormerVisuals : MonoBehaviour {
		[SerializeField] private Former _former;

		[Header("References")]
		[SerializeField] private Animator _animator;
		[SerializeField] private Transform _outputSlotRoot;

		[Header("Settings")]
		[SerializeField] private float _itemDestroyDelay = 0.1f;

		private static readonly int IsWorking = Animator.StringToHash("IsWorking");
		private GameObject _currentInputItem;

		private void DestroyInputItem() {
			if (_currentInputItem != null) {
				Destroy(_currentInputItem);
				_currentInputItem = null;
			}
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

			_currentInputItem = item;
			item.transform.SetParent(_outputSlotRoot);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
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
			DestroyInputItem();
		}
		public void OnLidOpened() {
		}
	}
}
