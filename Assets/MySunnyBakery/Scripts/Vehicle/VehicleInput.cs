using System;
using UnityEngine;
using MySunnyBakery.Core;

namespace MySunnyBakery.Vehicles {
	[DefaultExecutionOrder(-1)]
	public class VehicleInput: MonoBehaviour, IControllable {
		[SerializeField] private ArcadeVP.ArcadeVehicleController _vehicleController;

		public LocalPlayer Player { get; private set; }
		public event Action Exit;

		private void Update() {
			if (_vehicleController == null) {
				return;
			}
			if (Player == null) {
				_vehicleController.ProvideInputs(0, 0, 1);
				return;
			}

			if (Player.Actions.Vehicle.Exit.WasPerformedThisFrame()) {
				Exit?.Invoke();
				return;
			}

			var steering = Player.Actions.Vehicle.Turn.ReadValue<float>();
			var acceleration = Player.Actions.Vehicle.Throttle.ReadValue<float>();
			var brake = Player.Actions.Vehicle.Brake.ReadValue<float>();
			_vehicleController.ProvideInputs(steering, acceleration, brake);
		}

		public void TakeControl(LocalPlayer player) {
			Player = player;
		}
		public void ReleaseControl() {
			if (Player != null) {
				_vehicleController.ProvideInputs(0, 0, 0);
				Player = null;
			}
		}

		public void ConfigureCamera(GameCamera camera) {
			if (camera == null) {
				return;
			}
			camera.SetTarget(transform);
			camera.SelectVehicleCamera();
		}
	}
}
