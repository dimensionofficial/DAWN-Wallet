using System.Collections;
using System.Collections.Generic;
using ZXing;
using ZXing.QrCode;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeManager
{
    public void EncodeQRCode(Image qrCodeImage, string code)
    {
        var encoded = GetQRTexture2D(code);
        qrCodeImage.sprite = Sprite.Create(encoded, Rect.MinMaxRect(0, 0, encoded.width, encoded.height), Vector2.one);
        qrCodeImage.gameObject.SetActive(true);
    }

    public Texture2D GetQRTexture2D(string info)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(info, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();

        return encoded;
    }

    static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
                Margin = 0
            }
        };
        return writer.Write(textForEncoding);
    }
}
