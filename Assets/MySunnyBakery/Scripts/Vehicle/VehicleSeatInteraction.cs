using System;
using MySunnyBakery.Core;
using UnityEngine;
using UnityEngine.Localization;
using MySunnyBakery.Interactions;

namespace MySunnyBakery.Vehicles {
	public class VehicleSeatInteraction : MonoBehaviour, IInteraction {
		[SerializeField] private VehicleInput _vehicle;
		[SerializeField] private LocalizedString _hint;
		[SerializeField] private Transform _exitPoint;
		
		private (GameObject gameObject, IControllable controllable) _character;

		public event Action<LocalizedString> HintChanged; 

		public void Interact(InteractionContext context) {
			if (!context.Invoker.TryGetComponent<IControllable>(out var character)) {
				return;
			}

			var player = character.Player;
			player.ReleaseControl(character);
			player.TakeControl(_vehicle);
			
			context.Invoker.SetActive(false);
			_character = (context.Invoker, character);
		}
		public bool CanInteract(InteractionContext context) {
			if (_vehicle == null || _vehicle.Player != null || context.Invoker == null) {
				return false;
			}
			return context.Invoker.TryGetComponent<IControllable>(out _);
		}
		public LocalizedString GetHint(InteractionContext context) {
			return _hint;
		}
		
		private void Exit() {
			var player = _vehicle.Player;
			player.ReleaseControl(_vehicle);

			_character.gameObject.transform.position = _exitPoint.position;
			_character.gameObject.SetActive(true);
			
			player.TakeControl(_character.controllable);
		}
		
		private void OnEnable() {
			_vehicle.Exit += Exit;
		}
		private void OnDisable() {
			_vehicle.Exit -= Exit;
		}
	}
}
