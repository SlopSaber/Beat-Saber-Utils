using System.Linq;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;

namespace BS_Utils.Utilities
{
    internal static class UIUtilities
    {
        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Any())
            {
                Texture2D tex2D = new Texture2D(2, 2);
                if (tex2D.LoadImage(file))
                    return tex2D;
            }

            return null;
        }

        public static async Task<Texture2D> LoadTextureFromResourcesAsync(Assembly resourceAssembly, string resourcePath)
        {
            var bytes = await Task.Run(() => GetResource(resourceAssembly, resourcePath));
            await IPA.Utilities.UnityGame.SwitchToMainThreadAsync();
            return LoadTextureRaw(bytes);
        }

        public static byte[] GetResource(Assembly asm, string resourceName)
        {
            using Stream stream = asm.GetManifestResourceStream(resourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
    }
}
