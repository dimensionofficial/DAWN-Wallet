using NBitcoin;

namespace ProgrammingBlockchainCodeExamples
{
	public class _Bip38 : UnityEngine.MonoBehaviour
	{
		void Start()
		{
			var passphraseCode = new BitcoinPassphraseCode("my secret", Network.Main, null);

            UnityEngine.Debug.Log(passphraseCode);

            EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();
            byte[] temp = encryptedKeyResult.Seed;
            string a = "";
            for (int i = 0; i < temp.Length; i++)
            {
                string b = temp[i].ToString();
                a += b;
            }
            UnityEngine.Debug.Log(a);

            var generatedAddress = encryptedKeyResult.GeneratedAddress;
			var encryptedKey = encryptedKeyResult.EncryptedKey;
			var confirmationCode = encryptedKeyResult.ConfirmationCode;

			UnityEngine.Debug.Log(generatedAddress);	// 14KZsAVLwafhttaykXxCZt95HqadPXuz73
			UnityEngine.Debug.Log(encryptedKey);		// 6PnWtBokjVKMjuSQit1h1Ph6rLMSFz2n4u3bjPJH1JMcp1WHqVSfr5ebNS
			UnityEngine.Debug.Log(confirmationCode);	// cfrm38VUcrdt2zf1dCgf4e8gPNJJxnhJSdxYg6STRAEs7QuAuLJmT5W7uNqj88hzh9bBnU9GFkN

			UnityEngine.Debug.Log(confirmationCode.Check("my secret", generatedAddress));	// True
			var bitcoinPrivateKey = encryptedKey.GetSecret("my secret");
			UnityEngine.Debug.Log(bitcoinPrivateKey.GetAddress() == generatedAddress);		// True
			UnityEngine.Debug.Log(bitcoinPrivateKey);	// KzzHhrkr39a7upeqHzYNNeJuaf1SVDBpxdFDuMvFKbFhcBytDF1R
		}
	}
}