using System.Drawing.Imaging;
using System.IO;
using QRCoder;
using ECCLevel = QRCoder.QRCodeGenerator.ECCLevel;

namespace Lofi.API.Shared
{
    public static class QRCodeUtils
    {
        public static byte[] CreateBmpQrCode(string plainText, ECCLevel eccLevel = ECCLevel.Q)
        {
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(plainText, eccLevel);
            var qr = new QRCode(data);
            var bmp = qr.GetGraphic(20);
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);
            return ms.ToArray();
        }
    }
}