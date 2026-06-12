using UnityEngine;

namespace MySunnyBakery.Production {
	public interface IMachine {
		bool CanReceive(GameObject item);
		void Receive(GameObject item);
		bool CanTake();
		GameObject Take();
	}
}