using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using MySunnyBakery.Utils;

namespace MySunnyBakery.Market {
	public class BuyInteractionVisuals : MonoBehaviour {
		[SerializeField] private BuyInteraction _buyInteraction;
		[Space]
		[SerializeField] private LocalizeStringEvent _label;
		[SerializeField] private LocalizedString _priceText;
		[SerializeField] private LocalizedString _soldText;

		private const string PriceKey = "value";

		private void UpdatePrice() {
			_label.StringReference.Set(PriceKey, _buyInteraction.Price);
			_label.RefreshString();
		}
		private void UpdateCount() {
			if (_buyInteraction.Count > 0) {
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
			if (_buyInteraction != null) {
				_buyInteraction.PriceChanged += OnPriceChanged;
				_buyInteraction.CountChanged += OnCountChanged;
			}

			UpdatePrice();
			UpdateCount();
		}
		private void OnDisable() {
			if (_buyInteraction != null) {
				_buyInteraction.PriceChanged -= OnPriceChanged;
				_buyInteraction.CountChanged -= OnCountChanged;
			}
		}
	}
}
