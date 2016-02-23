using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {
    [ExecuteInEditMode]
    public class CameraSetting : MonoBehaviour {
        Camera _attachedCamera;

        void OnEnable() {
            _attachedCamera = GetComponent<Camera> ();
        }
    	void Update () {
            _attachedCamera.hdr = true;
			_attachedCamera.depthTextureMode = DepthTextureMode.Depth;
    	}
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
	        Graphics.Blit (src, dst);
        }
    }
}