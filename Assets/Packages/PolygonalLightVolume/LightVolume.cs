using UnityEngine;
using System.Collections;

namespace PolygonalLightVolume {

	[RequireComponent(typeof(Camera))]
	public class LightVolume : MonoBehaviour {
		public const string PROP_SHADOW_CAM_2_WORLD_MAT = "_ShadowCamToWorldMat";
		public const string PROP_NDC_EYE_MAT = "_NDCEyeMat";
		public const string PROP_DEPTH_TEX = "_DepthTex";

		public int lod = 10;
		public Shader eyeDepthBaker;
		public Material lightVolumeMat;

		int _size = -1;
		Camera _cam;
		RenderTexture _depthTex;
		RenderTexture _eyeDepthTex;
		Matrix4x4 _ndcEyeMat;
		Material _eyeDepthBakerMat;

		void Start () {
			_cam = GetComponent<Camera>();
			_cam.depthTextureMode = DepthTextureMode.Depth;

			_eyeDepthBakerMat = new Material(eyeDepthBaker);

			var mfs = GetComponentsInChildren<MeshFilter> ();
			for (var i = 0; i < mfs.Length; i++)
				mfs [i].sharedMesh.bounds = new Bounds (Vector3.zero, _cam.farClipPlane * Vector3.one);
		}
		void Update () {
			var size = 1 << lod;
			if (size != _size) {
				_size = size;
				ReleaseTexture();

				_depthTex = new RenderTexture(_size, _size, 24, RenderTextureFormat.Depth);
				_eyeDepthTex = new RenderTexture(_size, _size, 0, RenderTextureFormat.RFloat);
				_depthTex.wrapMode = _eyeDepthTex.wrapMode = TextureWrapMode.Clamp;
				_depthTex.filterMode = _eyeDepthTex.filterMode = FilterMode.Bilinear;
				_cam.targetTexture = _depthTex;
				Debug.LogFormat("Create Depth Map : size={0}", size);
			}
			_depthTex.DiscardContents();

			_ndcEyeMat = UpdateMatrix();
			lightVolumeMat.mainTexture = _eyeDepthTex;
			Shader.SetGlobalMatrix(PROP_NDC_EYE_MAT, _ndcEyeMat);
			Shader.SetGlobalMatrix(PROP_SHADOW_CAM_2_WORLD_MAT, transform.localToWorldMatrix);
		}
		void OnPostRender() {
			Graphics.Blit(_depthTex, _eyeDepthTex, _eyeDepthBakerMat);
		}
		void OnDestroy() {
			ReleaseTexture();
			Destroy(_eyeDepthBakerMat);
		}

		void ReleaseTexture() {
			Destroy(_depthTex);
			Destroy(_eyeDepthTex);
		}

		Matrix4x4 UpdateMatrix() {
			var viewAngle = 0.5f * _cam.fieldOfView * Mathf.Deg2Rad;
			var n = _cam.nearClipPlane;
			var f = _cam.farClipPlane;
			var t = n * Mathf.Tan (viewAngle);
			var r = t * _cam.aspect;
			var ndcEyeMat = Matrix4x4.zero;
			ndcEyeMat[0, 0] = r / n;
			ndcEyeMat[1, 1] = t / n;
			ndcEyeMat[2, 2] = (n - f) / (n * f);
			ndcEyeMat[2, 3] = 1f / n;
			ndcEyeMat[3, 2] = 1f / f;
			return ndcEyeMat;
		}
	}
}
