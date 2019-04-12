using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TextureUIAsset : MonoBehaviour
{
    public Sprite cLogo;
    public Sprite hLogo;

    public Sprite tokenDefalutIcon;
    public Sprite btcIcon;
    public Sprite usdtIcon;
    public Sprite ethIcon;
    public Sprite eosIcon;

    public Sprite btcUnit;
    public Sprite rmbUnit;
    public Sprite usdUnit;

	public Sprite sendMark;
	public Sprite getMark;
	public Sprite myselMark;
    public Sprite eos_ram_in;
    public Sprite eos_delegatebw_in;
    public Sprite eos_delegatebw_out;
    public Sprite eos_ram_out;
    public Sprite eos_ram_in_big;
    public Sprite eos_delegatebw_in_big;
    public Sprite eos_delegatebw_out_big;
    public Sprite eos_ram_out_big;

    public static TextureUIAsset _Instance;

    void Awake()
    {
        _Instance = this;
    }

    public IEnumerator LoadImage(Image image,  string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            image.gameObject.SetActive(true);
            image.sprite = TextureUIAsset._Instance.tokenDefalutIcon;
        }
        else
        {
            string[] sp = path.Split('/');
            string fileName = sp[sp.Length - 1];
            if (!Directory.Exists(Application.temporaryCachePath + "/imagesa/"))
            {
                Directory.CreateDirectory(Application.temporaryCachePath + "/imagesa/");
            }
            if (EthTokenManager._Intance.tokenIconDic.ContainsKey(path))
            {
                Sprite sprites = EthTokenManager._Intance.tokenIconDic[path];
                image.sprite = sprites;
                image.gameObject.SetActive(true);
            }
            else if (File.Exists(Application.temporaryCachePath + "/imagesa/" + fileName))
            {
                byte[] b = File.ReadAllBytes(Application.temporaryCachePath + "/imagesa/" + fileName);
                Texture2D texture = new Texture2D(40, 40);
                bool bbb = texture.LoadImage(b);
                if (!bbb)
                {
                    File.Delete(Application.temporaryCachePath + "/imagesa/" + fileName);
                    StartCoroutine(LoadImage(image, path));
                }
                else
                {
                    texture.Apply();
                    if (texture != null)
                    {
                        Sprite sprites = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        image.sprite = sprites;
                        image.gameObject.SetActive(true);
                        EthTokenManager._Intance.tokenIconDic.Add(path, sprites);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(path) && !path.StartsWith("http"))
                {
                    path = "https://etherscan.io" + path;
                }
                WWW www = new WWW(path);
                yield return www;
                Texture2D texture = www.texture;
                if (texture != null)
                {
                    if (image == null)
                        yield break;
                    Sprite sprites = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    image.sprite = sprites;
                    image.gameObject.SetActive(true);
                    if (!EthTokenManager._Intance.tokenIconDic.ContainsKey(path))
                    {
                        EthTokenManager._Intance.tokenIconDic.Add(path, sprites);
                        using (FileStream fs = new FileStream(Application.temporaryCachePath + "/imagesa/" + fileName, FileMode.OpenOrCreate))
                        {
                            byte[] da = www.bytes;
                            fs.Write(da, 0, da.Length);
                        }
                    }
                }
            }

        }
    }

}
