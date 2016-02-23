using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {

	[ExecuteInEditMode]
	public class Obstacles : MonoBehaviour {
		public GameObject fab;
		public float scale = 1f;
		public float freq = 0.1f;
		public int count = 100;

		Transform[] _obstacles;
		float[] _seeds;

		void OnEnable() {
			_obstacles = new Transform[count];
			_seeds = new float[count];

			for (var i = 0; i < count; i++) {
				var inst = Instantiate(fab);

				var tr = _obstacles[i] = inst.transform;
				tr.SetParent(transform, false);
				tr.gameObject.hideFlags = HideFlags.DontSave;

				_seeds[i] = Random.value;
			}
		}
		void Update() {
			if (_obstacles == null)
				return;

			var t = Time.timeSinceLevelLoad * freq;

			for (var i = 0; i < count; i++) {
				var tr = _obstacles[i];
				tr.localPosition = new Vector3(
					scale * Noise(t, _seeds[i] * 100f),
					scale * Noise(t, _seeds[i] * 100f + 1000f),
					scale * Noise(t, _seeds[i] * 100f - 1000f));
			}
		}
		void OnDestroy() {
			if (_obstacles != null) {
				for (var i = 0; i < count; i++)
					DestroyImmediate(_obstacles[i].gameObject);
				_obstacles = null;
			}
		}

		float Noise(float x, float y) {
			return Mathf.PerlinNoise(x, y) * 2f - 1f;
		}
	}
}
