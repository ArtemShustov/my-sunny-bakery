using MySunnyBakery.Delivery;
using VContainer;
using VContainer.Unity;

namespace MySunnyBakery.World {
	public class WorldSceneLifetimeScope : LifetimeScope {
		protected override void Configure(IContainerBuilder builder) {
			builder.RegisterComponentInHierarchy<DeliveryService>();
			builder.RegisterComponentInHierarchy<DayCycleService>();
		}
	}
}
