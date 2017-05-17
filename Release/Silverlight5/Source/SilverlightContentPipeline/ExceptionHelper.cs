using Microsoft.Xna.Framework.Content.Pipeline;
using System;

namespace SilverlightContentPipeline
{
    internal static class ExceptionHelper
    {
        public static string Filename { get; set; }

        public static void RaiseException(string message, Exception innerException = null)
        {
            throw new InvalidContentException(message, new ContentIdentity(Filename), innerException);
        }
    }
}
