using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {
	
	[RequireComponent(typeof(Camera))]
	public class LightCapture : MonoBehaviour {
		public const string PROP_SCALE = "_Scale";

		public LayerMask lightVolumeLayer = 1 << 12;
		public Camera referenceCam;
		public Shader debugShader;
		public float scale = 1f;

		Camera _lightCam;
		Material _debugMat;

		void Start() {
			_lightCam = GetComponent<Camera> ();

			_debugMat = new Material (debugShader);
		}
		void OnDestroy() {
			Destroy (_debugMat);
		}
		void Update() {
			_lightCam.transform.SetParent (referenceCam.transform, false);
			_lightCam.transform.localPosition = Vector3.zero;
			_lightCam.transform.localRotation = Quaternion.identity;
			_lightCam.transform.localScale = Vector3.one;

			_lightCam.CopyFrom (referenceCam);
			_lightCam.depth++;
			_lightCam.cullingMask = lightVolumeLayer;
			_lightCam.clearFlags = CameraClearFlags.SolidColor;
			_lightCam.backgroundColor = Color.black;
			_lightCam.depthTextureMode |= DepthTextureMode.Depth;

			_debugMat.SetFloat (PROP_SCALE, scale);
		}
		void OnRenderImage(RenderTexture src, RenderTexture dst) {
			Graphics.Blit (src, dst, _debugMat);
		}
	}
}
