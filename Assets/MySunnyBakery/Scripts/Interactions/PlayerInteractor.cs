using System;
using System.Collections.Generic;
using System.Linq;
using MySunnyBakery.Characters;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

namespace MySunnyBakery.Interactions {
	public class PlayerInteractor : MonoBehaviour {
		[SerializeField] private Character _character;
		[SerializeField] private float _interactionRadius = 3f;
		[SerializeField] private LayerMask _interactionLayer = ~0;

		public event Action<LocalizedString> HintChanged;
		public event Action<LocalizedString> HoldHintChanged;

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

		private void OnEnable() {
			_character.Input.Use += OnUsePerformed;
			_character.Input.UseAlt += OnUseAltPerformed;
		}
		private void OnDisable() {
			_character.Input.Use -= OnUsePerformed;
			_character.Input.UseAlt -= OnUseAltPerformed;

			if (_selected != null) {
				_selected.HintChanged -= OnSelectedHintChanged;
			}
			if (_selectedHold != null) {
				_selectedHold.HoldHintChanged -= OnSelectedHoldHintChanged;
			}

			_selected = null;
			_selectedHold = null;
		}

		#region Selection

		private void UpdateSelection() {
			var context = GetContext();
			var position = transform.position;

			IInteraction newSelected = null;
			IHoldInteraction newSelectedHold = null;

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
						newSelected = interaction;
					}
				}

				if (collider.TryGetComponent<IHoldInteraction>(out var holdInteraction) && holdInteraction.CanHoldInteract(context)) {
					var closestPoint = collider.ClosestPoint(position);
					var sqrDistance = (closestPoint - position).sqrMagnitude;

					if (sqrDistance < closestHold) {
						closestHold = sqrDistance;
						newSelectedHold = holdInteraction;
					}
				}
			}

			if (newSelectedHold == null) {
				newSelectedHold = _defaultHoldStack.LastOrDefault(x => x != null && x.CanHoldInteract(context));
			}

			ChangeSelectedInteraction(newSelected, newSelectedHold);
		}

		private void ChangeSelectedInteraction(IInteraction newSelected, IHoldInteraction newSelectedHold) {
			if (_selected == newSelected && _selectedHold == newSelectedHold) {
				return;
			}

			if (_selected != null) {
				_selected.HintChanged -= OnSelectedHintChanged;
			}
			if (_selectedHold != null) {
				_selectedHold.HoldHintChanged -= OnSelectedHoldHintChanged;
			}

			_selected = newSelected;
			_selectedHold = newSelectedHold;

			if (_selected != null) {
				_selected.HintChanged += OnSelectedHintChanged;
				HintChanged?.Invoke(_selected.GetHint(GetContext()));
			} else {
				HintChanged?.Invoke(new LocalizedString());
			}

			if (_selectedHold != null) {
				_selectedHold.HoldHintChanged += OnSelectedHoldHintChanged;
				HoldHintChanged?.Invoke(_selectedHold.GetHoldHint(GetContext()));
			} else {
				HoldHintChanged?.Invoke(new LocalizedString());
			}
		}

		private void OnSelectedHintChanged(LocalizedString hint) {
			HintChanged?.Invoke(hint);
		}
		private void OnSelectedHoldHintChanged(LocalizedString hint) {
			HoldHintChanged?.Invoke(hint);
		}

		#endregion

		#region Execution

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
			if (_selectedHold == null) {
				return;
			}

			var context = GetContext();
			_selectedHold.HoldInteract(context);
		}

		#endregion

		#region Default Hold Stack

		public void AddDefaultHoldInteraction(IHoldInteraction holdInteraction) {
			if (holdInteraction == null || _defaultHoldStack.Contains(holdInteraction)) {
				return;
			}

			_defaultHoldStack.Add(holdInteraction);
		}
		public void RemoveDefaultHoldInteraction(IHoldInteraction holdInteraction) {
			_defaultHoldStack.Remove(holdInteraction);
		}

		#endregion

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

		private InteractionContext GetContext() {
			return new InteractionContext(_character.gameObject);
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
