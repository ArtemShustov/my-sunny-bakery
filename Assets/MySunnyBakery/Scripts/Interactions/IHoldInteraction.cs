using UnityEngine.Localization;

namespace MySunnyBakery.Interactions {
	public interface IHoldInteraction {
		void HoldInteract(InteractionContext context);
		bool CanHoldInteract(InteractionContext context);
		LocalizedString GetHoldHint(InteractionContext context);
	}
}
