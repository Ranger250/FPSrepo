using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;

public class ChatBox : MonoBehaviourPun
{

    public TextMeshProUGUI chatLog;
    public TMP_InputField chatInput;

    public static ChatBox instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        chatLog.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Cursor.lockState = CursorLockMode.None;
            if (EventSystem.current.currentSelectedGameObject == chatInput.gameObject)
            {
                onChatInputSend();
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
            }
        }
    }

    public void onChatInputSend()
    {
        if (chatInput.text.Length > 1)
        {
            Color my_color = GameManager.instance.playerColors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            string hexCC = ToRRBHex(my_color);

            
            photonView.RPC("log", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatInput.text, hexCC);
            chatInput.text = "";
        }
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Locked;
    }

    [PunRPC]
    void log(string playerName, string msg, string color)
    {
        chatLog.text += string.Format("<color={2}><b>{0}:</b></color> {1}\n", playerName, msg, color);
    }

    public static string ToRRBHex(Color c)
    {
        return string.Format("#{0:x2}{1:x2}{2:x2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
}
