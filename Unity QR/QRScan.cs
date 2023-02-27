using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;

public class QRCodeHandler : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImageBackground;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private TextMeshProUGUI _textOut;
    [SerializeField]
    private RectTransform _scanZone;

    private bool _isCamAvailable;
    private WebCamTexture _cameraTexture;

    void Start()
    {
        QRProductData productData = new QRProductData("ZohoCliq");

        string Json = JsonUtility.ToJson(productData);

        SetUpCamera();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRender();
    }

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            _isCamAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                //_cameraTexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
                _cameraTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }
        _cameraTexture.Play();
        //_rawImageBackground.transform.localScale = new Vector3(-1, -1, 1);
        _rawImageBackground.texture = _cameraTexture;
        _isCamAvailable = true;
    }

    private void UpdateCameraRender()
    {

        if (_isCamAvailable == false)
        {
            //Debug.Log("Camera Not Available");
            return;
        }

        //Debug.Log("Camera texture flipped?  : " + _cameraTexture.videoVerticallyMirrored);

        float ratio = (float)_cameraTexture.width / (float)_cameraTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = _cameraTexture.videoRotationAngle;

        //Debug.Log("CHECK => Rotation Angle BEFORE : " + orientation);

        //orientation = orientation * 3;
        //_rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);

        //Debug.Log("CHECK => Rotation Angle AFTER : " + orientation);

        _rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, orientation * 2, -orientation);
    }
    public void OnClickScan()
    {
        string result = Scan();

        QRProductData productData = JsonUtility.FromJson<QRProductData>(result);

        if (productData == null)
        {
            _textOut.text = "Product Data Not Serialized";
            return;
        }
        _textOut.text = productData.name;
    }


    private string Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width, _cameraTexture.height);
            if (result != null)
            {
                return result.Text;
            }
            return "Failed to read QR Code";
        }
        catch
        {
            return "FAILED IN TRY";
        }
    }
}

[System.Serializable]
public class QRProductData
{
    public string name;

    public QRProductData(string name)
    {
        this.name = name;
    }
}
