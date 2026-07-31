using MySunnyBakery.Characters;
using MySunnyBakery.Core;
using UnityEngine;

namespace MySunnyBakery.World {
	public class WorldEntryPoint: MonoBehaviour {
		[SerializeField] private LocalPlayer _player;
		[SerializeField] private Character _character;

		private void Start() {
			_player.TakeControl(_character.Input);
		}
	}
}
