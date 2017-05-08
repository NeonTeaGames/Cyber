using UnityEngine.Networking;

public class TextMessage : MessageBase {

    public string Message;
    
    public TextMessage(string message) {
        this.Message = message;
    }

    public TextMessage() {
    }

    public override void Deserialize(NetworkReader reader) {
        Message = reader.ReadString();
    }

    public override void Serialize(NetworkWriter writer) {
        writer.Write(Message);
    }

}
