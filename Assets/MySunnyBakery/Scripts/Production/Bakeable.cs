using MySunnyBakery.Utils;
using UnityEngine;

namespace MySunnyBakery.Production {
	public class Bakeable: MonoBehaviour {
		[SerializeField] private float _progress;

		public float Progress {
			get => _progress;
			set => SetProgress(value);
		}
		
		public event ValueChanged<float> ProgressChanged;

		public void SetProgress(float value) {
			var old = _progress;
			var clamped = Mathf.Clamp(value, 0, 2);
			
			if (Mathf.Approximately(old, clamped)) {
				return;
			}
			_progress = clamped;
			ProgressChanged?.Invoke(old, clamped);
		}
	}
}
