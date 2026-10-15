using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Launcher
{
	// Token: 0x0200000A RID: 10
	internal class GamepadHandler
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000042 RID: 66 RVA: 0x00004138 File Offset: 0x00002338
		// (remove) Token: 0x06000043 RID: 67 RVA: 0x00004170 File Offset: 0x00002370
		public event GamepadHandler.OnFocusedButtonChanged NewButtonFocusedEvents;

		// Token: 0x06000044 RID: 68 RVA: 0x000041A8 File Offset: 0x000023A8
		public GamepadHandler(Grid focusedGrid)
		{
			this.buttonList = new List<Button>();
			this.PressedButtons = new List<KeyValuePair<PlayerIndex, List<Buttons>>>();
			this.PressedKeys = new List<Keys>();
			this.SupportedGamepadButtons = new Dictionary<Buttons, List<Buttons>>();
			this.InitSupportedGamepadButtons();
			this.UpdateFocusedGrid(focusedGrid);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000041F4 File Offset: 0x000023F4
		private void InitSupportedGamepadButtons()
		{
			this.SupportedGamepadButtons.Add(Buttons.LeftThumbstickDown, new List<Buttons> { Buttons.DPadDown });
			this.SupportedGamepadButtons.Add(Buttons.LeftThumbstickUp, new List<Buttons> { Buttons.DPadUp });
			this.SupportedGamepadButtons.Add(Buttons.A, new List<Buttons> { Buttons.Start });
			this.SupportedGamepadButtons.Add(Buttons.B, new List<Buttons> { Buttons.Back });
			this.SupportedGamepadButtons.Add(Buttons.LeftThumbstickRight, new List<Buttons> { Buttons.DPadRight });
			this.SupportedGamepadButtons.Add(Buttons.LeftThumbstickLeft, new List<Buttons> { Buttons.DPadLeft });
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000042AC File Offset: 0x000024AC
		public void UpdateFocusedGrid(Grid focusedGrid)
		{
			if (this.lastKnownFocusedGrid == null)
			{
				this.lastKnownFocusedGrid = focusedGrid;
			}
			Grid grid = this.lastKnownFocusedGrid;
			lock (grid)
			{
				this.lastKnownFocusedGrid = focusedGrid;
				this.bUpdatedFocusedGrid = true;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004304 File Offset: 0x00002504
		public void TickGamepads(object StateInfo)
		{
			bool flag = false;
			using (IEnumerator enumerator = Enum.GetValues(typeof(PlayerIndex)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					this.PressedButtons.Add(new KeyValuePair<PlayerIndex, List<Buttons>>((PlayerIndex)obj, new List<Buttons>()));
				}
				goto IL_00E3;
			}
			IL_0059:
			Grid grid = this.lastKnownFocusedGrid;
			lock (grid)
			{
				if (this.bUpdatedFocusedGrid)
				{
					this.buttonList.Clear();
					this.lastKnownFocusedGrid.Dispatcher.BeginInvoke(new Action<List<Button>, bool>(delegate(List<Button> list, bool bFocusFirstItem)
					{
						MainWindow.UpdateFocusableButtonList(list, this.lastKnownFocusedGrid);
						if (bFocusFirstItem)
						{
							this.ProcessGamepadButton(Buttons.LeftThumbstickDown, PlayerIndex.One, ref this.focusedButton, ref list);
						}
					}), new object[] { this.buttonList, this.bUsingGamepad });
					this.bUpdatedFocusedGrid = false;
				}
			}
			if (App.ApplicationIsActivated())
			{
				this.CheckForGamepadInput();
			}
			Thread.Sleep(50);
			IL_00E3:
			if (flag)
			{
				return;
			}
			goto IL_0059;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004418 File Offset: 0x00002618
		private void CheckForGamepadInput()
		{
			foreach (object obj in Enum.GetValues(typeof(PlayerIndex)))
			{
				PlayerIndex PlayerIdx = (PlayerIndex)obj;
				GamePadState state = GamePad.GetState(PlayerIdx);
				List<Buttons> value = this.PressedButtons.Find((KeyValuePair<PlayerIndex, List<Buttons>> x) => x.Key == PlayerIdx).Value;
				foreach (KeyValuePair<Buttons, List<Buttons>> keyValuePair in this.SupportedGamepadButtons)
				{
					bool flag = state.IsButtonDown(keyValuePair.Key);
					if (keyValuePair.Value != null && !flag)
					{
						foreach (Buttons buttons in keyValuePair.Value)
						{
							if (state.IsButtonDown(buttons))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						if (!value.Contains(keyValuePair.Key))
						{
							this.ProcessGamepadButton(keyValuePair.Key, PlayerIdx, ref this.focusedButton, ref this.buttonList);
							value.Add(keyValuePair.Key);
						}
					}
					else
					{
						value.Remove(keyValuePair.Key);
					}
				}
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000045DC File Offset: 0x000027DC
		private void ProcessGamepadButton(Buttons buttons, PlayerIndex player, ref Button focusedButton, ref List<Button> buttonList)
		{
			this.bUsingGamepad = true;
			bool flag = false;
			if (buttons <= Buttons.LeftThumbstickLeft)
			{
				if (buttons == Buttons.A)
				{
					this.PressFocusedButton(ref focusedButton);
					return;
				}
				if (buttons != Buttons.B)
				{
					if (buttons != Buttons.LeftThumbstickLeft)
					{
						return;
					}
					if (focusedButton != null)
					{
						Func<Button, bool> func = delegate(Button x)
						{
							if (x != null && x.DataContext != null && x.DataContext is Setting)
							{
								(x.DataContext as Setting).PreviousSettingValue(false);
								return true;
							}
							return false;
						};
						flag = (bool)focusedButton.Dispatcher.Invoke(func, new object[] { focusedButton });
					}
					if (!flag)
					{
						this.FocusPrevButton(ref focusedButton, ref buttonList);
						return;
					}
				}
				else
				{
					this.PressCancelButton(ref buttonList);
				}
			}
			else
			{
				if (buttons == Buttons.LeftThumbstickUp)
				{
					this.FocusPrevButton(ref focusedButton, ref buttonList);
					return;
				}
				if (buttons == Buttons.LeftThumbstickDown)
				{
					this.FocusNextButton(ref focusedButton, ref buttonList);
					return;
				}
				if (buttons == Buttons.LeftThumbstickRight)
				{
					if (focusedButton != null)
					{
						Func<Button, bool> func2 = delegate(Button x)
						{
							if (x != null && x.DataContext != null && x.DataContext is Setting)
							{
								(x.DataContext as Setting).NextSettingValue(false);
								return true;
							}
							return false;
						};
						flag = (bool)focusedButton.Dispatcher.Invoke(func2, new object[] { focusedButton });
					}
					if (!flag)
					{
						this.FocusNextButton(ref focusedButton, ref buttonList);
						return;
					}
				}
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000225E File Offset: 0x0000045E
		private void PressCancelButton(ref List<Button> buttonList)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action<List<Button>>(delegate(List<Button> list)
			{
				foreach (Button button in list)
				{
					if (button.IsCancel)
					{
						button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
						break;
					}
				}
			}), new object[] { buttonList });
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000046FC File Offset: 0x000028FC
		private void PressFocusedButton(ref Button focusedButton)
		{
			if (focusedButton != null)
			{
				Func<Button, bool> func = (Button x) => x.DataContext == null || !(x.DataContext is Setting);
				bool flag = (bool)Application.Current.Dispatcher.Invoke(func, new object[] { focusedButton });
				Application.Current.Dispatcher.BeginInvoke(new Action<Button, bool>(delegate(Button btn, bool dropFocus)
				{
					if (dropFocus)
					{
						btn.RaiseEvent(new MouseEventArgs(global::System.Windows.Input.Mouse.PrimaryDevice, 0)
						{
							RoutedEvent = global::System.Windows.Input.Mouse.MouseLeaveEvent
						});
					}
					btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
				}), new object[] { focusedButton, flag });
				if (flag)
				{
					focusedButton = null;
				}
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000479C File Offset: 0x0000299C
		private void FocusPrevButton(ref Button focusedButton, ref List<Button> buttonList)
		{
			checked
			{
				int num = buttonList.Count - 1;
				if (focusedButton != null)
				{
					num = buttonList.IndexOf(focusedButton) - 1;
					if (num < 0)
					{
						num = buttonList.Count - 1;
					}
					if (buttonList[num].ActualHeight < 1.0)
					{
						num--;
						if (num < 0)
						{
							num = buttonList.Count - 1;
						}
					}
				}
				this.ChangeGamepadFocusTo(buttonList[num], ref focusedButton);
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000480C File Offset: 0x00002A0C
		private void FocusNextButton(ref Button focusedButton, ref List<Button> buttonList)
		{
			int num = 0;
			checked
			{
				if (focusedButton != null)
				{
					num = buttonList.IndexOf(focusedButton) + 1;
					if (num >= buttonList.Count)
					{
						num = 0;
					}
					if (buttonList[num].ActualHeight < 1.0)
					{
						num++;
						if (num >= buttonList.Count)
						{
							num = 0;
						}
					}
				}
				this.ChangeGamepadFocusTo(buttonList[num], ref focusedButton);
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004870 File Offset: 0x00002A70
		private void ChangeGamepadFocusTo(Button button, ref Button focusedButton)
		{
			if (focusedButton != null)
			{
				Application.Current.Dispatcher.BeginInvoke(new Action<Button>(delegate(Button btn)
				{
					btn.RaiseEvent(new MouseEventArgs(global::System.Windows.Input.Mouse.PrimaryDevice, 0)
					{
						RoutedEvent = global::System.Windows.Input.Mouse.MouseLeaveEvent
					});
				}), new object[] { focusedButton });
			}
			focusedButton = button;
			Application.Current.Dispatcher.BeginInvoke(new Action<Button>(delegate(Button btn)
			{
				btn.RaiseEvent(new MouseEventArgs(global::System.Windows.Input.Mouse.PrimaryDevice, 0)
				{
					RoutedEvent = global::System.Windows.Input.Mouse.MouseEnterEvent
				});
				if (this.NewButtonFocusedEvents != null)
				{
					this.NewButtonFocusedEvents(btn);
				}
			}), new object[] { focusedButton });
		}

		// Token: 0x04000019 RID: 25
		private bool bUsingGamepad;

		// Token: 0x0400001A RID: 26
		private Grid lastKnownFocusedGrid;

		// Token: 0x0400001B RID: 27
		private bool bUpdatedFocusedGrid;

		// Token: 0x0400001C RID: 28
		private List<Button> buttonList;

		// Token: 0x0400001D RID: 29
		private Button focusedButton;

		// Token: 0x0400001E RID: 30
		private List<KeyValuePair<PlayerIndex, List<Buttons>>> PressedButtons;

		// Token: 0x0400001F RID: 31
		private List<Keys> PressedKeys;

		// Token: 0x04000020 RID: 32
		private Dictionary<Buttons, List<Buttons>> SupportedGamepadButtons;

		// Token: 0x0200000B RID: 11
		// (Invoke) Token: 0x06000052 RID: 82
		public delegate void OnFocusedButtonChanged(Button btn);
	}
}
