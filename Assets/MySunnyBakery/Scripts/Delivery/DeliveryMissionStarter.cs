using MySunnyBakery.Items;
using MySunnyBakery.World;
using UnityEngine;
using VContainer;

namespace MySunnyBakery.Delivery {
	public class DeliveryMissionStarter : MonoBehaviour {
		[SerializeField] private ItemDefinition[] _availableItems;
		[SerializeField] private int _pointCount = 3;

		private DayCycleService _dayCycle;
		private DeliveryService _delivery;

		[Inject]
		private void Construct(DayCycleService dayCycleService, DeliveryService deliveryService) {
			_dayCycle = dayCycleService;
			_delivery = deliveryService;
		}

		private void OnEnable() {
			_dayCycle.DayStarted += OnDayStarted;
		}

		private void OnDisable() {
			_dayCycle.DayStarted -= OnDayStarted;
		}

		private void OnDayStarted() {
			if (_availableItems == null || _availableItems.Length == 0) {
				return;
			}
			var item = _availableItems[UnityEngine.Random.Range(0, _availableItems.Length)];
			var mission = new DeliveryMission(
				Mathf.Min(_pointCount, _delivery.PointsCount), 
				item
			);
			_delivery.StartMission(mission);
		}
	}
}
