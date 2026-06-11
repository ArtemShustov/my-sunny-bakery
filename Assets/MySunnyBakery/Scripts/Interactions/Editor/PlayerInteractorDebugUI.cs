using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace MySunnyBakery.Interactions.Editor {
	[InitializeOnLoad]
	public static class PlayerInteractorDebugOverlay {
		private static GameObject _debugObject;
		private static DebugGUI _debugGUI;
		private static PlayerInteractor _target;

		static PlayerInteractorDebugOverlay() {
			Selection.selectionChanged += OnSelectionChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state) {
			if (state == PlayModeStateChange.ExitingPlayMode) {
				DestroyOverlay();
			}
		}

		private static void OnSelectionChanged() {
			if (!Application.isPlaying) {
				DestroyOverlay();
				return;
			}

			var go = Selection.activeGameObject;
			if (go != null && go.TryGetComponent<PlayerInteractor>(out var interactor)) {
				if (_target != interactor) {
					DestroyOverlay();
					_target = interactor;
					CreateOverlay();
				}
			} else {
				DestroyOverlay();
				_target = null;
			}
		}

		private static void CreateOverlay() {
			if (_debugObject != null) return;

			_debugObject = new GameObject("__PlayerInteractorDebug__");
			_debugObject.hideFlags = HideFlags.HideAndDontSave;
			_debugGUI = _debugObject.AddComponent<DebugGUI>();
			_debugGUI.Initialize(_target);
			Object.DontDestroyOnLoad(_debugObject);
		}

		private static void DestroyOverlay() {
			if (_debugObject != null) {
				Object.DestroyImmediate(_debugObject);
				_debugObject = null;
				_debugGUI = null;
				_target = null;
			}
		}

		private class DebugGUI : MonoBehaviour {
			private PlayerInteractor _interactor;
			private FieldInfo _selectedField;
			private FieldInfo _selectedHoldField;
			private FieldInfo _defaultHoldStackField;

			public void Initialize(PlayerInteractor target) {
				_interactor = target;
				var type = typeof(PlayerInteractor);
				_selectedField = type.GetField("_selected", BindingFlags.NonPublic | BindingFlags.Instance);
				_selectedHoldField = type.GetField("_selectedHold", BindingFlags.NonPublic | BindingFlags.Instance);
				_defaultHoldStackField = type.GetField("_defaultHoldStack", BindingFlags.NonPublic | BindingFlags.Instance);
			}

			private void OnGUI() {
				if (_interactor == null) {
					Destroy(gameObject);
					return;
				}

				var areaRect = new Rect(10, 10, 350, 250);
				GUI.Box(areaRect, "PlayerInteractor Debug");

				GUILayout.BeginArea(new Rect(15, 25, 340, 230));
				GUILayout.Label("Selection", EditorStyles.boldLabel);
				DrawInteraction("Selected", _selectedField.GetValue(_interactor) as IInteraction);
				DrawInteraction("Selected Hold", _selectedHoldField.GetValue(_interactor) as IHoldInteraction);

				var defaultStack = _defaultHoldStackField.GetValue(_interactor) as List<IHoldInteraction>;
				GUILayout.Label("Default Hold Stack:", EditorStyles.boldLabel);
				if (defaultStack != null && defaultStack.Count > 0) {
					for (int i = 0; i < defaultStack.Count; i++) {
						var hold = defaultStack[i];
						if (hold is MonoBehaviour monoBehaviour) {
							GUILayout.Label($"  {i}: {monoBehaviour.name}");
						} else if (hold != null) {
							GUILayout.Label($"  {i}: {hold.GetType().Name}");
						} else {
							GUILayout.Label($"  {i}: null");
						}
					}
				} else {
					GUILayout.Label("  None");
				}
				GUILayout.EndArea();
			}

			private void DrawInteraction(string label, object interaction) {
				if (interaction == null) {
					GUILayout.Label($"{label}: None");
					return;
				}

				if (interaction is MonoBehaviour monoBehaviour) {
					GUILayout.BeginHorizontal();
					GUILayout.Label($"{label}: {monoBehaviour.name} ({interaction.GetType().Name})");
					if (GUILayout.Button("Select", GUILayout.Width(80))) {
						Selection.activeObject = monoBehaviour.gameObject;
						EditorGUIUtility.PingObject(monoBehaviour.gameObject);
					}
					GUILayout.EndHorizontal();
				} else {
					GUILayout.Label($"{label}: {interaction.GetType().Name}");
				}
			}
		}
	}
}
