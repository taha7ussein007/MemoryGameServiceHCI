using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGameService
{
    public class GameController
    {
        #region variables
        private DBController dbc = DBController.Instance;
        private static bool acceptAnswer = true;
        private static volatile gameRoom _gameRoom;
        private static volatile List<int> rdmLst = new List<int>();
        private static volatile Random rdm = new Random();
        private static volatile int winnerID = 0;
        //sync locks
        private static readonly object joinRoom_syncLock = new object();
        private static readonly object canIStart_syncLock = new object();
        private static readonly object IAnswered_syncLock = new object();
        private static readonly object generateRdmLst_syncLock = new object();
        private static readonly object ISurrender_syncLock = new object();
        #endregion
        public GameController()
        {

        }
        //reset all to default must be called at the host
        public void initializeGame()
        {
            generateRdmLst();
        }
        public bool login(string UsernameOrEmail, string Password)
        {
            string sql = "SELECT password FROM user WHERE username='@' OR email='@'";
            sql = sql.Replace("@", UsernameOrEmail);
            var reader = dbc.retreiveDate(sql);
            if(reader != null)
            {
                if(reader.Read())
                {
                    if (reader["password"].ToString() == Password)
                    {
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
                reader.Close();
            }
            return false;
        }
        public int openRoom(gameRoom gr)
        {
            try 
            {
                deleteOldHostRoomsIfAny(gr.host_ID);
            }
            catch (Exception) { }

            string table = "game_room";
            string columns = "name, level, numOfPlays, max_players, host_id, host_ip, game_state";
            string values = "'" + gr.name + "', '"
                + gr.level.ToString() + "', '"
                + gr.numOfPlays.ToString() + "', '"
                + gr.maxPlayers.ToString() + "', '"
                + gr.host_ID.ToString() + "', '"
                + gr.host_IP + "', '"
                + ((int)gr.gameState).ToString() + "'";
            if (dbc.add(table, columns, values))
            {
                //get room id
                string query = "SELECT id FROM game_room WHERE host_id='" + gr.host_ID.ToString() + "'";
                var reader = dbc.retreiveDate(query);
                if (reader != null)
                {
                    reader.Read();
                    int roomID = reader.GetInt32("id");
                    reader.Close();
                    _gameRoom = new gameRoom(roomID);
                    return roomID;
                }
            }
            return 0;
        }
        private void deleteOldHostRoomsIfAny(int HostId)
        {
            dbc.delete("game_room", "host_id", HostId.ToString());
        }
        public bool deleteRoom(int id = 0) 
        {
            if (id == 0)
                return dbc.delete("game_room", "id", _gameRoom.id.ToString());
            else
                return dbc.delete("game_room", "id", id.ToString());
        }
        public List<gameRoom> listAllRooms()
        {
            List<gameRoom> allGameRooms = new List<gameRoom>();
            string query = "SELECT `id`, `roomName`, `level`, `numOfPlays`, `max_players`, `host_id`, `host_ip`, `game_state`, `hostName` FROM (SELECT `id`, `name` AS 'roomName', `level`, `numOfPlays`, `max_players`, `host_id`, `host_ip`, `game_state` FROM game_room) AS t1 JOIN (SELECT id AS 'hostId', name AS 'hostName' FROM user) AS t2 ON t1.host_id = t2.hostId";
            if (dbc == null)
                dbc = DBController.Instance;
            var roomsReader = dbc.retreiveDate(query);
            if (roomsReader != null)
            {
                while (roomsReader.Read())
                {
                    gameRoom gr = new gameRoom();
                    gr.id = roomsReader.GetInt32("id");
                    gr.name = roomsReader.GetString("roomName");
                    gr.level = roomsReader.GetInt32("level");
                    gr.maxPlayers = roomsReader.GetInt32("max_players");
                    gr.host_ID = roomsReader.GetInt32("host_id");
                    gr.host_IP = roomsReader.GetString("host_ip");
                    gr.gameState = (GAME_STATE)roomsReader["game_state"];
                    gr.hostName = roomsReader.GetString("hostName");
                    gr.numOfPlays = roomsReader.GetInt32("numOfPlays");
                    gr.numOfPlayers = dbc.countRows("room_players", "player_id", "room_id", gr.id.ToString());
                    allGameRooms.Add(gr);
                }
                roomsReader.Close();
            }
            return allGameRooms;
        }
        public bool joinRoom(int roomID, int playerID, string playerIP)
        {
            //if state wait or ready and maxplayers not reached
            lock (ISurrender_syncLock)
            {
                lock (joinRoom_syncLock)
                {
                    _gameRoom = new gameRoom(roomID);
                    if (_gameRoom.numOfPlayers < _gameRoom.maxPlayers &&
                        (_gameRoom.gameState == GAME_STATE.Wait || _gameRoom.gameState == GAME_STATE.Ready))
                    {
                        if (dbc.add("room_players", "player_id, player_ip, room_id", "'" + playerID.ToString() + "', '" + playerIP + "', '" + roomID.ToString() + "'"))
                        {
                            _gameRoom.numOfPlayers++;
                            return true;
                        }
                    }
                    return false;
                }
            }
        }
        public bool IReady(int playerID)
        {
            string query = "UPDATE room_players SET is_ready='1' WHERE player_id='PID' AND room_id='RID'";
            query = query.Replace("PID", playerID.ToString()).Replace("RID", _gameRoom.id.ToString());
            return dbc.update(query);
        }
        public bool ISurrender(int playerID)
        {
            string query = "DELETE FROM room_players WHERE player_id='PID' AND room_id='RID'";
            query = query.Replace("PID", playerID.ToString()).Replace("RID", _gameRoom.id.ToString());
            lock (ISurrender_syncLock)
            {
                if (dbc.delete(query))
                {
                    _gameRoom.numOfPlayers--;
                    return true;
                }
                return false;
            }
        }
        public bool canIStart()
        {
            lock (ISurrender_syncLock)
            {
                lock (canIStart_syncLock)
                {
                    if (_gameRoom.numOfPlayers > 1)
                    {
                        //Assume that's all are ready
                        _gameRoom.gameState = GAME_STATE.Ready;
                        string query = "SELECT is_ready FROM room_players WHERE room_id='" + _gameRoom.id.ToString() + "'";
                        var reader = dbc.retreiveDate(query);
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                //check for previous assumption
                                if (Convert.ToBoolean(reader["is_ready"]) == false)
                                    _gameRoom.gameState = GAME_STATE.Wait;
                            }
                            reader.Close();
                            if (_gameRoom.gameState == GAME_STATE.Ready)
                            {
                                //indicates that the game has been already started
                                _gameRoom.gameState = GAME_STATE.Started;
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
        }
        /// <summary>
        /// Pre-Condition: The answer should be checked locally first
        /// to ensure that is a correct answer.
        /// </summary>
        /// <returns>true if answer accepted and false otherwise</returns>
        public int IAnswered(int playerID)
        {
            lock (IAnswered_syncLock)
            {
                if (acceptAnswer && winnerID == 0)
                {
                    acceptAnswer = false;
                    try
                    {
                        int newScore = getPlayerScore(playerID) + 1;
                        setPlayerScore(playerID, newScore);
                        if (newScore < _gameRoom.numOfPlays) //the game is over when player score = numOfPlays
                        {
                            generateRdmLst(); //generate list for the next play
                            acceptAnswer = true;
                            return 1; //notify that answer accepted :)
                        }
                        else
                        {
                            _gameRoom.gameState = GAME_STATE.Finished;
                            winnerID = playerID;
                            return -1; //(الفورة خلصت)
                        }
                    }
                    catch(Exception)
                    {
                        acceptAnswer = true;
                        return 0; //connection lost or surrender
                    }
                }
                if (winnerID != 0)
                    return -1; //(الفورة خلصت)
                else
                    return 0; //late correct answer
            }
        }
        /// <summary>
        /// Must be called after every play
        /// </summary>
        /// <returns>Game State</returns>
        public GAME_STATE checkGameState()
        {
            return _gameRoom.gameState;
        }
        private int getPlayerScore(int playerID)
        {
            string query = "SELECT score FROM room_players WHERE player_id='PID' AND room_id='RID'";
            query = query.Replace("PID", playerID.ToString()).Replace("RID", _gameRoom.id.ToString());
            var reader = dbc.retreiveDate(query);
            reader.Read();
            int score = Convert.ToInt32(reader["score"]);
            reader.Close();
            return score;
        }
        private void setPlayerScore(int playerID, int newScore)
        {
            string query = "UPDATE room_players SET score='NEWSCORE' WHERE player_id='PID' AND room_id='RID'";
            query = query.Replace("NEWSCORE", newScore.ToString()).Replace("PID", playerID.ToString()).Replace("RID", _gameRoom.id.ToString());
            dbc.update(query);
        }
        public List<playerScore> listRoomScores(int RoomID = 0)
        {
            string query = "SELECT playerName, is_ready, score FROM (SELECT player_id, is_ready, score FROM room_players WHERE room_id='RID') AS t1 JOIN (SELECT id, username AS 'playerName' FROM user) AS t2 ON t1.player_id=t2.id";
            if (RoomID == 0)
                query = query.Replace("RID", _gameRoom.id.ToString());
            else
                query = query.Replace("RID", RoomID.ToString());
            List<playerScore> pscrList = new List<playerScore>();
            var scoresReader = dbc.retreiveDate(query);
            if (scoresReader != null)
            {
                while (scoresReader.Read())
                {
                    playerScore pscr = new playerScore();
                    pscr.playername = scoresReader["playerName"].ToString();
                    if (Convert.ToBoolean(scoresReader["is_ready"]))
                        pscr.is_ready = "Ready";
                    else
                        pscr.is_ready = "Not Ready";
                    pscr.score = scoresReader["score"].ToString();
                    pscrList.Add(pscr);
                }
                scoresReader.Close();
                return pscrList;
            }
            return null;
        }
        private void generateRdmLst()
        {
            rdmLst.Clear();
            for (int i = 0; i < _gameRoom.level; i++)
            {
                int randomValue;
                lock (generateRdmLst_syncLock)
                {
                    randomValue = rdm.Next(0, 16);
                }
                if (!rdmLst.Contains(randomValue))
                {
                    rdmLst.Add(randomValue);
                }
                else
                {
                    i--;
                    continue;
                }
            }
            return;
        }
        public List<int> getRdmLst()
        {
            return rdmLst;
        }
        public string getTheWinner()
        {
            string query = "SELECT name FROM user WHERE id='"+winnerID+"'";
            var reader = dbc.retreiveDate(query);
            if(reader != null)
            {
                if (reader.Read()) 
                {
                    string winnerName = reader["name"].ToString();
                    reader.Close();
                    return winnerName;
                }
            }
            return "ERROR 404 Winner Not Found xD";
        }
    }
}