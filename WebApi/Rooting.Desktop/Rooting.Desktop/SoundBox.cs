using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rooting.Desktop
{
    internal class SoundBox
    {
        private readonly Dictionary<string, SoundEffect> SoundEffects = new();
        private readonly Dictionary<string, Song> BackgroundMusic = new();

        public SoundBox()
        {
            if (soundQueueTask == null) soundQueueTask = HandleQueue();
            if (blayBackgroundTask == null) blayBackgroundTask = PlayBackgroundMusic();
        }

        public void LoadContent(ContentManager contentManager)
        {
            BackgroundMusic.Clear();
            BackgroundMusic.Add("animals", contentManager.Load<Song>("audio\\23DB02TG_msc_animals"));
            BackgroundMusic.Add("allFamilies", contentManager.Load<Song>("audio\\23DB02TG_msc_allFamilies"));
            BackgroundMusic.Add("fight", contentManager.Load<Song>("audio\\23DB02TG_msc_fight"));
            BackgroundMusic.Add("funghi", contentManager.Load<Song>("audio\\23DB02TG_msc_funghi"));
            BackgroundMusic.Add("plants", contentManager.Load<Song>("audio\\23DB02TG_msc_plants"));

            SoundEffects.Clear();
            SoundEffects.Add("getCard", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_cardPickUp"));
            SoundEffects.Add("placeTile", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_placeTile"));
            SoundEffects.Add("hover", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UIHover"));
            SoundEffects.Add("exit", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UIExit"));
            SoundEffects.Add("select", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UISelect"));
            SoundEffects.Add("vocalCue", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_ultimateVocalCue"));

            Song = "allFamilies";
            Volume = 80;
        }

        private readonly ConcurrentQueue<string> sounds = new ConcurrentQueue<string>();

        private void QueueSound(string name)
        {
            sounds.Enqueue(name);
        }

        private readonly Task soundQueueTask;
        private readonly Task blayBackgroundTask;

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

        public int Volume { get; set; }
        public string Song { get; set; }

        private async Task PlayBackgroundMusic()
        {
            while (true)
            {
                if (Song != song)
                {
                    while (volume > 0)
                    {
                        volume = (volume - 0.1f);
                        if (volume < 0) { volume = 0; }
                        MediaPlayer.Volume = volume;
                        MediaPlayer.IsRepeating = true;
                        await Task.Delay(10);
                    }
                    song = Song;
                    MediaPlayer.Play(BackgroundMusic[song]);
                    MediaPlayer.IsRepeating = true;
                }

                if (Volume < volume)
                {
                    volume = volume - 0.1f;
                    MediaPlayer.Volume = volume;
                    MediaPlayer.IsRepeating = true;
                }
                if (Volume > volume)
                {
                    volume = volume + 0.1f;
                    MediaPlayer.Volume = volume;
                    MediaPlayer.IsRepeating = true;
                }
                await Task.Delay(10);
            }
        }

        private float volume;
        private string song;

        public void GetCard() => QueueSound("getCard");

        public void PlaceTile() => QueueSound("placeTile");

        public void Hover() => QueueSound("hover");

        public void ExitGame() => QueueSound("exit");

        public void SelectItem() => QueueSound("select");

        public void Vocal() => sounds.Enqueue("vocalCue");
    }
}