using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using Turnip.Models;

namespace Turnip.Services
{
    internal static class ResourceAccessor
    {
        private static readonly RandomNumberGenerator rSecure = RandomNumberGenerator.Create();
        private static readonly Random r = new Random();

        public static Uri Get(string resourcePath)
        {
            var uri = string.Format(
                "pack://application:,,,/{0};component/{1}"
                , Assembly.GetExecutingAssembly().GetName().Name
                , resourcePath
            );

            return new Uri(uri);
        }

        public static BitmapImage GetImage(string resourcePath)
        {
            var imageUri = Get(resourcePath);
            return new BitmapImage(imageUri);
        }

        public static string GetRandomGameId()
        {
            var n = 8;
            var code = new char[9];
            var random = new byte[20];
            rSecure.GetBytes(random);
            foreach (var c in random)
            {
                if ((c > 'A' && c < 'Z') || (c > 'a' && c < 'z') || (c > '2' && c < '9'))
                {
                    code[n] = (char)c;
                    n--;
                }
                if (n < 0) break;
            }
            code[4] = '-';
            var gameId = new string(code);
            return gameId;
        }

        internal static string GetRandomNickName()
        {
            return $"{GenerateName(6)} {GenerateName(4)}";
        }

        public static string GenerateName(int len)
        {
            var consonants = new[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            var vowels = new[] { "a", "e", "i", "o", "u", "ae", "y" };
            var Name = new StringBuilder();
            Name.Append(consonants[r.Next(consonants.Length)].ToUpperInvariant());
            Name.Append(vowels[r.Next(vowels.Length)]);
            while (Name.Length < len)
            {
                Name.Append(consonants[r.Next(consonants.Length)]);
                Name.Append(vowels[r.Next(vowels.Length)]);
            }
            return Name.ToString();
        }
    }

    public interface INavigationService
    {
        void NavigateTo(string pageName);

        event EventHandler? Navigate;
    }

    public class PageNavigationService : INavigationService
    {
        public void NavigateTo(string pageName)
        {
            if (Navigate == null) return;

            Navigate.Invoke(this, new PageNavigationEventArgs(pageName));
        }

        public event EventHandler? Navigate;
    }
}