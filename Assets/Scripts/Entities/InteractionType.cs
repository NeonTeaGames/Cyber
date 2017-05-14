
namespace Cyber.Entities {

    /// <summary>
    /// Describes the type of interaction being made.
    /// </summary>
    public enum InteractionType : byte {

        /// <summary>
        /// The <see cref="SyncBases.Interactable"/> is clicked or otherwise activated.
        /// </summary>
        Activate,

        /// <summary>
        /// The <see cref="SyncBases.Interactable"/> is called when for example the mouse is released of it.
        /// </summary>
        Deactivate,

        /// <summary>
        /// When the <see cref="SyncBases.Interactable"/> is hovered on.
        /// </summary>
        Enter,

        /// <summary>
        /// 
        /// </summary>
        Exit

    }
}
