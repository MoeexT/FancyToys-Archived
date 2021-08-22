
namespace FancyServer {

    internal interface IManager {
        void Deal(string content);
        void Send(object sdu);
    }

}