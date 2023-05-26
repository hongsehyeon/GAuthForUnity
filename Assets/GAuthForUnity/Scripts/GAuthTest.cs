using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GAuthForUnity;
using TMPro;

public class GAuthTest : MonoBehaviour
{
    [Header("GAuth")]
    [Tooltip("GAuth에서 발급받은 클라이언트 ID")]
    [SerializeField]
    private string _clientId = "fa273bb46a134f33a4c9cf739b0a637fa8a9a3b29f66414b842749e163e953ac";

    [Tooltip("GAuth에서 발급받은 클라이언트 SECRET")]
    [SerializeField]
    private string _clientSecret = "b1167248f40f42d7b298d20be5b99cad493277666b1c43a7a445eb6abc6a2e9a";

    [Tooltip("리다이렉트 URI")]
    [SerializeField]
    private string _redirectUri = "https://ihate.csharp.gg/";

    [Header("References")]
    public TMP_InputField EmailInputField;
    public TMP_InputField PasswordInputField;
    public GameObject LoginPanel;
    public TMP_Text ResultText;

    private void Start()
    {
        GAuthManager.GAuth.Init(_clientId, _clientSecret, _redirectUri);
    }

    public void OnClickLoginButton()
    {
        GAuthManager.GAuth.GetUserInfo(EmailInputField.text, PasswordInputField.text, (userInfo) =>
        {
            // 성공 처리 로직 (예시)
            LoginPanel.SetActive(false);
            ResultText.text = $"{userInfo.Grade}학년 {userInfo.ClassNum}반 {userInfo.Num}번 {userInfo.Name}학생의 정보를 불러왔습니다!";
        });
    }
}
