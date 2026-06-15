using UnityEngine;
using UnityEngine.Localization;

namespace MySunnyBakery.Items {
	[CreateAssetMenu(menuName = "My Sunny Bakery/Item Definition", fileName = "New Item Definition")]
	public class ItemDefinition : ScriptableObject {
		[SerializeField] private string _id;
		[SerializeField] private LocalizedString _name;
		[SerializeField] private int _maxStack = 1;
		[SerializeField] private Vector2Int _size = Vector2Int.one;
		[SerializeField] private Item _prefab;

		public string Id => _id;
		public LocalizedString Name => _name;
		public int MaxStack => _maxStack;
		public Vector2Int Size => _size;
		public Item Prefab => _prefab;

		public Item Instantiate(Transform parent) {
			if (_prefab == null) {
				return null;
			}

			var instance = Object.Instantiate(_prefab, parent);
			instance.Init(this);
			return instance;
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			if (Application.isPlaying || _prefab == null) {
				return;
			}

			var so = new UnityEditor.SerializedObject(_prefab);
			so.FindProperty("_definition").objectReferenceValue = this;
			so.ApplyModifiedPropertiesWithoutUndo();
		}
		#endif
	}
}
