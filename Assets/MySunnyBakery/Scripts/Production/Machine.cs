using UnityEngine;

namespace MySunnyBakery.Production {
	public abstract class Machine : MonoBehaviour {
		public abstract bool CanReceive(GameObject item);
		public abstract void Receive(GameObject item);
		public abstract bool CanTake();
		public abstract GameObject Take();
	}
}
