using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace MySunnyBakery.Core {
	public class LocalPlayer: MonoBehaviour {
		[SerializeField] private GameCamera _camera;
		public InputActions Actions { get; private set; }
		private InputUser _user;

		public void Awake() {
			Actions = new InputActions();
			Actions.Enable();
			
			_user = InputUser.CreateUserWithoutPairedDevices();
			_camera.Initialize(this);

			InputUser.listenForUnpairedDeviceActivity += 1;
			InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUser;
		}
		public void OnDestroy() {
			Actions.Disable();
			_user.UnpairDevicesAndRemoveUser();
			
			InputUser.listenForUnpairedDeviceActivity -= 1;
			InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUser;
		}

		public void TakeControl(IControllable controllable) {
			controllable.TakeControl(this);
			controllable.ConfigureCamera(_camera);
		}
		public void ReleaseControl(IControllable controllable) {
			controllable.ReleaseControl();
		}
		
		public void PairDevice(InputDevice device) {
			InputUser.PerformPairingWithDevice(device, _user);
		}
		public void UnpairDevices() {
			_user.UnpairDevices();
		}
		public bool IsPaired(InputDevice device) {
			return _user.pairedDevices.Contains(device);
		}
		
		private void OnUnpairedDeviceUser(InputControl control, InputEventPtr @event) {
			OnDeviceTriggered(control.device);
		}
		private void OnDeviceTriggered(InputDevice device) {
			var isKeyboardOrMouse = device is Keyboard or Mouse;

			var isDeviceUsed = IsPaired(device);
			if (!isDeviceUsed) {
				UnpairDevices();
				if (isKeyboardOrMouse) {
					PairDevice(Mouse.current);
					PairDevice(Keyboard.current);
				} else {
					PairDevice(device);
				}
			}
		}
	}
}
