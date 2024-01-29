using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OverdrawChecker
{

    public class OverdrawChecker : MonoBehaviour
    {
        [SerializeField] int _borderCount;
        [SerializeField] Text _text;
        [SerializeField] RawImage _rawImage;
        [SerializeField] Camera _camera;
        [SerializeField] ParentConstraint _pConst;
        [SerializeField] ComputeShader _cs;
        Vector2Int _screenRes;
        RenderTexture _rt;
        int count = 0;
        const int DivCount = 32;
        const int Interval = 10;

        readonly int ResultPropertyID = Shader.PropertyToID("_Result");
        readonly int OverdrawTexPropertyID = Shader.PropertyToID("_OverdrawTex");
        readonly int DivCountPropertyID = Shader.PropertyToID("_DivCount");
        readonly int CountPropertyID = Shader.PropertyToID("_Count");
        readonly int ResolutionPropertyID = Shader.PropertyToID("_Resolution");
        int kernel;
        ComputeBuffer buffer;
        int[] datas;

        void OnEnable()
        {
#if UNITY_EDITOR
            var res = UnityStats.screenRes.Split('x');
            _screenRes = new Vector2Int(int.Parse(res[0]), int.Parse(res[1]));
#else
            _screenRes = new Vector2Int(Screen.width, Screen.height);
#endif
            _rt = new RenderTexture(_screenRes.x, _screenRes.y, 0, RenderTextureFormat.RFloat);
            _camera.targetTexture = _rt;
            _rawImage.texture = _rt;
            _rawImage.enabled = true;

            var mainCam = Camera.main;
            var source = new ConstraintSource();
            source.sourceTransform = mainCam.transform;
            source.weight = 1;
            _pConst.SetSource(0, source);
            _camera.orthographic = mainCam.orthographic;
            _camera.nearClipPlane = mainCam.nearClipPlane;
            _camera.farClipPlane = mainCam.farClipPlane;
            _camera.fieldOfView = mainCam.fieldOfView;
            _camera.orthographicSize = mainCam.orthographicSize;

            kernel = _cs.FindKernel("CSMain");
            datas = new int[DivCount * DivCount];
            buffer = new ComputeBuffer(DivCount * DivCount, sizeof(int));

            _cs.SetBuffer(kernel, ResultPropertyID, buffer);
            _cs.SetTexture(kernel, OverdrawTexPropertyID, _rt);
            _cs.SetInt(DivCountPropertyID, DivCount);
            _cs.SetInt(CountPropertyID, (_rt.width / DivCount) * (_rt.height / DivCount));
            _cs.SetVector(ResolutionPropertyID, new Vector4(_rt.width, _rt.height, _rt.width / DivCount, _rt.height / DivCount));
        }

        void OnDisable()
        {
            _rt.Release();
            buffer.Release();
        }

        void Update()
        {
            if (count > Interval)
            {
                _cs.Dispatch(kernel, DivCount, DivCount, 1);

                buffer.GetData(datas);
                
                int overdrawValue = 0;
                foreach (var data in datas)
                {
                    overdrawValue += data;
                }
                overdrawValue = (int)Mathf.Ceil((float)overdrawValue / (float)(DivCount * DivCount));

                _text.text = $"Overdraw : {overdrawValue.ToString()}%";
                if (overdrawValue > _borderCount)
                {
                    _text.color = new Color(1, 0, 0, 1);
                }
                else
                {
                    _text.color = new Color(1, 1, 1, 1);
                }

                count = 0;

            }
            else
            {
                count++;
            }
        }
    }
}