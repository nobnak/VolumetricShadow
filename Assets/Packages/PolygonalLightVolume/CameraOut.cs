using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {
	[RequireComponent(typeof(Camera))]
	public class CameraOut : MonoBehaviour {
		public RenderTextureFormat format;
		public TextureEvent OnCapture;

		Camera _attachedCamera;
		RenderTexture _outputTex;

		void OnEnable() {
			_attachedCamera = GetComponent<Camera> ();
		}
		void Update() {
			if (_outputTex == null || _outputTex.width != _attachedCamera.pixelWidth || _outputTex.height != _attachedCamera.pixelHeight) {
				ReleaseTexture ();
				_outputTex = new RenderTexture (_attachedCamera.pixelWidth, _attachedCamera.pixelHeight, 24, format);
			}

			_attachedCamera.targetTexture = _outputTex;
			OnCapture.Invoke (_outputTex);
		}
		void OnDisable() {
			_attachedCamera.targetTexture = null;
			ReleaseTexture ();
		}

		void ReleaseTexture () {
			Destroy (_outputTex);
		}
	}

	[System.Serializable]
	public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> {}
}
