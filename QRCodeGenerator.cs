using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class QRCodeGenerator : MonoBehaviour
{
    public static Texture2D EncodeTextToQRCode(string content, Vector2Int dimensions, int margin = 0)
    {
        Texture2D qrCodeTex = new Texture2D(dimensions.x, dimensions.y);

        Color32[] qrCode = Encode(content, qrCodeTex.width, qrCodeTex.height, margin);

        qrCodeTex.SetPixels32(qrCode);
        qrCodeTex.Apply();

        return qrCodeTex;
    }

    private static Color32[] Encode(string content, int width, int height, int margin)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = width,
                Height = height,
                Margin = margin
            }
        };

        var qrCode = writer.Write(content);

        return qrCode;
    }
}
