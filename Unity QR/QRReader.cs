using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;

public class QRReader : MonoBehaviour
{
    [SerializeField]
    private DeviceCameraController deviceCameraController;
    [SerializeField]
    private string scannedText;

    private bool _isScanning = false;

    private void Start()
    {
        //StopScanning();
    }

    void Update()
    {
        if (_isScanning)
        {
            string result = Scan();
            scannedText = result;
        }
    }

    public void StartScanning()
    {
        _isScanning = true;
        deviceCameraController.SetCanUpdateCamera(true);
    }

    public void StopScanning()
    {
        _isScanning = false;
        deviceCameraController.SetCanUpdateCamera(false);
    }

    public string GetScannedText()
    {
        return scannedText;
    }

    public void ClearScannedText()
    {
        scannedText = "";
    }

    private string Scan()
    {
        try
        {
            WebCamTexture activeCamTexture = deviceCameraController.GetActiveWebCamTexture();
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(activeCamTexture.GetPixels32(), activeCamTexture.width, activeCamTexture.height);
            if (result != null)
            {
                return result.Text;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
