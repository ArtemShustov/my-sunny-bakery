using UnityEngine.Localization;
using System;

namespace MySunnyBakery.Interactions {
	public interface IInteraction {
		event Action<LocalizedString> HintChanged;
		
		void Interact(InteractionContext context);
		bool CanInteract(InteractionContext context);
		LocalizedString GetHint(InteractionContext context);
	}
}