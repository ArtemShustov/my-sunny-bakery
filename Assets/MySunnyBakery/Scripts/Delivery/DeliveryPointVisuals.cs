using UnityEngine;

namespace MySunnyBakery.Delivery {
	public class DeliveryPointVisuals : MonoBehaviour {
		[SerializeField] private DeliveryPoint _deliveryPoint;

		private void OnEnable() {
			if (_deliveryPoint != null) {
				_deliveryPoint.Delivered += OnDelivered;
			}
		}

		private void OnDisable() {
			if (_deliveryPoint != null) {
				_deliveryPoint.Delivered -= OnDelivered;
			}
		}

		private void OnDelivered(DeliveryPoint point) {
		}
	}
}
