using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MySunnyBakery.Production {
	public class FurnaceVisuals: MonoBehaviour {
		[Header("References")]
		[SerializeField] private Furnace _furnace;
		[SerializeField] private ParticleSystem _fireParticles;
		[SerializeField] private Transform _visualRoot;

		[Header("Jump Animation")]
		[SerializeField] private AnimationCurve _jumpCurve;
		[SerializeField] private float _jumpDuration = 0.3f;

		private CancellationTokenSource _animationCts;

		private async UniTask PlayJumpAnimation(CancellationToken cancellationToken) {
			while (true) {
				var elapsed = 0f;
				while (elapsed < _jumpDuration) {
					cancellationToken.ThrowIfCancellationRequested();
					elapsed += Time.deltaTime;
					var t = Mathf.Clamp01(elapsed / _jumpDuration);
					var curveValue = _jumpCurve.Evaluate(t);
					_visualRoot.localScale = new Vector3(2 - curveValue, curveValue, 2 - curveValue);
					await UniTask.Yield();
				}
			}
		}

		private void OnWorkStarted() {
			_fireParticles.Play();
			_animationCts?.Cancel();
			_animationCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
			PlayJumpAnimation(_animationCts.Token).Forget();
		}

		private void OnWorkStopped() {
			_fireParticles.Stop();
			
			_animationCts?.Cancel();
			_animationCts?.Dispose();
			_animationCts = null;
			
			_visualRoot.localScale = Vector3.one;
		}

		private void OnEnable() {
			_furnace.WorkStarted += OnWorkStarted;
			_furnace.WorkStopped += OnWorkStopped;
		}
		private void OnDisable() {
			_furnace.WorkStarted -= OnWorkStarted;
			_furnace.WorkStopped -= OnWorkStopped;
			
			_animationCts?.Cancel();
			_animationCts?.Dispose();
			_animationCts = null;
			
			_visualRoot.localScale = Vector3.one;
		}
	}
}
