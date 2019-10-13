using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace External.Types
{
    public class CardEventArgs
    {
        public string CardNumber { get; set; }
    }

    public class Card
    {
        public string CardNumber { get; set; }
    }

    public interface ICardReader
    {
        Action<object, CardEventArgs> CardInserted { get; set; }
        Action<object, CardEventArgs> CardRemoved { get; set; }

        Card GetCardOnScanner();
    }
}
