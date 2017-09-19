using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGameService
{
    public sealed class ChatServiceController
    {
        #region Variables
        private volatile static List<ChatMessage> chatQ;
        private static readonly object syncSender = new Object();
        private static readonly object syncReceive = new Object();
        private static readonly object syncReceiveAll = new Object();
        //singleton instance
        private static volatile ChatServiceController instance = null;
        private static readonly object syncRoot = new Object();

        #endregion
        public static ChatServiceController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ChatServiceController();
                    }
                }
                return instance;
            }
        }
        private ChatServiceController() 
        {
            if (chatQ != null)
                chatQ.Clear();
            chatQ = new List<ChatMessage>();
        }
        /// <summary>
        /// Any Msg Must be encrypted at client side
        /// and a Uniform Way Must Be Used.
        /// </summary>
        /// <param name="encryptedMsg">
        /// Client-Side Encrypted Msg Content</param>
        public void Send(ChatMessage encryptedMsg)
        {
            lock(syncSender)
            {
                chatQ.Add(encryptedMsg);
                return;
            }
        }
        /// <summary>
        /// Any Msg Must be decrypted at client side
        /// and a Uniform Way Must Be Used.
        /// </summary>
        /// <returns>last unseen encrypted Msg String</returns>
        public ChatMessage Receive(int LastReceivedIndex) 
        {
            lock (syncSender)
            {
                lock(syncReceive)
                {
                    /* Check that the Last Received Msg Index
                    is not already received or if there is
                    no new msgs to receive.*/
                    int toReceiveIndex = LastReceivedIndex + 1;
                    if (toReceiveIndex < chatQ.Count)
                    {
                        //send unseen msg to the client
                        return chatQ[toReceiveIndex];
                    }
                    else
                        return null;
                }
            }
        }
        /// <summary>
        /// Any Msg Must be decrypted at client side
        /// and a Uniform Way Must Be Used.
        /// </summary>
        /// <returns>List of all last unseen encrypted Msgs</returns>
        public List<ChatMessage> ReceiveAll(int LastReceivedIndex)
        {
            lock (syncSender)
            {
                lock (syncReceiveAll)
                {
                    /* Check that the Last Received Msg Index
                    is not already received or if there is
                    no new msgs to receive.*/
                    int toReceiveIndex = LastReceivedIndex + 1;
                    if (toReceiveIndex < chatQ.Count)
                    {
                        int toReceiveCount = chatQ.Count - toReceiveIndex;
                        //send all unseen msgs to the client
                        return chatQ.GetRange(toReceiveIndex, toReceiveCount);
                    }
                    else
                        return null;
                }
            }
        }
    }
}
