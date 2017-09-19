using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MemoryGameService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
    [ServiceContract(Name = "IMemoryGameService")]
    public interface IMemoryGameService
    {

        #region Observer Operations

        [OperationContract(Name = "getNumOfAnswers")]
        int getNumOfAnswers();

        #endregion

        #region Game Operations
        // TODO: Add your service operations here
        [OperationContract(Name = "initializeGame")]
        void initializeGame();
        /*
        [OperationContract(Name = "login")]
        bool login(string UsernameOrEmail, string Password);
        */
        [OperationContract(Name = "openRoom")]
        int openRoom(gameRoom gr);

        [OperationContract(Name = "deleteRoom")]
        bool deleteRoom();
        /*
        [OperationContract(Name = "listAllRooms")]
        List<gameRoom> listAllRooms();
        */
        [OperationContract(Name = "joinRoom")]
        bool joinRoom(int roomID, int playerID, string playerIP);

        [OperationContract(Name = "IReady")]
        bool IReady(int playerID);

        [OperationContract(Name = "ISurrender")]
        bool ISurrender(int playerID);

        [OperationContract(Name = "canIStart")]
        bool canIStart();

        [OperationContract(Name = "IAnswered")]
        int IAnswered(int playerID);

        [OperationContract(Name = "checkGameState")]
        GAME_STATE checkGameState();

        [OperationContract(Name = "listRoomScores")]
        List<playerScore> listRoomScores();

        [OperationContract(Name = "getRdmLst")]
        List<int> getRdmLst();

        [OperationContract(Name = "getTheWinner")]
        string getTheWinner();
        #endregion

        #region Chat Operations
        [OperationContract(Name = "SendChatMsg")]
        void Send(ChatMessage encryptedMsg);

        [OperationContract(Name = "ReceiveChatMsg")]
        ChatMessage Receive(int LastReceivedIndex);

        [OperationContract(Name = "ReceiveAllChatMsgs")]
        List<ChatMessage> ReceiveAll(int LastReceivedIndex);
        #endregion
    }

    public interface IGameWindowCallBack
    {
        [OperationContract(Name = "notify_updateScores")]
        void notify_updateScores();

        [OperationContract(Name = "notify_goToNxtLvl")]
        void notify_goToNxtLvl();

        [OperationContract(Name = "notify_gameFinished")]
        void notify_gameFinished();
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "MemoryGameService.ContractType".

    [DataContract(Name = "playerScore")]
    public class playerScore
    {
        [DataMember(Name = "playername")]
        public string playername { get; set; }

        [DataMember(Name = "is_ready")]
        public string is_ready { get; set; }

        [DataMember(Name = "score")]
        public string score { get; set; }
    }

    [DataContract(Name = "gameRoom")]
    public class gameRoom
    {
        public gameRoom() { }
        public gameRoom(int roomId)
        {
            DBController dbc = DBController.Instance;
            string query = "SELECT `id`, 'roomName', `level`, `numOfPlays`, `max_players`, `host_id`, `host_ip`, `game_state`, `hostName` FROM (SELECT `id`, `name` AS 'roomName', `level`, `numOfPlays`, `max_players`, `host_id`, `host_ip`, `game_state` FROM game_room WHERE id='" + roomId + "') AS t1 JOIN (SELECT id AS 'hostId', name AS 'hostName' FROM user) AS t2 ON t1.host_id = t2.hostId";
            var roomsReader = dbc.retreiveDate(query);
            if (roomsReader != null)
            {
                if (roomsReader.Read())
                {
                    this.id = roomsReader.GetInt32("id");
                    this.name = roomsReader.GetString("roomName");
                    this.level = roomsReader.GetInt32("level");
                    this.maxPlayers = roomsReader.GetInt32("max_players");
                    this.host_ID = roomsReader.GetInt32("host_id");
                    this.host_IP = roomsReader.GetString("host_ip");
                    this.gameState = (GAME_STATE)roomsReader["game_state"];
                    this.hostName = roomsReader.GetString("hostName");
                    this.numOfPlays = roomsReader.GetInt32("numOfPlays");
                    roomsReader.Close();
                    this.numOfPlayers = dbc.countRows("room_players", "player_id", "room_id", this.id.ToString());
                }
            }
        }

        [DataMember(Name = "id")]
        public int id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "level")]
        public int level { get; set; }

        [DataMember(Name = "maxPlayers")]
        public int maxPlayers { get; set; }

        [DataMember(Name = "host_ID")]
        public int host_ID { get; set; }

        [DataMember(Name = "hostName")]
        public string hostName { get; set; }

        [DataMember(Name = "host_IP")]
        public string host_IP { get; set; }

        [DataMember(Name = "numOfPlayers")]
        public int numOfPlayers { get; set; }

        [DataMember(Name = "numOfPlays")]
        public int numOfPlays { get; set; }

        [DataMember(Name = "gameState")]
        public GAME_STATE gameState { get; set; }
    }

    [DataContract(Name = "GAME_STATE")]
    public enum GAME_STATE
    {
        [EnumMemberAttribute]
        Wait = 1,
        [EnumMemberAttribute]
        Cancelled,
        [EnumMemberAttribute]
        Ready,
        [EnumMemberAttribute]
        Started,
        [EnumMemberAttribute]
        Paused,
        [EnumMemberAttribute]
        Aborted,
        [EnumMemberAttribute]
        Finished
    }

    [DataContract(Name = "ChatMessage")]
    public class ChatMessage
    {
        [DataMember(Name = "senderName")]
        public string senderName { get; set; }

        [DataMember(Name = "MsgContent")]
        public string MsgContent { get; set; }
    }

    [DataContract(Name = "GamePlayer")]
    public class GamePlayer
    {
        public GamePlayer() { }
        public GamePlayer(string UsernameOrEmail)
        {
            DBController dbc = DBController.Instance;
            string query = "SELECT * FROM `user` WHERE username='@' OR email='@'";
            query = query.Replace("@", UsernameOrEmail);
            var PlayerReader = dbc.retreiveDate(query);
            if (PlayerReader != null)
            {
                if (PlayerReader.Read())
                {
                    this.id = PlayerReader.GetInt32("id");
                    this.name = PlayerReader.GetString("name");
                    this.username = PlayerReader.GetString("username");
                    this.email = PlayerReader.GetString("email");
                    this.mobile = PlayerReader.GetString("mobile");
                    this.type = PlayerReader.GetInt32("type");
                    this.active = PlayerReader.GetBoolean("active");
                    this.premium = PlayerReader.GetBoolean("premium");
                    this.online = PlayerReader.GetBoolean("online");
                    this.subscribed = PlayerReader.GetBoolean("subscribed");
                    this.profile_link = Convert.ToString(PlayerReader["profile_link"]);
                    PlayerReader.Close();
                }
                PlayerReader.Close();
            }
        }

        [DataMember(Name = "id")]
        public int id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "username")]
        public string username { get; set; }

        [DataMember(Name = "email")]
        public string email { get; set; }

        [DataMember(Name = "mobile")]
        public string mobile { get; set; }

        [DataMember(Name = "type")]
        public int type { get; set; }

        [DataMember(Name = "active")]
        public bool active { get; set; }

        [DataMember(Name = "premium")]
        public bool premium { get; set; }

        [DataMember(Name = "online")]
        public bool online { get; set; }

        [DataMember(Name = "subscribed")]
        public bool subscribed { get; set; }

        [DataMember(Name = "profile_link")]
        public string profile_link { get; set; }
    }
}
