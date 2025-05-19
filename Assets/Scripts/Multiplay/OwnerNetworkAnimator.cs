using System;
using Unity.Netcode.Components;

namespace Multiplay
{
    [Obsolete("This class is deprecated. Use NetworkAnimator instead.")]
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}