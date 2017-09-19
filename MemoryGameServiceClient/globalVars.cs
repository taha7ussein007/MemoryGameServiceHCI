using MemoryGameService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGameServiceClient
{
    public static class globalVars
    {
        public static bool offlineGame { get; set; }
        public static string userName_mainWindow { get; set; }
        public static GamePlayer player { get; set; }
        public static string userIp { get; set; }
        public static int roomId { get; set; }
        public static IMemoryGameService proxy { get; set; }

        public static ChatClientController onlineChatClientController =
            new ChatClientController();

        #region offline Instances
        public static IMemoryGameService offlineProxy = 
            new MemoryGameService.MemoryGameService();
        public static DBController offlineDBController =
            MemoryGameService.DBController.Instance;
        public static GameController offlineGameController =
            new MemoryGameService.GameController();
        #endregion

    }
}
