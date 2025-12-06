using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(TiffSlicer))]
public class RemoteTiffLoader : MonoBehaviour
{
    [Tooltip("The URL of the .tif file to download.")]
    public string tiffUrl = "https://your-server.com/path/to/your/file.tif";

    private TiffSlicer tiffSlicer;

    void Start()
    {
        tiffSlicer = GetComponent<TiffSlicer>();
        
        StartCoroutine(DownloadAndSliceTiff());
    }

    private IEnumerator DownloadAndSliceTiff()
    {
        Debug.Log($"Starting download from: {tiffUrl}");

        UnityWebRequest www = UnityWebRequest.Get(tiffUrl);
        www.certificateHandler = new AcceptAllCertificatesHandler();
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to download TIFF file: {www.error}");
        }
        else
        {
            Debug.Log("Download complete!");
            
            byte[] tiffData = www.downloadHandler.data;

            tiffSlicer.LoadTiffFromData(tiffData);

            byte[] pngDataXZ = tiffSlicer.GetSlice("xz", 128);
            if (pngDataXZ != null)
            {
                System.IO.File.WriteAllBytes("remote_slice_xz_128.png", pngDataXZ);
                Debug.Log("Saved remote XZ slice to remote_slice_xz_128.png");
            }

            byte[] pngDataYZ = tiffSlicer.GetSlice("yz", 64);
            if (pngDataYZ != null)
            {
                System.IO.File.WriteAllBytes("remote_slice_yz_64.png", pngDataYZ);
                Debug.Log("Saved remote YZ slice to remote_slice_yz_64.png");
            }
        }
    }
}
