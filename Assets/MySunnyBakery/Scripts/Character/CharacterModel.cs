using System;
using UnityEngine;

namespace MySunnyBakery.Characters {
	[RequireComponent(typeof(Animator))]
	public class CharacterModel: MonoBehaviour {
		[field: SerializeField] public Transform RightHandSlot { get; private set; }
		
		public Animator Animator { get; private set; }
		
		public event Action<Vector3, Quaternion> AnimatorMoved;

		private void Awake() {
			Animator = GetComponent<Animator>();
		}
		
		private void OnAnimatorMove() {
			AnimatorMoved?.Invoke(Animator.deltaPosition, Animator.deltaRotation);
		}
	}
}
