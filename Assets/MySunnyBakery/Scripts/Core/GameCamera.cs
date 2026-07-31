using MySunnyBakery.Utils;
using UnityEngine;
using Unity.Cinemachine;

namespace MySunnyBakery.Core {
	public class GameCamera: MonoBehaviour {
		[Header("Character Camera")]
		[SerializeField] private CinemachineCamera _characterCamera;
		[SerializeField] private CameraInput _characterCameraInput;

		[Header("Vehicle Camera")]
		[SerializeField] private CinemachineCamera _vehicleCamera;
		[SerializeField] private CameraInput _vehicleCameraInput;

		private LocalPlayer _player;
		private Transform _target;
		private CinemachineCamera _activeCamera;

		private void Awake() {
			_characterCamera.Priority = 0;
			_vehicleCamera.Priority = 0;
			_activeCamera = null;
		}
		
		public void Initialize(LocalPlayer player) {
			_player = player;
			_characterCameraInput.Input = _player.Actions;
			_vehicleCameraInput.Input = _player.Actions;
		}

		public void SwitchCamera(CinemachineCamera cam) {
			if (_activeCamera != null) {
				_activeCamera.Priority = 0;
			}
			_activeCamera = cam;
			if (_activeCamera != null) {
				_activeCamera.Priority = 10;
			}

			SetTarget(_target);
		}
		
		public void SelectCharacterCamera() {
			SwitchCamera(_characterCamera);
		}

		public void SelectVehicleCamera() {
			SwitchCamera(_vehicleCamera);
		}

		public void SetTarget(Transform target) {
			_target = target;

			if (_activeCamera == null) {
				return;
			}
			_activeCamera.Target = new CameraTarget() {
				TrackingTarget = target,
				LookAtTarget = target,
				CustomLookAtTarget = true,
			};
		}
	}
}
