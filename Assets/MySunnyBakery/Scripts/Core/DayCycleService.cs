using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace MySunnyBakery.Core {
	public class DayCycleService : MonoBehaviour {
		private FadeScreen _fadeScreen;

		private int _days;

		public event Action DayStarted;
		public event Action DayEnded;

		[Inject]
		public void Construct(FadeScreen fadeScreen) {
			_fadeScreen = fadeScreen;
		}

		private void Start() {
			DayStarted?.Invoke();
		}

		public async UniTask GoNextDay(CancellationToken token = default) {
			using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, destroyCancellationToken);
			var linkedToken = linkedTokenSource.Token;

			await _fadeScreen.FadeIn(linkedToken);
			DayEnded?.Invoke();
			_days++;
			Debug.Log($"Day {_days} started");
			await _fadeScreen.FadeOut(linkedToken);
			DayStarted?.Invoke();
		}
	}
}
