using MySunnyBakery.Items;

namespace MySunnyBakery.Delivery {
	public class DeliveryTask {
		public DeliveryPoint Point { get; }
		public ItemDefinition RequiredItem { get; }
		public bool IsCompleted { get; set; }

		public DeliveryTask(DeliveryPoint point, ItemDefinition requiredItem) {
			Point = point;
			RequiredItem = requiredItem;
			IsCompleted = false;
		}
	}
}
