using System.Collections.Generic;
using UnityEngine;
using MySunnyBakery.Characters;
using UnityEngine.InputSystem;

namespace MySunnyBakery.Interactions {
	public class PlayerInteractor: MonoBehaviour {
		[SerializeField] private Character _character;
		[SerializeField] private float _interactionRadius = 3f;
		[SerializeField] private LayerMask _interactionLayer = ~0;

		private IInteraction _selected;
		private readonly List<IInteraction> _available = new();
		private readonly Collider[] _overlap = new Collider[16];

		private void Awake() {
			if (_character == null) {
				_character = GetComponent<Character>();
			}
		}
		private void Update() {
			FindAvailableInteractions();
			SelectClosest();
		}

		private void FindAvailableInteractions() {
			_available.Clear();

			var size = Physics.OverlapSphereNonAlloc(transform.position, _interactionRadius, _overlap, _interactionLayer);
			var context = GetContext();

			for (var i = 0; i < size; i++) {
				var col = _overlap[i];
				if (!col.TryGetComponent<IInteraction>(out var interaction)) {
					continue;
				}

				if (interaction.CanInteract(context)) {
					_available.Add(interaction);
				}
			}
		}
		private void SelectClosest() {
			_selected = null;

			if (_available.Count == 0) {
				return;
			}

			var position = transform.position;
			float closestSqr = float.MaxValue;

			foreach (var interaction in _available) {
				if (interaction is MonoBehaviour mb) {
					var sqr = (mb.transform.position - position).sqrMagnitude;
					if (sqr < closestSqr) {
						closestSqr = sqr;
						_selected = interaction;
					}
				}
			}
		}
		private InteractionContext GetContext() {
			return new InteractionContext(_character.gameObject);
		}
		
		public void Use() {
			if (_selected == null) {
				return;
			}
			
			var context = GetContext();
			if (_selected.CanInteract(context)) {
				_selected.Interact(context);
			}
		}

		private void OnEnable() {
			_character.Input.Use += OnUsePerformed;
		}
		private void OnDisable() {
			_character.Input.Use -= OnUsePerformed;
		}

		private void OnUsePerformed(InputAction.CallbackContext context) {
			if (context.performed) {
				Use();
			}
		}

		private void OnDrawGizmosSelected() {
			var position = transform.position;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(position, _interactionRadius);

			if (!Application.isPlaying) {
				return;
			}

			var context = GetContext();

			Gizmos.color = Color.white;
			foreach (var col in Physics.OverlapSphere(position, _interactionRadius, _interactionLayer)) {
				if (col.TryGetComponent<IInteraction>(out var interaction) && interaction.CanInteract(context)) {
					Gizmos.DrawLine(position, col.transform.position);
				}
			}

			if (_selected is MonoBehaviour selectedMb) {
				Gizmos.color = Color.green;
				Gizmos.DrawLine(position, selectedMb.transform.position);
			}
		}
	}
}
