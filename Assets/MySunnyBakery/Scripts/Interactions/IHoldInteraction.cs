using UnityEngine.Localization;
using System;

namespace MySunnyBakery.Interactions {
	public interface IHoldInteraction {
		event Action<LocalizedString> HoldHintChanged;
		
		void HoldInteract(InteractionContext context);
		bool CanHoldInteract(InteractionContext context);
		LocalizedString GetHoldHint(InteractionContext context);
	}
}