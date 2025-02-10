using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ComAssistant;

internal class GlobalCache
{
    private GlobalCache()
    {
    }

    public static GlobalCache Instance { get; } = new();

    public Parity[] ParitySource => new[] { Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space };

    public StopBits[] StopBitsSource => new[] { StopBits.None, StopBits.One, StopBits.Two, StopBits.OnePointFive };

    public int[] BaudRates { get; } = { 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
    public int[] DataBitsArray { get; } = { 8, 9 };
    public Encoding[] Encodings { get; } = Encoding.GetEncodings().Select(x => x.GetEncoding()).ToArray();
}