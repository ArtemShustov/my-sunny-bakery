using System;
using UnityEngine;
using MySunnyBakery.Core;
using MySunnyBakery.Utils;
using UnityEngine.InputSystem;

namespace MySunnyBakery.Characters {
	[DefaultExecutionOrder(-1)]
	public class CharacterInput: MonoBehaviour, IControllable {
		private Camera _camera;
		
		public Vector2 Move { get; private set; }
		public LocalPlayer Player { get; private set; }

		public event Action<InputAction.CallbackContext> Use;
		public event Action<InputAction.CallbackContext> UseAlt;

		private void Awake() {
			_camera = Camera.main;
		}
		private void Update() {
			if (Player == null) {
				return;
			}
			
			Move = GetRelatedMove();
		}

		public Vector2 GetRelatedMove() {
			var input = Player.Actions.Player.Move.ReadValue<Vector2>();
			
			if (!_camera) {
				return input;
			}

			var directionAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
			if (directionAngle < 0) {
				directionAngle += 360;
			}
			directionAngle += _camera.transform.eulerAngles.y;
			if (directionAngle > 360) {
				directionAngle -= 360;
			}
			var forward = Quaternion.Euler(0, directionAngle, 0) * Vector3.forward;
			var result = forward * input.magnitude;
			return new Vector2(result.x, result.z);
		}
		
		public void TakeControl(LocalPlayer player) {
			if (Player == player) {
				return;
			}

			UnsubscribeActions();
			Player = player;
			if (enabled) {
				SubscribeActions();
			}
		}
		public void ReleaseControl() {
			UnsubscribeActions();

			Player = null;
			Move = Vector2.zero;
		}
		public void ConfigureCamera(GameCamera camera) {
			if (camera == null) {
				return;
			}
			camera.SetTarget(transform);
			camera.SelectCharacterCamera();
		}

		private void SubscribeActions() {
			if (Player == null) {
				return;
			}
			UnsubscribeActions();

			Player.Actions.Player.Use.SubscribeAll(OnUse);
			Player.Actions.Player.UseAlt.SubscribeAll(OnUseAlt);
		}
		private void UnsubscribeActions() {
			if (Player == null) {
				return;
			}
			
			Player.Actions.Player.Use.UnsubscribeAll(OnUse);
			Player.Actions.Player.UseAlt.UnsubscribeAll(OnUseAlt);
		}

		private void OnUse(InputAction.CallbackContext context) {
			Use?.Invoke(context);
		}
		private void OnUseAlt(InputAction.CallbackContext context) {
			UseAlt?.Invoke(context);
		}
		
		private void OnEnable() {
			SubscribeActions();
		}
		private void OnDisable() {
			UnsubscribeActions();
		}
	}
}
