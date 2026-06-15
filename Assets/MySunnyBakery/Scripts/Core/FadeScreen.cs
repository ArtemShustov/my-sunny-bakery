using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MySunnyBakery.Core {
	public class FadeScreen : MonoBehaviour {
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private GameObject _root;
		[SerializeField] private float _fadeDuration = 1f;

		private void Awake() {
			_root.SetActive(false);
		}

		public async UniTask FadeIn(CancellationToken token = default) {
			_root.SetActive(true);
			var time = 0f;
			while (time < _fadeDuration) {
				time += Time.deltaTime;
				_canvasGroup.alpha = Mathf.Clamp01(time / _fadeDuration);
				await UniTask.Yield(token);
			}
			_canvasGroup.alpha = 1f;
		}

		public async UniTask FadeOut(CancellationToken token = default) {
			var time = 0f;
			while (time < _fadeDuration) {
				time += Time.deltaTime;
				_canvasGroup.alpha = 1f - Mathf.Clamp01(time / _fadeDuration);
				await UniTask.Yield(token);
			}
			_canvasGroup.alpha = 0f;
			_root.SetActive(false);
		}
	}
}
