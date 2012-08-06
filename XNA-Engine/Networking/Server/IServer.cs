using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Networking
{
    /// <summary>
    /// Receives and Sends messages, handles Connect/Disconnect and Auth
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// If the server has been started, returns true.
        /// Returns false if the server is stopped or shutdown.
        /// A server cannot be started from shutdown.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Start the server.  Until the server is started, it cannot handle messages
        /// Cannot be started after Shutdown
        /// </summary>
        void Start();
        /// <summary>
        /// Stop handling messages.
        /// Can be resumed with Start
        /// </summary>
        void Stop();
        /// <summary>
        /// Completely release all clients and stop handling messages.
        /// Cannot be started after a shutdown.
        /// Sends a shutdown message to clients unless immediate is true
        /// </summary>
        /// <param name="immediate"></param>
        void Shutdown(bool immediate = false);

        /// <summary>
        /// Tries to connect a client to the Server.
        /// Fires OnConnect, even if client does not successfully connect
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        void Connect(Client client, ServerEventArgs e = null);
        /// <summary>
        /// Tries to disconnect a client to the Server.
        /// Fires OnDisconnect, even if client does not successfully disconnect
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        void Disconnect(Client client, ServerEventArgs e = null);
        /// <summary>
        /// Tries to authenticate a client.
        /// Fires OnAuthenticate, even if client does not successfully authenticate
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        void Authenticate(Client client, ServerEventArgs e = null);

        /// <summary>
        /// Fired when the server is started
        /// </summary>
        event EventHandler OnStart;
        /// <summary>
        /// Fired when the server is stopped
        /// </summary>
        event EventHandler OnStop;
        /// <summary>
        /// Fired when the server is shut down
        /// </summary>
        event EventHandler OnShutdown;

        /// <summary>
        /// Fired when a client attempts to connect to the server
        /// </summary>
        event EventHandler<ServerEventArgs> OnConnect;
        /// <summary>
        /// Fired when a client attempts to disconnect to the server
        /// </summary>
        event EventHandler<ServerEventArgs> OnDisconnect;
        /// <summary>
        /// Fired when a client attempts to authenticate
        /// </summary>
        event EventHandler<ServerEventArgs> OnAuthenticate;

        
        /// <summary>
        /// Receives a message from a client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="client"></param>
        void ReceiveMsg(string msg, Client client);

        /// <summary>
        /// Sends a message to a specific group of clients.
        /// Sends to all connected clients if the list is empty
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="client"></param>
        void SendMsg(string msg, params Client[] client);

        /// <summary>
        /// Checks if the client has authenticated with the server,
        /// and if that authentication is still valid
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        bool IsAuthenticated(Client client);

        /// <summary>
        /// Gets the string that uniquely identifies the client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        string GetClientString(Client client);
        /// <summary>
        /// Gets a client from a given string that uniquely identifies the client
        /// </summary>
        /// <param name="clientString"></param>
        /// <returns></returns>
        Client GetClient(string clientString);

        /// <summary>
        /// Gets the strings that uniquely identify the clients
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        IEnumerable<string> GetClientStrings(params Client[] clients);
        /// <summary>
        /// Gets the clients from the given strings that uniquely identify the clients
        /// </summary>
        /// <param name="clientStrings"></param>
        /// <returns></returns>
        IEnumerable<Client> GetClients(params string[] clientStrings);
    }
}
