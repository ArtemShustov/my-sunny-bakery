using MySunnyBakery.Items;

namespace MySunnyBakery.Delivery {
	public class DeliveryMission {
		public int PointCount { get; }
		public ItemDefinition RequiredItem { get; }

		public DeliveryMission(int pointCount, ItemDefinition requiredItem) {
			PointCount = pointCount;
			RequiredItem = requiredItem;
		}
	}
}
