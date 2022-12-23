using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ReadyPlayerMe
{
    public interface IAvatarImporter : IOperation<AvatarContext>
    {
        Task<GameObject> ImportModel(byte[] bytes, CancellationToken token);
        Task<GameObject> ImportModel(string path, CancellationToken token);
    }
}
