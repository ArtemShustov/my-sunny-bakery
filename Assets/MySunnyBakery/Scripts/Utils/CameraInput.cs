using System;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MySunnyBakery.Utils {
	public class CameraInput: InputAxisControllerBase<CameraInput.Reader> {
		public InputActions Input;

		public float LookX => Input?.Player.Look.ReadValue<Vector2>().x ?? 0;
		public float LookY => Input?.Player.Look.ReadValue<Vector2>().y ?? 0;
		public float Zoom => Input?.Player.Zoom.ReadValue<Vector2>().y ?? 0;

		private void Update() {
			if (Application.isPlaying) {
				UpdateControllers();
			}
		}

		public float GetLookValue(IInputAxisOwner.AxisDescriptor.Hints hint) {
			return hint switch {
				IInputAxisOwner.AxisDescriptor.Hints.Default => 0,
				IInputAxisOwner.AxisDescriptor.Hints.X => LookX,
				IInputAxisOwner.AxisDescriptor.Hints.Y => LookY,
				_ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
			};
		}

		[Serializable]
		public class Reader: IInputAxisReader {
			public bool IsZoom;
			
			public float GetValue(Object context, IInputAxisOwner.AxisDescriptor.Hints hint) {
				if (context is not CameraInput camera) {
					return 0;
				}
				return IsZoom ? camera.Zoom : camera.GetLookValue(hint);
			}
		}
	}
}
