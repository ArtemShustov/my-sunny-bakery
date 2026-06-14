using VContainer;
using VContainer.Unity;

namespace MySunnyBakery.Core {
	public class RootLifetimeScope : LifetimeScope {
		protected override void Configure(IContainerBuilder builder) {
			builder.Register<PlayerData>(Lifetime.Singleton);
		}
	}
}
