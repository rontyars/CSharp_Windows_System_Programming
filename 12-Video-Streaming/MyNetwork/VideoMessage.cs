using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetwork
{
    public class VideoMessage: Message
    {
        public sealed override string MessageHeader
        {
            get
            {
                return "[VIDEO]";
            }
            set
            {
                _messageHeader = value;
            }
        }
    }
}
