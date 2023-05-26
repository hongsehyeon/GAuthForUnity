using System;
using System.Collections;
using UnityEngine;
using GAuthForUnity.Core;

namespace GAuthForUnity
{
    public class GAuthManager : MonoBehaviour
    {
        #region Signleton
        private static readonly object _lock = new object();
        private static GAuthManager _instance;
        public static GAuthManager GAuth
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = FindObjectOfType<GAuthManager>();

                        if (_instance == null)
                        {
                            GameObject obj = new GameObject() { name = "GAuthManager" };
                            _instance = obj.AddComponent<GAuthManager>();
                        }
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            lock (_lock)
            {
                if (_instance == null) _instance = this;
                else Destroy(_instance);
            }
        }

        private void OnDestroy()
        {
            lock (_lock)
            {
                if (_instance != this) return;
                _instance = null;
            }
        }
        #endregion

        #region Private Field
        private readonly GAuth _gAuth = new GAuthCore();
        private string _clientId;
        private string _clientSecret;
        private string _redirectUri;
        #endregion

        #region Public
        /// <summary>
        /// 기본 정보를 설정합니다.
        /// </summary>
        /// <param name="clientId">GAuth에서 발급받은 클라이언트 ID</param>
        /// <param name="clientSecret">GAuth에서 발급받은 클라이언트 SECRET</param>
        /// <param name="redirectUri">리다이렉트 URI</param>
        public void Init(string clientId, string clientSecret, string redirectUri)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
        }

        /// <summary>
        /// 서버에서 유저 정보를 받아옵니다.
        /// </summary>
        /// <param name="email">GAuth 계정 이메일</param>
        /// <param name="password">GAuth 계정 비밀번호</param>
        /// <param name="callback">콜백 함수</param>
        public void GetUserInfo(string email, string password, Action<GAuthUserInfo> callback)
        {
            StartCoroutine(GetUserInfoCoroutine(email, password, callback));
        }
        #endregion

        #region Private
        private IEnumerator GetUserInfoCoroutine(string email, string password, Action<GAuthUserInfo> callback)
        {
            var task1 = _gAuth.GenerateTokenAsync(email, password, _clientId, _clientSecret, _redirectUri);
            while (!task1.IsCompleted)
                yield return null;

            if (task1.IsFaulted || task1.IsCanceled)
            {
                HandleError("GenerateTokenAsync Failed");
                yield break;
            }

            GAuthToken token = task1.Result;
            var task2 = _gAuth.GetUserInfoAsync(token.AccessToken);
            while (!task2.IsCompleted)
                yield return null;

            if (task2.IsFaulted || task2.IsCanceled)
            {
                HandleError("GetUserInfoAsync Failed");
                yield break;
            }

            callback?.Invoke(task2.Result);
        }

        private void HandleError(string code)
        {
            Debug.LogError($"Handle Error : {code}");
        }
        #endregion
    }
}
