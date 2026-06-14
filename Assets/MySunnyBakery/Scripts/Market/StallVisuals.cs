using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using MySunnyBakery.Utils;

namespace MySunnyBakery.Market {
	public class StallVisuals : MonoBehaviour {
		[SerializeField] private Stall _stall;
		[Space]
		[SerializeField] private LocalizeStringEvent _label;
		[SerializeField] private LocalizedString _priceText;
		[SerializeField] private LocalizedString _soldText;

		private const string PriceKey = "value";

		private void UpdatePrice() {
			_label.StringReference.Set(PriceKey, _stall.Price);
			_label.RefreshString();
		}
		private void UpdateCount() {
			if (_stall.Count > 0) {
				_label.StringReference = _priceText;
				UpdatePrice();
			} else {
				_label.StringReference = _soldText;
			}
			_label.RefreshString();
		}
		
		private void OnPriceChanged(int oldPrice, int newPrice) {
			UpdatePrice();
		}
		private void OnCountChanged(int oldCount, int newCount) {
			if (oldCount == newCount) {
				return;
			}
			UpdateCount();
		}

		private void OnEnable() {
			if (_stall != null) {
				_stall.PriceChanged += OnPriceChanged;
				_stall.CountChanged += OnCountChanged;
			}

			UpdatePrice();
			UpdateCount();
		}
		private void OnDisable() {
			if (_stall != null) {
				_stall.PriceChanged -= OnPriceChanged;
				_stall.CountChanged -= OnCountChanged;
			}
		}
	}
}
