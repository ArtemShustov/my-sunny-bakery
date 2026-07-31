using UnityEngine;

namespace MySunnyBakery.Core {
	public class LocalPlayer: MonoBehaviour {
		[SerializeField] private GameCamera _camera;
		
		public InputActions Actions { get; private set; }

		private void Awake() {
			Actions = new InputActions();
			Actions.Enable();
			
			_camera.Initialize(this);
		}
		private void OnDestroy() {
			Actions.Disable();
			Actions.Dispose();
		}

		public void TakeControl(IControllable controllable) {
			controllable.TakeControl(this);
			controllable.ConfigureCamera(_camera);
		}
		public void ReleaseControl(IControllable controllable) {
			controllable.ReleaseControl();
		}
	}
}
