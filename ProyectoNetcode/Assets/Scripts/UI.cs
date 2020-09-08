using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public static int PlayerIdName;
    public static int KillerIdName;
    public static float vida = 100f;
    public RectTransform healthBarFill;
    [SerializeField]public TMPro.TMP_InputField inputField;

    GameObject objfps;
    float m_Health;

    Canvas m_Canvas;

    public void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        objfps = GameObject.FindGameObjectWithTag("FPSCount");
    }

#pragma warning disable 649
    [SerializeField] TMPro.TMP_Text m_HealthText;
    [SerializeField] TMPro.TMP_Text m_KillerText;
    [SerializeField] TMPro.TMP_Text m_IDText;
    [SerializeField] TMPro.TMP_Text m_BallsText;
    [SerializeField] TMPro.TMP_Text chatText;
#pragma warning restore 649


    private void Update()
    {
        m_IDText.text = "Player " + PlayerIdName.ToString();
        m_BallsText.text = objfps.GetComponent<FpsCount>().balls.ToString();
        m_KillerText.text = "";
        m_HealthText.text = vida.ToString();

        if (vida == 0)
        {
            m_KillerText.text = "Te ha matado: " + KillerIdName.ToString();
        }

    }
   
}
