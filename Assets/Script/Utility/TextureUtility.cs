using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextureUtility : MonoBehaviour
{
    private Dictionary<string, Texture> textureDict = new Dictionary<string, Texture>();

    public void GetTexture(string full_url, System.Action<Texture> callback) {

        if (textureDict.TryGetValue(full_url, out Texture texture)) {
            if (callback != null)
                callback(texture);
            return;
        }

        StartCoroutine(GetText(full_url, callback));
    }

    private IEnumerator GetText(string full_url, System.Action<Texture> callback)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(full_url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(uwr);

                // Get downloaded asset bundle
                if (!textureDict.ContainsKey(full_url)) {

                    textureDict.Add(full_url, texture);
                }

                if (callback != null)
                    callback(texture);
            }
        }
    }

 
}
