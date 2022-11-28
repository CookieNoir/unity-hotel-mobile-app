using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Hotel.Repositories
{
    public class ImageRepository : MonoBehaviour
    {
        [SerializeField] private Texture2D _placeholderImage;
        private Dictionary<string, Texture2D> _loadedImages;

        private void Awake()
        {
            _loadedImages = new Dictionary<string, Texture2D>();
            _loadedImages.Add("placeholder", _placeholderImage);
        }

        public async Task<Texture2D> GetImage(string url)
        {
            if (!_loadedImages.ContainsKey(url) || _loadedImages[url] == null)
            {
                Texture2D texture = await _GetRemoteTexture(url);
                _loadedImages.Add(url, texture);
            }
            return _loadedImages[url];
        }

        private async Task<Texture2D> _GetRemoteTexture(string url)
        {
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            www.timeout = 5;
            var operation = www.SendWebRequest();

            while (!operation.isDone) await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{www.error}, URL:{www.url}");
                return _placeholderImage;
            }
            return DownloadHandlerTexture.GetContent(www);
        }
    }
}