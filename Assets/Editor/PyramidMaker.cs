using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class PyramidMaker : EditorWindow {
	int lod = 9;
	string directoryname = @"Assets\Models\Pyramids";

	[MenuItem("Window/Pyramid Maker")]
	static void Init() {
		EditorWindow.GetWindow (typeof(PyramidMaker));
	}

	void OnGUI() {
		var size = 1 << lod;

		GUILayout.BeginVertical ();

		GUILayout.Label (string.Format ("Grid Size {0}x{0}", size));
		lod = EditorGUILayout.IntField ("LOD", lod);

		directoryname = EditorGUILayout.TextField ("Save Directory", directoryname);

		if (GUILayout.Button ("Build"))
			Build (size, directoryname);

		GUILayout.EndVertical ();
	}
	void Build(int size, string directoryname) {
		var filename = string.Format ("PolygonalLightVolume{0:d4}.obj", size);
		if (!Directory.Exists (directoryname)) {
			Debug.LogFormat ("Directory {0} not found", directoryname);
			return;
		}
		var fullpath = Path.Combine (directoryname, filename);
		using (var writer = new StreamWriter(fullpath, false)) {
			Write (size, writer);
		}
	}

	void Write(int size, StreamWriter writer) {
		writer.WriteLine ("g PolygonalLightVolume");

		var edgeCount = size - 1;
		var dx = -2f / edgeCount;
		var dy = 2f / edgeCount;
		var xoffset = 1f;
		var yoffset = -1f;
		for (var vy = 0; vy < size; vy++) {
			for (var vx = 0; vx < size; vx++) {
				AppendVertex (writer, vx * dx + xoffset, vy * dy + yoffset, 1f);
			}
		}
		AppendVertex (writer, 0f, 0f, 0f);

		var duv = 1f / edgeCount;
		for (var vy = 0; vy < size; vy++) {
			for (var vx = 0; vx < size; vx++) {
				AppendUV (writer, duv * vx, duv * vy);
			}
		}
		AppendUV (writer, 0.5f, 0.5f);

		for (var vy = 0; vy < edgeCount; vy++) {
			for (var vx = 0; vx < edgeCount; vx++) {
				var vi = vx + vy * size;
				AppendTriangle (writer, vi, vi + size + 1, vi + 1);
				AppendTriangle (writer, vi, vi + size, vi + size + 1);
			}
		}

		var vnear = size * size;
		for (var i = 0; i < edgeCount; i++) {
			var vi = i;
			AppendTriangle (writer, vnear, vi, vi + 1);
		}
		for (var i = 0; i < edgeCount; i++) {
			var vi = size * (i + 1) - 1;
			AppendTriangle (writer, vnear, vi, vi + size);
		}
		for (var i = 0; i < edgeCount; i++) {
			var vi = size * (size - 1) + i;
			AppendTriangle (writer, vnear, vi + 1, vi);
		}
		for (var i = 0; i < edgeCount; i++) {
			var vi = size * i;
			AppendTriangle (writer, vnear, vi + size, vi);
		}
	}
	void AppendVertex(StreamWriter writer, float x, float y, float z) {
		writer.WriteLine (string.Format ("v {0:e7} {1:e7} {2:e7}", x, y, z));
	}
	void AppendUV(StreamWriter writer, float u, float v) {
		writer.WriteLine (string.Format ("vt {0:e7} {1:e7}", u, v));
	}
	void AppendTriangle(StreamWriter writer, int vx, int vy, int vz, int tx, int ty, int tz) {
		writer.WriteLine (string.Format ("f {0:d}/{1:d} {2:d}/{3:d} {4:d}/{5:d}", 
			vx + 1, tx + 1, vy + 1, ty + 1, vz + 1, tz + 1));
	}
	void AppendTriangle(StreamWriter writer, int vx, int vy, int vz) {
		AppendTriangle (writer, vx, vy, vz, vx, vy, vz);
	}
}
