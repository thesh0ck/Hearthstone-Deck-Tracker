﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Enums.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;

namespace Hearthstone_Deck_Tracker.Replay
{
	/// <summary>
	/// Interaction logic for ReplayViewer.xaml
	/// </summary>
	public partial class ReplayViewer : Window
	{
		public ReplayViewer()
		{
			InitializeComponent();
		}

		private int _playerController;
		private int _opponentController;
		public void Load(List<ReplayKeyPoint> replay)
		{
			Replay = replay;
			_currentGameState = Replay[0];
			var player = _currentGameState.Data.First(x => x.IsPlayer);
			var opponent = _currentGameState.Data.First(x => x.HasTag(GAME_TAG.PLAYER_ID) && !x.IsPlayer);
            _playerController = player.GetTag(GAME_TAG.CONTROLLER);
			_opponentController = opponent.GetTag(GAME_TAG.CONTROLLER);
			foreach(var kp in Replay)
				ListBoxKeyPoints.Items.Add(kp);
			DataContext = this;
		}

		private void Update()
		{
			DataContext = null;
			DataContext = this;
		}
		public void NextState()
		{
			if(Replay.Count > _index + 1)
			_currentGameState = Replay[++_index];
			Update();
		}
		public List<ReplayKeyPoint> Replay;
		private ReplayKeyPoint _currentGameState;
		private int _index;

		private Entity GetHero(int controller)
		{
			return
				_currentGameState.Data.FirstOrDefault(
				                             x =>
				                             !string.IsNullOrEmpty(x.CardId) && x.CardId.Contains("HERO") &&
				                             x.IsControlledBy(controller)) ?? new Entity();
		}

		public List<ReplayKeyPoint> KeyPoints
		{
			get { return Replay; }
		} 
		public string PlayerHero
		{
			get
			{
				if(_currentGameState == null)
					return string.Empty;
				var cardId = GetHero(_playerController).CardId;
				return cardId == null ? null : Game.GetCardFromId(cardId).Name;
			} 
		}

		public string PlayerHealth
		{
			get
			{
				if(_currentGameState == null)
					return string.Empty;
				var hero = GetHero(_playerController);
                return (hero.GetTag(GAME_TAG.HEALTH) - hero.GetTag(GAME_TAG.DAMAGE)).ToString();
			}
		}

		public string PlayerArmor
		{
			get
			{
				if(_currentGameState == null)
					return string.Empty;
				return GetHero(_playerController).GetTag(GAME_TAG.ARMOR).ToString(); }
		}

		public string PlayerAttack
		{
			get
			{
				if(_currentGameState == null)
					return string.Empty;
				return GetHero(_playerController).GetTag(GAME_TAG.ATK).ToString(); }
		}

		public IEnumerable<string> PlayerHand
		{
			get
			{
				if(_currentGameState == null)
					return null;
				var hand = _currentGameState.Data.Where(x => x.IsInZone(TAG_ZONE.HAND) && x.IsControlledBy(_playerController));
				return hand.Select(x => Game.GetCardFromId(x.CardId).Name);
			}
		}

		public IEnumerable<string> PlayerBoard {
			get
			{
				if(_currentGameState == null)
					return null;
				var board = _currentGameState.Data.Where(x => x.IsInZone(TAG_ZONE.PLAY) && x.IsControlledBy(_playerController) && x.HasTag(GAME_TAG.HEALTH) && !string.IsNullOrEmpty(x.CardId) && !x.CardId.Contains("HERO"));
				return board.Select(x => Game.GetCardFromId(x.CardId).Name + " (" + x.GetTag(GAME_TAG.ATK) + " - " + x.GetTag(GAME_TAG.HEALTH) + ")");
			}
		}
		public string OpponentHero
		{
			get
			{
				if(_currentGameState == null)
					return null;
				var cardId = GetHero(_opponentController).CardId;
				return cardId == null ? null : Game.GetCardFromId(cardId).Name;
			}
		}

		public string OpponentHealth
		{
			get
			{
				if(_currentGameState == null)
					return null;
				var hero = GetHero(_opponentController);
				return (hero.GetTag(GAME_TAG.HEALTH) - hero.GetTag(GAME_TAG.DAMAGE)).ToString();
			}
		}

		public string OpponentArmor
		{
			get
			{
				if(_currentGameState == null)
					return null;
				return GetHero(_opponentController).GetTag(GAME_TAG.ARMOR).ToString(); }
		}

		public string OpponentAttack
		{
			get
			{
				if(_currentGameState == null)
					return null;
				return GetHero(_opponentController).GetTag(GAME_TAG.ATK).ToString(); }
		}

		public IEnumerable<string> OpponentHand
		{
			get
			{
				if(_currentGameState == null)
					return null;
				var hand = _currentGameState.Data.Where(x => x.IsInZone(TAG_ZONE.HAND) && x.IsControlledBy(_opponentController));
				return hand.Select(x => string.IsNullOrEmpty(x.CardId) ? "" :  Game.GetCardFromId(x.CardId).Name);
			}
		}

		public IEnumerable<string> OpponentBoard
		{
			get
			{
				if(_currentGameState == null)
					return null;
				var board = _currentGameState.Data.Where(x => x.IsInZone(TAG_ZONE.PLAY) && x.IsControlledBy(_opponentController) && x.HasTag(GAME_TAG.HEALTH) && !string.IsNullOrEmpty(x.CardId) && !x.CardId.Contains("HERO"));
				return board.Select(x => Game.GetCardFromId(x.CardId).Name + " (" + x.GetTag(GAME_TAG.ATK) + " - " + x.GetTag(GAME_TAG.HEALTH) + ")");
			}
		}

		private void BtnNext_Click(object sender, RoutedEventArgs e)
		{
			NextState();
		}

		private void ListBoxKeyPoints_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_currentGameState = (ReplayKeyPoint)((ListBox)sender).SelectedItem;
			Update();
		}
	}
}