using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
namespace HardwareWallet
{
    using System;
    using UnityEngine;

    public class SaveAndLoad
    {
        public static void Save(string key, byte[] data, byte[] sec)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] newsec = sha256.ComputeHash(sec);
            byte[] byteToSave = Encry.EncryData(data, newsec);
            string result = string.Empty;
            for (int i = 0; i < byteToSave.Length; i++)
            {
                result += "$" + System.Convert.ToString(byteToSave[i], 16);
            }
            PlayerPrefs.SetString(key, result);
        }

        public static string LoadString(string key, byte[] sec)
        {
            byte[] data = Load(key, sec);
            if (data == null)
            {
                return null;
            }
            return System.Text.UTF8Encoding.UTF8.GetString(data);
        }

        public static byte[] Load(string key, byte[] sec)
        {
            if (PlayerPrefs.HasKey(key))
            {
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] newsec = sha256.ComputeHash(sec);
                string data = PlayerPrefs.GetString(key);
                string[] chars = data.Split(new char[] { '$' }, System.StringSplitOptions.RemoveEmptyEntries);
                byte[] b = new byte[chars.Length];
                for (int i = 0; i < chars.Length; i++)
                {
                    b[i] = System.Convert.ToByte(chars[i], 16);
                }
                try
                {
                    return Encry.DecryData(b, newsec);
                } catch (Exception e)
                {
                    return null;
                }
               
            }
            return null;
        }

    }
}
