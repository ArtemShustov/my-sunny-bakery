using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MySunnyBakery.Delivery {
	public class DeliveryService : MonoBehaviour {
		private readonly List<DeliveryPoint> _registeredPoints = new List<DeliveryPoint>();
		private readonly List<DeliveryTask> _activeTasks = new List<DeliveryTask>();

		private DeliveryMission _currentMission;

		public IReadOnlyList<DeliveryTask> ActiveTasks => _activeTasks;
		public bool IsMissionActive => _currentMission != null;

		public event Action<DeliveryTask> DeliveryCompleted;
		public event Action<IReadOnlyList<DeliveryTask>> MissionStarted;
		public event Action MissionCompleted;

		public void RegisterPoint(DeliveryPoint point) {
			if (!_registeredPoints.Contains(point)) {
				_registeredPoints.Add(point);
			}
		}

		public void UnregisterPoint(DeliveryPoint point) {
			_registeredPoints.Remove(point);
		}

		public void StartMission(DeliveryMission mission) {
			if (IsMissionActive) {
				ClearMission();
			}

			_currentMission = mission;

			var shuffled = new List<DeliveryPoint>(_registeredPoints);
			Shuffle(shuffled);

			var count = Mathf.Min(_currentMission.PointCount, shuffled.Count);
			for (var i = 0; i < count; i++) {
				var point = shuffled[i];
				var task = new DeliveryTask(point, _currentMission.RequiredItem);

				_activeTasks.Add(task);
				point.Activate(_currentMission.RequiredItem);
				point.Delivered += OnPointDelivered;
			}

			MissionStarted?.Invoke(_activeTasks);

			void Shuffle<T>(List<T> list) {
				for (var i = list.Count - 1; i > 0; i--) {
					var j = UnityEngine.Random.Range(0, i + 1);
					(list[i], list[j]) = (list[j], list[i]);
				}
			}
		}

		public void ClearMission() {
			foreach (var task in _activeTasks) {
				task.Point.Delivered -= OnPointDelivered;
				task.Point.Deactivate();
			}
			_activeTasks.Clear();
			_currentMission = null;
		}

		private void OnPointDelivered(DeliveryPoint point) {
			var task = _activeTasks.Find(t => t.Point == point);
			if (task == null) {
				return;
			}
			task.IsCompleted = true;
			DeliveryCompleted?.Invoke(task);

			if (_activeTasks.All(t => t.IsCompleted)) {
				MissionCompleted?.Invoke();
			}
		}

		private void OnDestroy() {
			foreach (var task in _activeTasks) {
				if (task.Point != null) {
					task.Point.Delivered -= OnPointDelivered;
				}
			}
		}
	}
}
