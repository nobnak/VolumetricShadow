using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {
    [ExecuteInEditMode]
    public class CameraSetting : MonoBehaviour {
        public enum DebugModeEnum { None = 0, DepthView }

        public DebugModeEnum debugMode;
        public Material depthViewMat;

        Camera _attachedCamera;

        void OnEnable() {
            _attachedCamera = GetComponent<Camera> ();
        }
    	void Update () {
            _attachedCamera.hdr = true;
    	}
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            switch (debugMode) {
            case DebugModeEnum.DepthView:
                Graphics.Blit (src, dst, depthViewMat);
                break;
            default:
                Graphics.Blit (src, dst);
                break;
            }
        }
    }
}