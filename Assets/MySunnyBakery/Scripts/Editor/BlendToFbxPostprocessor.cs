using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace MySunnyBakery.Utils.Editor {
	public class BlendToFbxPostprocessor : AssetPostprocessor {
		private const string ASSET_LABEL = "AutoBlend2Fbx";
		private const string PYTHON_SCRIPT = @"
import sys

def export_fbx(blend_path: str, fbx_path: str) -> None:
    import bpy
    bpy.ops.wm.open_mainfile(filepath=blend_path)
    bpy.ops.export_scene.fbx(
        filepath=fbx_path,
        use_selection=False,
        global_scale=1.0,
        apply_unit_scale=True,
        apply_scale_options='FBX_SCALE_ALL',
        axis_forward='-Z',
        axis_up='Y',
        bake_space_transform=True,
        use_mesh_modifiers=True,
        add_leaf_bones=False,
        primary_bone_axis='Y',
        secondary_bone_axis='X',
        armature_nodetype='NULL',
        use_armature_deform_only=False,
        bake_anim=False,
        path_mode='RELATIVE'
    )

if __name__ == '__main__':
    argv = sys.argv[sys.argv.index('--') + 1:]
    if len(argv) < 2:
        sys.exit(1)
    export_fbx(blend_path=argv[0], fbx_path=argv[1])
";

		private static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths
		) {
			var targets = importedAssets
				.Where(path => path.EndsWith(".blend", StringComparison.OrdinalIgnoreCase) && HasLabel(path))
				.Select(BuildEntry)
				.ToList();

			if (targets.Count == 0) {
				return;
			}

			var blenderPath = GetBlenderPath();
			if (string.IsNullOrEmpty(blenderPath)) {
				Debug.LogError("[BlendToFbx] Blender not found. Set BLENDER_PATH or add blender to PATH.");
				return;
			}

			var scriptPath = Path.Combine(Path.GetTempPath(), $"blend_to_fbx_{Guid.NewGuid():N}.py");
			File.WriteAllText(scriptPath, PYTHON_SCRIPT);

			var success = 0;

			try {
				for (var i = 0; i < targets.Count; i++) {
					var entry = targets[i];
					var fileName = Path.GetFileName(entry.InputFullPath);

					EditorUtility.DisplayProgressBar(
						"Blend → FBX",
						$"Exporting {fileName} ({i + 1}/{targets.Count})",
						(float)i / targets.Count
					);

					try {
						if (RunBlenderExport(blenderPath, scriptPath, entry.InputFullPath, entry.OutputFullPath) == 0) {
							success++;
						}
					} catch (Exception ex) {
						Debug.LogError($"[BlendToFbx] {fileName}: {ex.Message}");
					}
				}
			} finally {
				EditorUtility.ClearProgressBar();
				try {
					File.Delete(scriptPath);
				}
				catch { }
			}

			AssetDatabase.Refresh();
			Debug.Log($"[BlendToFbx] {success}/{targets.Count} exported.");
		}

		private static bool HasLabel(string assetPath) {
			var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
			return asset != null && AssetDatabase.GetLabels(asset)
				.Any(l => string.Equals(l, ASSET_LABEL, StringComparison.OrdinalIgnoreCase));
		}

		private static BlendEntry BuildEntry(string assetPath) {
			var inputFull = Path.GetFullPath(assetPath);
			var sourceDirectory = Path.GetDirectoryName(inputFull) ?? string.Empty;
			var parentDirectory = Path.GetDirectoryName(sourceDirectory) ?? sourceDirectory;

			return new BlendEntry {
				InputFullPath = inputFull,
				OutputFullPath = Path.Combine(
					parentDirectory,
					Path.GetFileNameWithoutExtension(inputFull) + ".fbx"
				)
			};
		}

		private static int RunBlenderExport(string blenderExe, string scriptPath, string blendPath, string fbxPath) {
			var psi = new ProcessStartInfo {
				FileName = blenderExe,
				Arguments = $"--background --python \"{scriptPath}\" -- \"{blendPath}\" \"{fbxPath}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
			};

			using var process = Process.Start(psi);
			var stderr = process!.StandardError.ReadToEnd();
			process.WaitForExit();

			if (!string.IsNullOrWhiteSpace(stderr)) {
				Debug.LogWarning($"[BlendToFbx] {Path.GetFileName(blendPath)}: {stderr.Trim()}");
			}

			return process.ExitCode;
		}

		private static string GetBlenderPath() {
			var envPath = Environment.GetEnvironmentVariable("BLENDER_PATH");
			if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath)) return envPath;
			return FindOnPath("blender") ?? FindBlenderInstall();
		}

		private static string FindBlenderInstall() {
#if UNITY_EDITOR_WIN
			const string searchRoot = @"C:\Program Files\Blender Foundation";
			const string executable = "blender.exe";
#elif UNITY_EDITOR_OSX
			const string searchRoot = "/Applications";
			const string executable = "Contents/MacOS/Blender";
#else
			return null;
#endif

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
			if (!Directory.Exists(searchRoot)) return null;

			return Directory.EnumerateDirectories(searchRoot, "Blender*")
				.Select(dir => Path.Combine(dir, executable))
				.Where(File.Exists)
				.OrderByDescending(p => p)
				.FirstOrDefault();
#endif
		}

		private static string FindOnPath(string executable) {
			var ext = Environment.OSVersion.Platform == PlatformID.Win32NT ? ".exe" : string.Empty;

			foreach (var dir in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator)) {
				try {
					var full = Path.Combine(dir, executable + ext);
					if (File.Exists(full)) return full;
				} catch { }
			}

			return null;
		}

		private struct BlendEntry {
			public string InputFullPath;
			public string OutputFullPath;
		}
	}
}
