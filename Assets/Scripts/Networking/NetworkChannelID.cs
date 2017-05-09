
namespace Cyber.Networking {
    
    /// <summary>
    /// Networked channel ID's to be used.
    /// </summary>
    public class NetworkChannelID {

        /// <summary>
        /// The default channel which is reliable and sequenced. True TCP!
        /// </summary>
        public static byte ReliableSequenced;

        /// <summary>
        /// Another channel which is unreliable (like UDP) but always in correct sending order (like TCP).
        /// </summary>
        public static byte UnreliableSequenced;

        /// <summary>
        /// Very unreliable, true UDP!
        /// </summary>
        public static byte Unreliable;
    }
}
