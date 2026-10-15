using System;
using System.Windows.Media;

namespace Launcher
{
	// Token: 0x0200002C RID: 44
	internal class SoundEffect
	{
		// Token: 0x06000113 RID: 275 RVA: 0x00006EA0 File Offset: 0x000050A0
		internal SoundEffect(MainWindow InOwner, int InMaxInstances, string InFileName, float InVolume)
		{
			this.Owner = InOwner;
			this.EffectInstances = new MediaPlayer[InMaxInstances];
			this.ResourceAddress = new Uri(string.Format("Audio/{0}.mp3", InFileName), UriKind.Relative);
			this.Volume = InVolume;
			this.CurrentInstance = 0;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00006EEC File Offset: 0x000050EC
		internal void Play()
		{
			if (!this.Owner.IsSfxMuted)
			{
				MediaPlayer effectInstance = this.GetEffectInstance();
				if (effectInstance != null)
				{
					effectInstance.Position = TimeSpan.Zero;
					effectInstance.Play();
				}
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00006F24 File Offset: 0x00005124
		private MediaPlayer GetEffectInstance()
		{
			MediaPlayer mediaPlayer = this.EffectInstances[this.CurrentInstance];
			if (mediaPlayer == null)
			{
				mediaPlayer = new MediaPlayer();
				mediaPlayer.Open(this.ResourceAddress);
				mediaPlayer.Volume = (double)this.Volume;
				this.EffectInstances[this.CurrentInstance] = mediaPlayer;
			}
			checked
			{
				this.CurrentInstance++;
				if (this.CurrentInstance >= this.EffectInstances.Length)
				{
					this.CurrentInstance = 0;
				}
				return mediaPlayer;
			}
		}

		// Token: 0x040000DB RID: 219
		private readonly MediaPlayer[] EffectInstances;

		// Token: 0x040000DC RID: 220
		private readonly Uri ResourceAddress;

		// Token: 0x040000DD RID: 221
		private readonly float Volume;

		// Token: 0x040000DE RID: 222
		private int CurrentInstance;

		// Token: 0x040000DF RID: 223
		private readonly MainWindow Owner;
	}
}
