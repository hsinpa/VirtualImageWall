using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextureUtility : MonoBehaviour
{
    private Dictionary<string, Sprite> textureDict = new Dictionary<string, Sprite>();

    public void GetTexture(string full_url, System.Action<Sprite> callback) {

        if (textureDict.TryGetValue(full_url, out Sprite texture)) {
            if (callback != null)
                callback(texture);
            return;
        }

        StartCoroutine(GetText(full_url, callback));
    }

    private IEnumerator GetText(string full_url, System.Action<Sprite> callback)
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
                var texSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);


                // Get downloaded asset bundle
                if (!textureDict.ContainsKey(full_url)) {

                    textureDict.Add(full_url, texSprite);
                }

                if (callback != null)
                    callback(texSprite);
            }
        }
    }

 
}
