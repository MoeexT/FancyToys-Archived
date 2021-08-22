
namespace FancyServer.Logging {

    internal struct DialogStruct {
        public string title;
        public string content;
    }

    internal class DialogClerk {
        public static void Dialog(string title, string message) {
            LoggingManager.Send(new DialogStruct {
                title = title,
                content = message,
            });
        }
    }

}