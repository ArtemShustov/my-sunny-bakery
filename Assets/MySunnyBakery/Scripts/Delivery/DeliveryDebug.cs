using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace MySunnyBakery.Delivery {
	public class DeliveryDebug : MonoBehaviour {
		[Inject] private DeliveryService _deliveryService;

		private void OnMissionStarted(IReadOnlyList<DeliveryTask> tasks) {
			Debug.Log($"[{nameof(DeliveryDebug)}] Mission started with {tasks.Count} tasks:");
			for (var i = 0; i < tasks.Count; i++) {
				var task = tasks[i];
				Debug.Log($"  Task {i}: Deliver {task.RequiredItem.Name} to {task.Point.name}");
			}
		}

		private void OnMissionCompleted() {
			Debug.Log($"[{nameof(DeliveryDebug)}] All deliveries completed! Mission finished.");
		}

		private void OnDeliverCompleted(DeliveryTask delivery) {
			Debug.Log($"[{nameof(DeliveryDebug)}] Delivery to {delivery.Point.name} completed.");
		}

		private void OnEnable() {
			_deliveryService.MissionStarted += OnMissionStarted;
			_deliveryService.MissionCompleted += OnMissionCompleted;
			_deliveryService.DeliveryCompleted += OnDeliverCompleted;
		}

		private void OnDisable() {
			_deliveryService.MissionStarted -= OnMissionStarted;
			_deliveryService.MissionCompleted -= OnMissionCompleted;
			_deliveryService.DeliveryCompleted -= OnDeliverCompleted;
		}
	}
}
