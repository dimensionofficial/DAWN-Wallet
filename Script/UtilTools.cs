using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilTools
{
    public static List<GameObject> allDontDestroyOnLoad = new List<GameObject>();
    private static int m_seed = 0;

    public static int GenerateSeed()
    {
        m_seed++;
        return m_seed;
    }

    public static float FrameToDuration(int frame)
    {
        return frame * Time.fixedDeltaTime;
    }

    public static void SetDontDestroyOnLoad(GameObject go)
    {
        if (allDontDestroyOnLoad.Contains(go))
        {
            return;
        }
        GameObject.DontDestroyOnLoad(go);
        allDontDestroyOnLoad.Add(go);
    }

    /// <summary>
    /// 判断当前播放的动画是否是指定动画
    /// </summary>
    /// <param name="mator"></param>
    /// <param name="animationName"></param>
    /// <returns></returns>
    public static bool IsPlayAnimationName(Animator mator, string animationName, int layerIndex = 0)
    {
        return mator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animationName);
    }

    /// <summary>
    /// 判断当前动画是否播放完（非连续动画）
    /// </summary>
    /// <param name="mator"></param>
    /// <returns></returns>
    public static bool IsAnimationPlayOver(Animator mator, int layerIndex = 0)
    {
        if (mator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= 0.95F)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static float GetAnimationLessNormalizedTime(Animator mator, int layerIndex = 0)
    {
        return mator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
    }

    /// <summary>
    /// 获取当前动画的时间长
    /// </summary>
    /// <param name="mator"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public static float GetCurrentAnimationLength(Animator mator, int layerIndex = 0)
    {
        mator.Update(0);
        return mator.GetCurrentAnimatorStateInfo(layerIndex).length;
    }


    public static void PlayAnimationAndSetSpeed(float playTime, Animator mator, string mationName, string parameterName, int layerIndex = 0)
    {
        mator.Play(mationName);
        mator.Update(0);
        SetCurrentAniationSpeed(playTime, mator, parameterName, layerIndex);
    }

    public static void SetCurrentAniationSpeed(float playTime, Animator mator, string parameterName, int layerIndex = 0)
    {
        float length = UtilTools.GetCurrentAnimationLength(mator, layerIndex);
        float currentSpeed = length / playTime;
        mator.SetFloat(parameterName, currentSpeed);
    }

    public static GameObject GetResourcesGameObject(string path)
    {
        return GameObject.Instantiate((GameObject)Resources.Load(path, typeof(GameObject)));
    }

    public static T GetResourcesObject<T>(string path)        
    {
        GameObject go = GetResourcesGameObject(path);
        return go.GetComponent<T>();
    }

    public static GameObject Instantiate(GameObject prefab, Transform parent, bool isActive = true)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.transform.SetParent(parent, false);
        go.SetActive(isActive);
        return go;
    }
}
