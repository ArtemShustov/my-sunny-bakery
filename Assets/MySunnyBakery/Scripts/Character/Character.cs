using UnityEngine;

namespace MySunnyBakery.Characters {
	public class Character: MonoBehaviour {
		[field: SerializeField] public CharacterModel Model { get; private set; }
		[field: SerializeField] public CharacterController Controller { get; private set; }
		[field: SerializeField] public CharacterMovement Movement { get; private set; }
		[field: SerializeField] public CharacterInput Input { get; private set; }
	}
}
