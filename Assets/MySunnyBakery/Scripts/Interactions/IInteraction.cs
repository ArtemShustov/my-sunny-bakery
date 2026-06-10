using UnityEngine.Localization;

namespace MySunnyBakery.Interactions {
	public interface IInteraction {
		void Interact(InteractionContext context);
		bool CanInteract(InteractionContext context);
		LocalizedString GetHint(InteractionContext context);
	}
}
