using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ShadowVolume : MonoBehaviour {
	public const string PROP_SHADOW_CAM_2_WORLD_MAT = "_ShadowCamToWorldMat";
	public const string PROP_NDC_EYE_MAT = "_NDCEyeMat";
	public const string PROP_DEPTH_TEX = "_DepthTex";

	public int lod = 10;
	public Shader eyeDepthBaker;
	public Transform target;

	int _size = -1;
	Camera _cam;
	RenderTexture _depthTex;
	RenderTexture _eyeDepthTex;
	Matrix4x4 _ndcEyeMat;
	Mesh _shadowMesh;
	Material _eyeDepthBakerMat;

	Material _targetMat;

	void Start () {
		_cam = GetComponent<Camera>();
		_cam.depthTextureMode = DepthTextureMode.Depth;

		var targetMf = target.GetComponent<MeshFilter>();
		var targetRd = target.GetComponent<Renderer>();
		_shadowMesh = new Mesh();
		targetMf.sharedMesh = _shadowMesh;
		_targetMat = targetRd.sharedMaterial;

		_eyeDepthBakerMat = new Material(eyeDepthBaker);
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

			UpdateMesh(_shadowMesh, _size);
		}
		_depthTex.DiscardContents();

		_ndcEyeMat = UpdateMatrix();
		_targetMat.mainTexture = _eyeDepthTex;
		Shader.SetGlobalMatrix(PROP_NDC_EYE_MAT, _ndcEyeMat);
		Shader.SetGlobalMatrix(PROP_SHADOW_CAM_2_WORLD_MAT, transform.localToWorldMatrix);
	}
	void OnPostRender() {
		Graphics.Blit(_depthTex, _eyeDepthTex, _eyeDepthBakerMat);
	}
	void OnDestroy() {
		ReleaseTexture();
		Destroy(_shadowMesh);
		Destroy(_eyeDepthBakerMat);
	}

	void ReleaseTexture() {
		Destroy(_depthTex);
		Destroy(_eyeDepthTex);
	}

	void UpdateMesh(Mesh mesh, int size) {
		var vertexCount = size * size;
		var vertices = new Vector3[vertexCount];
		var uv = new Vector2[vertexCount];
		var dx = 2f / size;
		var offsetx = -1f;
		var duv = 1f / size;
		for (var vy = 0; vy < size; vy++) {
			for (var vx = 0; vx < size; vx++) {
				var i = vx + size * vy;
				vertices[i] = new Vector3(dx * vx + offsetx, dx * vy + offsetx, 0f);
				uv[i] = new Vector2(duv * vx, duv * vy);
			}
		}

		var ti = 0;
		var edgeCount = size - 1;
		var triangleCount = 6 * edgeCount * edgeCount;
		var triangles = new int[triangleCount];
		for (var ey = 0; ey < edgeCount; ey++) {
			for (var ex = 0; ex < edgeCount; ex++) {
				var vi = ex + ey * size;
				triangles[ti++] = vi;
				triangles[ti++] = vi + size + 1;
				triangles[ti++] = vi + 1;
				triangles[ti++] = vi;
				triangles[ti++] = vi + size;
				triangles[ti++] = vi + size + 1;
			}
		}

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.bounds = new Bounds(Vector3.zero, 1000f * Vector3.one);
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
