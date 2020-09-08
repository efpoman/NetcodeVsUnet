
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text healthtxt;
    [SerializeField]
    TMPro.TMP_Text idtxt;
    [SerializeField]
    TMPro.TMP_Text ballstxt;
    [SerializeField]
    TMPro.TMP_Text killertxt;
    private Player player;

    GameObject objFps;

    public void SetPlayer(Player _player)
    {
        player = _player;
        objFps = GameObject.FindGameObjectWithTag("FPSCount");
    }


    // Update is called once per frame
    void Update()
    {
        healthtxt.text =player.GetHealth().ToString();
        idtxt.text = "Player " +player.netId.ToString();
        ballstxt.text = objFps.GetComponent<FpsCount>().balls.ToString();

        if (player.GetHealth()==0)
            killertxt.text = "Te ha matado: " + this.player.GetKiller();
        else
            killertxt.text = "";
        
    }


}
