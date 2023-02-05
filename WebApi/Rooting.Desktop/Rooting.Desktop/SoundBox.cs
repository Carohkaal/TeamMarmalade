using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rooting.Desktop
{
    internal class SoundBox
    {
        private readonly Dictionary<string, SoundEffect> SoundEffects = new();
        private readonly Dictionary<string, Song> BackgroundMusic = new();

        public SoundBox()
        {
        }

        public void LoadContent(ContentManager contentManager)
        {
            BackgroundMusic.Clear();
            BackgroundMusic.Add("animals", contentManager.Load<Song>("audio\\23DB02TG_msc_animals"));
            BackgroundMusic.Add("basis", contentManager.Load<Song>("audio\\23DB02TG_msc_basis"));
            BackgroundMusic.Add("fight", contentManager.Load<Song>("audio\\23DB02TG_msc_fight"));
            BackgroundMusic.Add("funghi", contentManager.Load<Song>("audio\\23DB02TG_msc_funghi"));
            BackgroundMusic.Add("plant", contentManager.Load<Song>("audio\\23DB02TG_msc_plant"));

            MediaPlayer.Play(BackgroundMusic["fight"]);
            Thread.Sleep(2000);
            MediaPlayer.Play(BackgroundMusic["animals"]);

            SoundEffects.Clear();
            SoundEffects.Add("getCard", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_cardPickUp"));
            SoundEffects.Add("placeTile", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_placeTile"));
            SoundEffects.Add("hover", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UIHover"));
            SoundEffects.Add("exit", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UIExit"));
            SoundEffects.Add("select", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UISelect"));
            SoundEffects.Add("vocalCue", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_ultimateVocalCue"));

            QueueSound("vocalCue");
        }

        private readonly ConcurrentQueue<string> sounds = new ConcurrentQueue<string>();

        private void QueueSound(string name)
        {
            sounds.Enqueue(name);
            if (SoundQueueTask == null) SoundQueueTask = HandleQueue();
        }

        private Task SoundQueueTask;

        public void Mixer(int basic, int animals, int funghi, int plant, int fight)
        {
        }

        private async Task HandleQueue()
        {
            while (true)
            {
                if (sounds.TryDequeue(out var name))
                {
                    var soundEffectInstance = SoundEffects[name].CreateInstance();
                    soundEffectInstance.Play();
                    await Task.Delay(20);
                    while (soundEffectInstance.State != SoundState.Stopped) { await Task.Delay(10); }
                }
                await Task.Delay(10);
            }
        }

        public void GetCard() => QueueSound("getCard");

        public void PlaceTile() => QueueSound("placeTile");

        public void Hover() => QueueSound("hover");

        public void ExitGame() => QueueSound("exit");

        public void SelectItem() => QueueSound("select");

        public void Vocal() => sounds.Enqueue("vocalCue");
    }
}