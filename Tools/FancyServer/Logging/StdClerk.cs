
namespace FancyServer.Logging {

    public enum StdType {
        //Input = 0,
        Output = 1,
        Error = 2,
    }

    public struct StdStruct {
        public StdType type;
        public string process;
        public string content;
    }

    public static class StdClerk {
        public static StdType StdLevel { get; set; } = StdType.Error;

        public static void StdOutput(string process, string message) {
            LoggingManager.Send(new StdStruct {
                type = StdType.Output,
                process = process,
                content = message,
            });
        }

        public static void StdError(string process, string message) {
            LoggingManager.Send(new StdStruct {
                type = StdType.Error,
                process = process,
                content = message,
            });
        }
    }

}