using System;
using UnityEngine.InputSystem;

namespace MySunnyBakery.Utils {
	public static class InputActionsExtensions {
		public static void SubscribeAll(this InputAction action, Action<InputAction.CallbackContext> callback) {
			action.started += callback;
			action.performed += callback;
			action.canceled += callback;
		}
		public static void UnsubscribeAll(this InputAction action, Action<InputAction.CallbackContext> callback) {
			action.started -= callback;
			action.performed -= callback;
			action.canceled -= callback;
		}
	}
}
