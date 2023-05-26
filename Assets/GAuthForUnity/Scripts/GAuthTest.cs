using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GAuthForUnity;
using TMPro;

public class GAuthTest : MonoBehaviour
{
    [Header("GAuth")]
    [Tooltip("GAuth���� �߱޹��� Ŭ���̾�Ʈ ID")]
    [SerializeField]
    private string _clientId = "fa273bb46a134f33a4c9cf739b0a637fa8a9a3b29f66414b842749e163e953ac";

    [Tooltip("GAuth���� �߱޹��� Ŭ���̾�Ʈ SECRET")]
    [SerializeField]
    private string _clientSecret = "b1167248f40f42d7b298d20be5b99cad493277666b1c43a7a445eb6abc6a2e9a";

    [Tooltip("�����̷�Ʈ URI")]
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
            // ���� ó�� ���� (����)
            LoginPanel.SetActive(false);
            ResultText.text = $"{userInfo.Grade}�г� {userInfo.ClassNum}�� {userInfo.Num}�� {userInfo.Name}�л��� ������ �ҷ��Խ��ϴ�!";
        });
    }
}
