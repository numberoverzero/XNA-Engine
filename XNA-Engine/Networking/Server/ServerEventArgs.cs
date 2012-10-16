using System;
using System.Collections.Generic;

namespace Engine.Networking
{
    /// <summary>
    ///   Events passed when an authentication attempt occurs.
    /// </summary>
    public class ServerEventArgs : EventArgs
    {
        /// <summary>
        ///   Initialize a set of parameterless server interaction args at the current time
        /// </summary>
        /// <param name="success"> </param>
        /// <param name="client"> </param>
        public ServerEventArgs(bool success, Client client)
            : this(success, client, null)
        {
        }

        /// <summary>
        ///   Initialize a set of server interaction args at the current time with the given parameters
        /// </summary>
        /// <param name="success"> </param>
        /// <param name="client"> </param>
        /// <param name="parameters"> </param>
        public ServerEventArgs(bool success, Client client, IDictionary<string, string> parameters)
            : this(success, client, parameters, DateTime.Now)
        {
        }

        /// <summary>
        ///   Initialize a set of server interaction args at the given time with the given parameters
        /// </summary>
        /// <param name="success"> </param>
        /// <param name="client"> </param>
        /// <param name="parameters"> </param>
        /// <param name="time"> </param>
        public ServerEventArgs(bool success, Client client, IDictionary<string, string> parameters, DateTime time)
        {
            Success = success;
            Client = client;
            Parameters = parameters ?? new Dictionary<string, string>();

            Time = time;
        }

        /// <summary>
        ///   Was the interaction successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///   The TcpClient that interacted
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        ///   When the interaction occured
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///   Keyed values that the client provided to the server during the interaction
        /// </summary>
        public IDictionary<string, string> Parameters { get; set; }
    }
}