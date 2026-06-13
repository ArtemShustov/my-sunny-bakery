using System.Collections.Generic;
using MySunnyBakery.Utils;
using UnityEngine;

namespace MySunnyBakery.Production {
	public class Bakeable: MonoBehaviour {
		[SerializeField] private float _progress;
		[SerializeField] private List<Renderer> _renderers = new();

		private static readonly int ProgressId = Shader.PropertyToID("_Progress");
		private MaterialPropertyBlock _propertyBlock;

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
			SetMaterialProgress(clamped);
			ProgressChanged?.Invoke(old, clamped);
		}

		private void SetMaterialProgress(float progress) {
			if (_renderers == null || _renderers.Count == 0) {
				return;
			}

			if (_propertyBlock == null) {
				_propertyBlock = new MaterialPropertyBlock();
			}

			_propertyBlock.SetFloat(ProgressId, progress);

			foreach (var renderer in _renderers) {
				if (renderer != null) {
					renderer.SetPropertyBlock(_propertyBlock);
				}
			}
		}

		private void OnEnable() {
			SetMaterialProgress(_progress);
		}
		private void OnValidate() {
			SetMaterialProgress(_progress);
		}
	}
}
