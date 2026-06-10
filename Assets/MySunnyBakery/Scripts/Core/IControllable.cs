using MySunnyBakery.Core;

namespace MySunnyBakery.Core {
	public interface IControllable {
		LocalPlayer Player { get; }

		void TakeControl(LocalPlayer player);
		void ReleaseControl();

		void ConfigureCamera(GameCamera camera);
	}
}
