using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MemoryGameService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MemoryGameService" in both code and config file together.
    public class MemoryGameService : IMemoryGameService
    {
        #region Variables
        private static GameController gc = new GameController();
        private static ChatServiceController csc;
        public static int numOfAnswers;
        #endregion
        public MemoryGameService()
        {
            csc = ChatServiceController.Instance;
        }

        #region Observer Operations
        public int getNumOfAnswers()
        {
            return numOfAnswers;
        }

        #endregion

        #region Game Operations
        public void initializeGame()
        {
            gc.initializeGame();
        }
        public int openRoom(gameRoom gr)
        {
            return gc.openRoom(gr);
        }
        public bool deleteRoom()
        {
            return gc.deleteRoom();
        }
        public bool joinRoom(int roomID, int playerID, string playerIP)
        {
            return gc.joinRoom(roomID, playerID, playerIP);
        }
        public bool IReady(int playerID)
        {
            return gc.IReady(playerID);
        }
        public bool ISurrender(int playerID)
        {
            return gc.ISurrender(playerID);
        }
        public bool canIStart()
        {
            return gc.canIStart();
        }
        public int IAnswered(int playerID)
        {
            switch(gc.IAnswered(playerID))
            {
                case 1: //Answer accepted
                    numOfAnswers++;
                    return 1;
                case 0: //Late Answer
                    return 0;
                case -1: //GameOver
                    numOfAnswers++;
                    return -1;
                default: //Error
                    return IAnswered(playerID);
            }
        }
        public GAME_STATE checkGameState()
        {
            return gc.checkGameState();
        }
        public List<playerScore> listRoomScores()
        {
            return gc.listRoomScores();
        }
        public List<int> getRdmLst()
        {
            return gc.getRdmLst();
        }
        public string getTheWinner()
        {
            return gc.getTheWinner();
        }
        #endregion

        #region Chat Operations
        public void Send(ChatMessage encryptedMsg)
        {
            csc.Send(encryptedMsg);
        }
        public ChatMessage Receive(int LastReceivedIndex)
        {
            return csc.Receive(LastReceivedIndex);
        }
        public List<ChatMessage> ReceiveAll(int LastReceivedIndex)
        {
            return csc.ReceiveAll(LastReceivedIndex);
        }
        #endregion
    }
}
