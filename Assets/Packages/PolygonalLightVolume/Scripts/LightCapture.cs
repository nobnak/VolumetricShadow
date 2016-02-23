using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {
	
	[RequireComponent(typeof(Camera))]
	public class LightCapture : MonoBehaviour {
		public const string PROP_SCALE = "_Scale";

		public LayerMask lightVolumeLayer = 1 << 12;
		public Shader debugShader;
		public float scale = 1f;

		Camera _mainCam;
		Camera _lightCam;
		Material _debugMat;

		void Start() {
			_mainCam = GetComponent<Camera> ();

			var lightCamObj = new GameObject ("Light Cam", typeof(Camera));
			_lightCam = lightCamObj.GetComponent<Camera> ();

			_debugMat = new Material (debugShader);
		}
		void OnDestroy() {
			Destroy (_lightCam.gameObject);
			Destroy (_debugMat);
		}
		void OnPreRender() {
			_lightCam.transform.SetParent (_mainCam.transform, false);
			_lightCam.transform.localPosition = Vector3.zero;
			_lightCam.transform.localRotation = Quaternion.identity;
			_lightCam.transform.localScale = Vector3.one;

			_lightCam.CopyFrom (_mainCam);
			_lightCam.depth++;
			_lightCam.cullingMask = lightVolumeLayer;
			_lightCam.clearFlags = CameraClearFlags.Nothing;
			_lightCam.backgroundColor = Color.black;
			_lightCam.depthTextureMode = DepthTextureMode.Depth;

			_debugMat.SetFloat (PROP_SCALE, scale);
		}
	}
}
