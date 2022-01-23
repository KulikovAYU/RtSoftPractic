using System.Net.Sockets;
using System.Text;

namespace ServerApp.Core
{
    public class ObjectState
    {
        public const int BufferSize = 1024;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

        public Socket workSocket = null;
    }
}