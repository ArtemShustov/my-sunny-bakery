using UnityEngine;
using MySunnyBakery.Characters;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

namespace MySunnyBakery.Interactions {
	public class PlayerInteractor : MonoBehaviour {
		[SerializeField] private Character _character;
		[SerializeField] private float _interactionRadius = 3f;
		[SerializeField] private LayerMask _interactionLayer = ~0;

		private IInteraction _selected;
		private IHoldInteraction _selectedHold;
		private readonly Collider[] _overlap = new Collider[16];
		private readonly List<IHoldInteraction> _defaultHoldStack = new List<IHoldInteraction>();

		private void Awake() {
			if (_character == null) {
				_character = GetComponent<Character>();
			}
		}

		private void Update() {
			UpdateSelection();
		}

		private void UpdateSelection() {
			_selected = null;
			_selectedHold = null;

			var context = GetContext();
			var position = transform.position;

			var closestInteraction = float.MaxValue;
			var closestHold = float.MaxValue;

			var count = Physics.OverlapSphereNonAlloc(
				position,
				_interactionRadius,
				_overlap,
				_interactionLayer
			);

			for (var i = 0; i < count; i++) {
				var collider = _overlap[i];

				if (collider.TryGetComponent<IInteraction>(out var interaction) && interaction.CanInteract(context)) {
					var closestPoint = collider.ClosestPoint(position);
					var sqrDistance = (closestPoint - position).sqrMagnitude;
					
					if (sqrDistance < closestInteraction) {
						closestInteraction = sqrDistance;
						_selected = interaction;
					}
				}

				if (collider.TryGetComponent<IHoldInteraction>(out var holdInteraction) && holdInteraction.CanHoldInteract(context)) {
					var closestPoint = collider.ClosestPoint(position);
					var sqrDistance = (closestPoint - position).sqrMagnitude;
					
					if (sqrDistance < closestHold) {
						closestHold = sqrDistance;
						_selectedHold = holdInteraction;
					}
				}
			}
		}

		private InteractionContext GetContext() {
			return new InteractionContext(_character.gameObject);
		}

		private void ExecuteSelected() {
			if (_selected == null) {
				return;
			}

			var context = GetContext();
			if (_selected.CanInteract(context)) {
				_selected.Interact(context);
			}
		}
		private void ExecuteHoldSelected() {
			_defaultHoldStack.RemoveAll(x => x == null);
			
			var context = GetContext();
			var target = _selectedHold != null && _selectedHold.CanHoldInteract(context) 
				? _selectedHold 
				: _defaultHoldStack.LastOrDefault(x => x != null && x.CanHoldInteract(context));
			
			if (target != null) {
				target.HoldInteract(context);
			}
		}

		public void AddDefaultHoldInteraction(IHoldInteraction holdInteraction) {
			if (holdInteraction != null && !_defaultHoldStack.Contains(holdInteraction)) {
				_defaultHoldStack.Add(holdInteraction);
			}
		}
		public void RemoveDefaultHoldInteraction(IHoldInteraction holdInteraction) {
			_defaultHoldStack.Remove(holdInteraction);
		}

		private void OnEnable() {
			_character.Input.Use += OnUsePerformed;
			_character.Input.UseAlt += OnUseAltPerformed;
		}
		private void OnDisable() {
			_character.Input.Use -= OnUsePerformed;
			_character.Input.UseAlt -= OnUseAltPerformed;
		}

		private void OnUsePerformed(InputAction.CallbackContext context) {
			if (context.performed) {
				ExecuteSelected();
			}
		}
		private void OnUseAltPerformed(InputAction.CallbackContext context) {
			if (context.performed) {
				ExecuteHoldSelected();
			}
		}
		
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, _interactionRadius);

			if (!Application.isPlaying) {
				return;
			}

			if (_selected is MonoBehaviour selectedMono) {
				Gizmos.color = Color.green;
				Gizmos.DrawLine(transform.position, selectedMono.transform.position);
			}

			if (_selectedHold is MonoBehaviour selectedHoldMono) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(transform.position, selectedHoldMono.transform.position);
			}
		}
	}
}
