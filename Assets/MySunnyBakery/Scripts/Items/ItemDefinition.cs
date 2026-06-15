using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace MySunnyBakery.Items {
	[CreateAssetMenu(menuName = "My Sunny Bakery/Item Definition", fileName = "New Item Definition")]
	public class ItemDefinition : ScriptableObject {
		[field: FormerlySerializedAs("_id")] 
		[field: SerializeField] public string Id { get; private set; }
		[field: FormerlySerializedAs("_name")] 
		[field: SerializeField] public LocalizedString Name { get; private set; }

		[field: Header("Visual")]
		[field: FormerlySerializedAs("_prefab")]
		[field: SerializeField] public Item Prefab { get; private set; }

		[field: Header("Storage")] 
		[field: FormerlySerializedAs("_maxStack")] 
		[field: SerializeField] public int MaxStack { get; private set; } = 1;
		[field: FormerlySerializedAs("_size")]
		[field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;

		public Item Instantiate(Transform parent) {
			if (Prefab == null) {
				return null;
			}

			var instance = Object.Instantiate(Prefab, parent);
			instance.Init(this);
			return instance;
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			if (Application.isPlaying || Prefab == null) {
				return;
			}

			var so = new UnityEditor.SerializedObject(Prefab);
			so.FindProperty("_definition").objectReferenceValue = this;
			so.ApplyModifiedPropertiesWithoutUndo();
		}
		#endif
	}
}
