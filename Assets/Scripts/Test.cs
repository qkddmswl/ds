using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
   public void TakeScreenshotAndSaveFunc()
    {
        StartCoroutine("TakeScreenshotAndSave");
    }

    IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png"));

        // To avoid memory leaks
        Destroy(ss);
    }

    public void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;

                Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy(texture, 5f);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }

    public void PickVideo()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the selected video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
    }
}
