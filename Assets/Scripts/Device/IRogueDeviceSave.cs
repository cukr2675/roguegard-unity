using System.IO;

namespace Roguegard.Device
{
    public interface IRogueDeviceSave<T>
        where T : IRogueDevice
    {
        T NewGame();

        T LoadGame(Stream stream);
    }
}
