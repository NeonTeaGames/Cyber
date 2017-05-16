using Cyber.Networking;
using Cyber.Networking.Clientside;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDebugControl : MonoBehaviour {

    public Text PacketLossNumberText;
    public Text PingNumberText;
    public Text PacketsReceivedNumberText;

    public RectTransform ActualPanel;

    private bool visible = false;

    private double LastPingChecked = 0.5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("ToggleNetworkDebug")) {
            visible = !visible && Client.IsRunning();
            ActualPanel.gameObject.SetActive(visible);
        }
        if (visible) {
            LastPingChecked += Time.deltaTime;
            if (LastPingChecked > 0.5) {
                PingNumberText.text = ((int) (Client.GetSyncHandler().GetPing())) + "ms";
                LastPingChecked -= 0.5;
            }
            PacketLossNumberText.text = ((int) (Client.GetSyncHandler().GetPacketLoss() * 10000) * 1f / 100) + "%";
            PacketsReceivedNumberText.text = Client.GetSyncHandler().GetSyncPacketsReceived() + "";
        }
	}
}
