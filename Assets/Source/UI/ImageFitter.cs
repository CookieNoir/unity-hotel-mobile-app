using UnityEngine;
using UnityEngine.UI;

public class ImageFitter : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RawImage _rawImage;

    public async void SetImage(ImageRepository imageRepository, string imagePath)
    {
        Texture2D texture = await imageRepository.GetImage(imagePath);
        float aspect = (float)texture.width / texture.height;
        float width = 1f;
        float height = 1f;
        if (aspect > 1f)
        {
            height /= aspect;
        }
        else
        {
            width = aspect;
        }
        _rectTransform.localScale = new Vector3(width, height, 1f);
        _rawImage.texture = texture;
    }
}
