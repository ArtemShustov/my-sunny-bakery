using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MySunnyBakery.Characters;
using MySunnyBakery.Interactions;
using UnityEngine;
using UnityEngine.Localization;
using VContainer;

namespace MySunnyBakery.Core {
	public class BedInteraction: MonoBehaviour, IInteraction {
		[SerializeField] private LocalizedString _hint;

		private DayCycleService _dayCycle;
		private CancellationTokenSource _cts;
		
		public event Action<LocalizedString> HintChanged;

		[Inject]
		public void Construct(DayCycleService dayCycle) {
			_dayCycle = dayCycle;
		}
		
		public void Interact(InteractionContext context) {
			if (!context.Invoker.TryGetComponent<Character>(out var character)) {
				return;
			}
			_cts?.Dispose();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
			ExecuteAsync(character, _cts.Token).Forget();
		}
		public bool CanInteract(InteractionContext context) {
			return _cts == null && context.Invoker.TryGetComponent<Character>(out _);
		}
		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}

		private async UniTask ExecuteAsync(Character character, CancellationToken token = default) {
			var player = character.Input.Player;
			player.ReleaseControl(character.Input);

			try {
				await _dayCycle.GoNextDay(token);
			} 
			finally {
				_cts?.Dispose();
				_cts = null;
			}
			
			player.TakeControl(character.Input);
		} 
	}
}
