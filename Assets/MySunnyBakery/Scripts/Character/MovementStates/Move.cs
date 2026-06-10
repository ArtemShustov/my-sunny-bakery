using System;
using UnityEngine;

namespace MySunnyBakery.Characters.MovementStates {
	[Serializable]
	public class Move: CharacterMovement.State {
		[SerializeField] private float _runThreshold = 0.3f;
		[SerializeField] private float _idleTransitionDelay = 0.1f;
		private bool _isRunning;
		private float _noInputTime;

		public override bool CheckTransition(float deltaTime) {
			if (Character.Input.Move.sqrMagnitude < Mathf.Epsilon) {
				_noInputTime += deltaTime;
				if (_noInputTime >= _idleTransitionDelay) {
					Character.Movement.ChangeState<Idle>();
					return true;
				}
			} else {
				_noInputTime = 0f;
			}
			return false;
		}

		public override void OnUpdate(float deltaTime) {
			var input = Character.Input.Move;
			var direction = new Vector3(input.x, 0, input.y).normalized;
			
			Character.Movement.Data.Movement = direction;

			var newIsRunning = input.sqrMagnitude > _runThreshold * _runThreshold;
			if (newIsRunning != _isRunning) {
				_isRunning = newIsRunning;
				Character.Model.Animator.CrossFadeInFixedTime(_isRunning ? "Run" : "Walk", 0.2f);
			}
		}

		public override void OnEnter(CharacterMovement.State previousState) {
			_noInputTime = 0f;
			_isRunning = Character.Input.Move.sqrMagnitude > _runThreshold * _runThreshold;
			Character.Model.Animator.CrossFadeInFixedTime(_isRunning ? "Run" : "Walk", 0.2f);
		}
	}
}
