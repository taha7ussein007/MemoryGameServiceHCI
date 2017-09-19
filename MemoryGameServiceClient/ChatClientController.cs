using MemoryGameService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MemoryGameServiceClient
{
    public class ChatClientController
    {
        private static ProfanityFilter censor;
        public ChatClientController()
        {
            censor = new ProfanityFilter();
        }
        /// <summary>
        /// Check that connection established
        /// before send or receive any msg.
        /// </summary>
        /// <returns>False if connection lost 
        /// and true otherwise</returns>
        private bool checkConnection()
        {
            if (globalVars.proxy == null)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Cleans the content profanity, replacing it with stars.
        /// </summary>
        /// <param name="content">The content to clean profanity.</param>
        /// <returns>The content without profanity words.</returns>
        private string filter(string content)
        {
            string cleanMsg = null;
            try
            {
                if (censor.ValidateTextContainsProfanity(content))
                    cleanMsg = censor.CleanTextProfanity(content);
                else
                    cleanMsg = content;

                return cleanMsg;
            }
            catch(Exception)
            {
                return cleanMsg;
            }
        }
        /// <summary>
        /// Get the Msg and encrypt its content
        /// to ensure secure connection channel
        /// </summary>
        /// <param name="content">Msg Content</param>
        /// <returns>encrypted Msg Content or null if
        /// no content found</returns>
        private string encrypt(string content)
        {
            if (content.Length > 0)
                return StringCipher.Encrypt(content);
            else
                return null;
        }
        /// <summary>
        /// Get the encrypted Msg and decrypt its content
        /// </summary>
        /// <param name="content">Msg Content</param>
        /// <returns>decrypted Msg Content or null if
        /// no content found</returns>
        private string decrypt(string content)
        {
            if (content.Length > 0)
                return StringCipher.Decrypt(content);
            else
                return null;
        }
        /// <summary>
        /// Any Msg Must be encrypted at client side
        /// and a Uniform Way Must Be Used.
        /// </summary>
        /// <param name="Msg">
        /// Must be Encrypted And Filtered (optional)
        /// First of all.</param>
        public bool Send(ChatMessage Msg)
        {
            if(checkConnection())
            {
                Msg.MsgContent = encrypt(filter(Msg.MsgContent));
                if (Msg.MsgContent != null && Msg.MsgContent.Length > 0)
                {
                    globalVars.proxy.Send(Msg);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Any Msg Must be decrypted at client side
        /// and a Uniform Way Must Be Used.
        /// </summary>
        /// <returns>last unseen encrypted Msg String</returns>
        public ChatMessage Receive(int LastReceivedIndex) 
        {
            if (checkConnection())
            {
                ChatMessage Msg = globalVars.proxy.Receive(LastReceivedIndex);
                Msg.MsgContent = decrypt(Msg.MsgContent);
                if (Msg.MsgContent != null && Msg.MsgContent.Length > 0)
                {
                    return Msg;
                }
            }
            return null;
        }
        /// <summary>
        /// Any Msg Must be decrypted at client side
        /// and a Uniform Way Must Be Used.
        /// </summary>
        /// <returns>List of all last unseen encrypted Msgs</returns>
        public List<ChatMessage> ReceiveAll(int LastReceivedIndex)
        {
            if (checkConnection())
            {
                try
                {
                    List<ChatMessage> MsgsList = globalVars.proxy.ReceiveAll(LastReceivedIndex);
                    List<ChatMessage> FinalMsgsList = new List<ChatMessage>();
                    if (MsgsList != null)
                    {
                        foreach (var Msg in MsgsList)
                        {
                            Msg.MsgContent = decrypt(Msg.MsgContent);
                            if (Msg.MsgContent != null && Msg.MsgContent.Length > 0)
                                FinalMsgsList.Add(Msg);
                        }
                        if (FinalMsgsList.Count > 0)
                            return FinalMsgsList;
                    }
                }
                catch (Exception) { }
            }
            return null;
        }
    }
}
