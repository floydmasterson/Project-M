using UnityEngine;
using UnityEngine.SceneManagement;

namespace SmartConsole.Demo
{
    /// <summary>
    /// Script that initialize some util commands
    /// Must derive from CommandBehaviour that derive from MonoBehaviour
    /// </summary>
    public class DefaultCommands : CommandBehaviour
    {
        /// <summary>
        /// Start function must be overrided
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /* write command functions using snake case convention */


    }
}
