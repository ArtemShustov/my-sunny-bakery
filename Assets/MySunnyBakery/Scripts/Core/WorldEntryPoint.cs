using MySunnyBakery.Characters;
using UnityEngine;

namespace MySunnyBakery.Core {
	public class WorldEntryPoint: MonoBehaviour {
		[SerializeField] private LocalPlayer _player;
		[SerializeField] private Character _character;

		private void Start() {
			_player.TakeControl(_character.Input);
		}
	}
}
