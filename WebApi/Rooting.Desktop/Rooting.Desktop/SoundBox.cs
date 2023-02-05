using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rooting.Desktop
{
    internal class SoundBox
    {
        private readonly Dictionary<string, SoundEffect> soundEffects = new();

        public SoundBox()
        {
        }

        public void LoadContent(ContentManager contentManager)
        {
            soundEffects.Clear();
            soundEffects.Add("getCard", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_cardPickUp"));
            soundEffects.Add("placeTile", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_placeTile"));
            soundEffects.Add("hover", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UIHover"));
            soundEffects.Add("exit", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UIExit"));
            soundEffects.Add("select", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_UISelect"));
            soundEffects.Add("vocalCue", contentManager.Load<SoundEffect>("audio\\23DB02TG_sfx_ultimateVocalCue"));

            QueueSound("vocalCue");
        }

        private readonly ConcurrentQueue<string> sounds = new ConcurrentQueue<string>();

        private void QueueSound(string name)
        {
            sounds.Enqueue(name);
            if (SoundQueueTask == null) SoundQueueTask = HandleQueue();
        }

        private Task SoundQueueTask;

        private async Task HandleQueue()
        {
            while (true)
            {
                if (sounds.TryDequeue(out var name))
                {
                    var soundEffectInstance = soundEffects[name].CreateInstance();
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