using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class ARCameraFeed : MonoBehaviour
{
    public RawImage rawImage;
    private ARCameraManager arCameraManager;

    void Start()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager != null)
        {
            arCameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    void OnDestroy()
    {
        if (arCameraManager != null)
        {
            arCameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height),
                outputDimensions = new Vector2Int(image.width, image.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };

            var texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
            var rawTextureData = texture.GetRawTextureData<byte>();
            var rawTextureDataPtr = rawTextureData.GetUnsafePtr();
            image.Convert(conversionParams, new System.IntPtr(rawTextureDataPtr), rawTextureData.Length);
            texture.Apply();

            rawImage.texture = texture;
            image.Dispose();
        }
    }
}
