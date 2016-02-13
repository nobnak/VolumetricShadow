using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {
	[RequireComponent(typeof(Camera))]
	public class CameraIn : MonoBehaviour {
		public Texture inputTex;

		public void SetTexture(Texture tex) {
			inputTex = tex;
		}

		void OnRenderImage(RenderTexture src, RenderTexture dst) {
			Graphics.Blit (inputTex, dst);
		}
	}
}