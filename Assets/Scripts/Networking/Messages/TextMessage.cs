using UnityEngine.Networking;

public class TextMessage : MessageBase {

    public string message;
    
    public TextMessage(string message) {
        this.message = message;
    }

    public TextMessage() {
    }

    public override void Deserialize(NetworkReader reader) {
        message = reader.ReadString();
    }

    public override void Serialize(NetworkWriter writer) {
        writer.Write(message);
    }

}
