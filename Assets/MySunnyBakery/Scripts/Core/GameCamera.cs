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

		public void SelectCharacterCamera() {
			_characterCamera.Priority = 10;
			_vehicleCamera.Priority = 0;
			_activeCamera = _characterCamera;
		}

		public void SelectVehicleCamera() {
			_vehicleCamera.Priority = 10;
			_characterCamera.Priority = 0;
			_activeCamera = _vehicleCamera;
		}

		public void SetTarget(Transform target) {
			_characterCamera.Follow = target;
			_characterCamera.LookAt = target;
			_vehicleCamera.Follow = target;
			_vehicleCamera.LookAt = target;
		}
	}
}
