using UnityEngine;

namespace MySunnyBakery.Production {
	public class MillVisuals: MonoBehaviour {
		[Header("References")]
		[SerializeField] private Mill _mill;
		[SerializeField] private ParticleSystem _grindParticles;
		[SerializeField] private Transform _wheel;

		[Header("Wheel Settings")]
		[SerializeField] private float _wheelSpeed = 180f;

		private bool _wheelActive;

		private void Update() {
			if (_wheelActive) {
				_wheel.Rotate(0f, _wheelSpeed * Time.deltaTime, 0f, Space.Self);
			}
		}

		private void OnWorkStarted() {
			_grindParticles.Play();
			_wheelActive = true;
		}

		private void OnWorkStopped() {
			_grindParticles.Stop();
			_wheelActive = false;
		}

		private void OnItemTaken() {
			_grindParticles.Stop();
			_wheelActive = false;
		}
		
		private void OnEnable() {
			_mill.WorkStarted += OnWorkStarted;
			_mill.WorkStopped += OnWorkStopped;
			_mill.ItemTaken += OnItemTaken;
		}

		private void OnDisable() {
			_mill.WorkStarted -= OnWorkStarted;
			_mill.WorkStopped -= OnWorkStopped;
			_mill.ItemTaken -= OnItemTaken;
		}
	}
}
