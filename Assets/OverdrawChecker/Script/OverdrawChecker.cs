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
        const int DivCount = 8;
        const int Interval = 10;

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
        }

        void OnDisable()
        {
            _rt.Release();
        }

        void Update()
        {
            if (count > Interval)
            {
                int num = DivCount * DivCount;
                ComputeBuffer buffer = new ComputeBuffer(num, sizeof(int));

                int kernel = _cs.FindKernel("CSMain");

                _cs.SetBuffer(kernel, "_Result", buffer);
                _cs.SetTexture(kernel, "_OverdrawTex", _rt);
                _cs.SetInt("_DivCount", DivCount);
                _cs.SetInt("_Count", (_rt.width / DivCount) * (_rt.height / DivCount));
                _cs.SetVector("_Resolution", new Vector4(_rt.width, _rt.height, _rt.width / DivCount, _rt.height / DivCount));

                _cs.Dispatch(kernel, DivCount, DivCount, 1);

                int[] datas = new int[num];
                buffer.GetData(datas);
                buffer.Release();

                int overdrawValue = 0;
                foreach (var data in datas)
                {
                    overdrawValue += data;
                }
                overdrawValue = (int)Mathf.Ceil((float)overdrawValue / (float)num);

                _text.text = "Overdraw : " + overdrawValue + "%";
                if (overdrawValue > _borderCount)
                {
                    _text.color = new Color(1, 0, 0, 1);
                }
                else
                {
                    _text.color = new Color(1, 1, 1, 1);
                }

            }
            else
            {
                count++;
            }
        }
    }
}