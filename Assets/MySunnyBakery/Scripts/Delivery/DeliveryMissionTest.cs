using System.Collections.Generic;
using MySunnyBakery.Items;
using UnityEngine;

namespace MySunnyBakery.Delivery {
	public class DeliveryMissionTest : MonoBehaviour {
		[SerializeField] private DeliveryService _deliveryService;
		[SerializeField] private ItemDefinition _availableItem;
		[SerializeField] private int _pointCount = 3;

		private void Start() {
			_deliveryService.StartMission(_pointCount, _availableItem);
		}

		private void OnMissionStarted(IReadOnlyList<DeliveryTask> tasks) {
			Debug.Log($"[{nameof(DeliveryMissionTest)}] Mission started with {tasks.Count} tasks:");
			for (var i = 0; i < tasks.Count; i++) {
				var task = tasks[i];
				Debug.Log($"  Task {i}: Deliver {task.RequiredItem.Name} to {task.Point.name}");
			}
		}

		private void OnMissionCompleted() {
			Debug.Log($"[{nameof(DeliveryMissionTest)}] All deliveries completed! Mission finished.");
		}
		
		private void OnDeliverCompleted(DeliveryTask delivery) {
			Debug.Log($"[{nameof(DeliveryMissionTest)}] Delivery to {delivery.Point.name} completed.");
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
