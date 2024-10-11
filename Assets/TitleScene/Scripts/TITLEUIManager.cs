using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DB;

public class TITLEUIManager : MonoBehaviour
{
    //Variables

    public bool isLogInSuccess = false;

    // sign in
    public GameObject signInObject;
    private Animator signInAnimator;

    public TextMeshProUGUI signInIDText;
    public TMP_InputField signInPWDText;

    //sign up
    public GameObject signUpObject;
    private Animator signUpAnimator;

    public TMP_InputField signUpIDText;
    public TMP_InputField signUpPWDText;
    public TMP_InputField signUpPWDConFirmText;


    // notices
    public GameObject noticeMessageObject;
    public TextMeshProUGUI noticeText;

    private Animator noticeMessageAnimator;

    //FireBase

    public class PlayerInfo
    {
        public string id;
        public string pwd;

        public PlayerInfo() { }

        public PlayerInfo(string id, string pwd)
        {
            this.id = id;
            this.pwd = pwd;
        }
    }

    DatabaseReference reference;

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    private void Awake()
    {
        signInAnimator = signInObject.GetComponent<Animator>(); 
        signUpAnimator = signUpObject.GetComponent<Animator>();
        noticeMessageAnimator = noticeMessageObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        signInObject.SetActive(true);
        signUpObject.SetActive(false);
        noticeMessageObject.SetActive(false);

        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        signInObject.SetActive(true);
    }



    

    // SignIn 버튼 누를시 올바른 입력인지 확인하고 FireBase 체크를 하는 함수
    public void CheckSignInTexts()
    {
        string id = signInIDText.text;
        string pwd = signInPWDText.text.Trim();

        if (id == string.Empty || pwd == string.Empty)
        {
            NoticeMessageAppear("ID 와 PW 를 입력해주세요");
            return;
        }

        reference.Child("users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string pwdCheck = snapshot.Child("pwd").GetValue(true).ToString().Trim();


                    Debug.Log($"pwdCheck: {pwdCheck} (Length: {pwdCheck.Length})");
                    Debug.Log($"pwd: {pwd} (Length: {pwd.Length})");
                    Debug.Log(pwdCheck + " " + pwd);

                    if (pwdCheck == pwd)
                    {
                        NoticeMessageAppear("로그인 성공");
                        isLogInSuccess = true;
                    }
                    else
                    {
                        NoticeMessageAppear("패스워드가 틀립니다.");
                    }
                }
                else
                {
                    NoticeMessageAppear("존재하지 않는 아이디 입니다.");
                }
            }
            else
            {
                NoticeMessageAppear("인터넷 연결을 확인해 주세요");
            }
        });


    }



    // SignUp 버튼 누르시 확인후 FireBase 확인
    public void CheckSignUpTexts()
    {
        string id = signUpIDText.text;
        string pwd = signUpPWDText.text;
        string pwdCF = signUpPWDConFirmText.text;

        if (id == string.Empty || pwd == string.Empty)
        {
            NoticeMessageAppear("아이디 비밀번호를 입력해주세요");
            return;
        }
        else if (pwd != pwdCF)
        {
            NoticeMessageAppear("비밀번호가 똑같지 않습니다");
            return;
        }

        reference.Child("users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    NoticeMessageAppear("이미 존재하는 아이디입니다");
                }
                else
                {
                    NoticeMessageAppear("아이디 생성 완료!");

                    signUpIDText.text = "";
                    signUpPWDText.text = "";
                    signUpPWDConFirmText.text = "";

                    PlayerInfo playerInfo = new PlayerInfo(id,pwd);
                    string json = JsonUtility.ToJson(playerInfo);
                    reference.Child("users").Child(id).SetRawJsonValueAsync(json);

                    
                }
            }
            else
            {
                NoticeMessageAppear("인터넷 연결을 확인해 주세요");
            }
        });
    }



    //SignUpUI 함수들
    public void SignUpAppear()
    {
        if (signUpObject.activeSelf == false) signUpObject.SetActive(true);
        else signUpAnimator.SetTrigger("windowIn");
    }
    public void SignUpDisAppear()
    {
        signUpAnimator.SetTrigger("windowOut");
    }
    // NoticeMessage 함수들
    public void NoticeMessageAppear(string noticeMessageText)
    {
        noticeText.text = noticeMessageText;
        if (noticeMessageObject.activeSelf == false) noticeMessageObject.SetActive(true);
        else noticeMessageAnimator.SetTrigger("windowIn");
    }
    public void NoticeMessageDisAppear()
    {
        noticeMessageAnimator.SetTrigger("windowOut");

        if (isLogInSuccess == true)
        {
            StartCoroutine("GotoMainScene");
        }
    }

    IEnumerator GotoMainScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainScene");
    }
}
