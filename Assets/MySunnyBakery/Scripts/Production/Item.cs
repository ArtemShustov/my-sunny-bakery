using UnityEngine;

namespace MySunnyBakery.Production {
	public class Item: MonoBehaviour {
		[SerializeField] private string _id;

		public string Id => _id;

		public void Init(string id) {
			_id = id;
		}
	}
}
