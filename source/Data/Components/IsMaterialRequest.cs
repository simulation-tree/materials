using Data.Components;
using Worlds;

namespace Materials.Components
{
    [Component]
    public struct IsMaterialRequest
    {
        public IsDataRequest request;

        public IsMaterialRequest(IsDataRequest request)
        {
            this.request = request;
        }
    }
}
