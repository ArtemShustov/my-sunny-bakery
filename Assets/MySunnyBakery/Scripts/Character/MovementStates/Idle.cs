using System;
using UnityEngine;

namespace MySunnyBakery.Characters.MovementStates {
	[Serializable]
	public class Idle: CharacterMovement.State {
		public override bool CheckTransition(float deltaTime) {
			if (Character.Input.Move.sqrMagnitude >= Mathf.Epsilon) {
				Character.Movement.ChangeState<Move>();
				return true;
			}
			return false;
		}

		public override void OnEnter(CharacterMovement.State previousState) {
			Character.Movement.Data.Movement = Vector3.zero;
			Character.Model.Animator.CrossFadeInFixedTime("Idle", 0.1f);
		}
	}
}
