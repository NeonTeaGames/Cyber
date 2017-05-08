using UnityEngine.Networking;

/// <summary>
/// Generic Text Message for chat etc.
/// To be removed later when no longer necessary.
/// </summary>
public class TextMessage : MessageBase {

    /// <summary>
    /// Message inside the Text Message. Does not container sender information.
    /// </summary>
    public string Message;
    
    /// <summary>
    /// Create a TextMessage containing the message to be sent.
    /// </summary>
    /// <param name="message"></param>
    public TextMessage(string message) {
        this.Message = message;
    }

    /// <summary>
    /// Parameter-less constructor using when deserializing the message.
    /// </summary>
    public TextMessage() {
    }

    /// <summary>
    /// Used to deserialize a message received via networking.
    /// </summary>
    /// <param name="reader"></param>
    public override void Deserialize(NetworkReader reader) {
        Message = reader.ReadString();
    }

    /// <summary>
    /// Used to serialize the message before it is sent.
    /// </summary>
    /// <param name="writer"></param>
    public override void Serialize(NetworkWriter writer) {
        writer.Write(Message);
    }

}
