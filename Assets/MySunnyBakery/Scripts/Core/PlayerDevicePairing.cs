using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace MySunnyBakery.Core {
	[RequireComponent(typeof(LocalPlayer))]
	public class PlayerDevicePairing: MonoBehaviour {
		public bool AutoPair { get; set; } = true;
		
		private LocalPlayer _player;
		private InputUser _user;

		private void Awake() {
			_player = GetComponent<LocalPlayer>();
			
			_user = InputUser.CreateUserWithoutPairedDevices();

			InputUser.listenForUnpairedDeviceActivity += 1;
			InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUser;
		}

		private void Start() {
			_user.AssociateActionsWithUser(_player.Actions);
		}

		private void OnDestroy() {
			_user.UnpairDevicesAndRemoveUser();
			
			InputUser.listenForUnpairedDeviceActivity -= 1;
			InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUser;
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
			if (!AutoPair) {
				return;
			}
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
