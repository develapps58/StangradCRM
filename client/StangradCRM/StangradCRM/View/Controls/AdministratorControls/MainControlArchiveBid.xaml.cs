﻿/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 01.09.2016
 * Время: 11:32
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Controls.AdministratorControls
{
	/// <summary>
	/// Interaction logic for MainControlArchiveBid.xaml
	/// </summary>
	public partial class MainControlArchiveBid : UserControl
	{
		CollectionViewSource viewSource;
		public MainControlArchiveBid()
		{
			InitializeComponent();

			viewSource = new CollectionViewSource();
			viewSource.Source = BidViewModel.instance().getArchiveCollection();
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Bid bid = e.Item as Bid;
				if(bid == null) return;
				e.Accepted = bid.IsVisible;
			};
			
			DataContext = new
			{
				BidCollection = this.viewSource.View
			};
			
		}
		
		void DgvBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetEquipmentBidSource();
			SetBuyerSource();
		}
		
		private void SetEquipmentBidSource ()
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			Binding binding = new Binding();
			if(bid == null)
			{
				binding.Source = null;
			}
			else 
			{
				binding.Source = bid.EquipmentBidCollection;
			}
			dgvEquipmentBid.SetBinding(DataGrid.ItemsSourceProperty, binding);
			
			if(dgvEquipmentBid.Items.Count > 0)
			{
				dgvEquipmentBid.SelectedIndex = 0;
			}
			SetComplectationSource();
		}
		
		private void SetBuyerSource ()
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			Binding binding = new Binding();
			if(bid == null)
			{
				binding.Source = null;
			}
			else 
			{
				List<Buyer> buyerList = new List<Buyer>();
				Buyer buyer = BuyerViewModel.instance().getById(bid.Id_buyer);
				if(buyer != null)
				{
					buyerList.Add(buyer);
					binding.Source = buyerList;
				}
				else
				{
					binding.Source = null;
				}
			}
			dgvBuyer.SetBinding(DataGrid.ItemsSourceProperty, binding);
		}
		
		void DgvEquipmentBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetComplectationSource();
		}
		
		private void SetComplectationSource ()
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			Binding binding = new Binding();
			if(equipmentBid == null)
			{
				binding.Source = null;
			}
			else
			{
				binding.Source = equipmentBid.ComplectationCollection;
			}
			dgvComplectation.SetBinding(DataGrid.ItemsSourceProperty, binding);
		}
		
		void TbxFastSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			BidViewModel.instance().fastSearch(tbxFastSearch.Text, (TSObservableCollection<Bid>)viewSource.Source);
			viewSource.View.Refresh();
			if(dgvBid.Items.Count > 0)
			{
				dgvBid.SelectedIndex = 0;
			}
			else
			{
				dgvBid.SelectedIndex = -1;
			}
		}
		
		void BtnClearFastSearch_Click(object sender, RoutedEventArgs e)
		{
			tbxFastSearch.Text = "";
		}
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void DgvBid_RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			BidShipmentSaveWindow window = new BidShipmentSaveWindow(bid);
			window.ShowDialog();
			
			viewSource.View.Refresh();
          	dgvBid.CurrentCell = new DataGridCellInfo(row.Item, dgvBid.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void DgvBid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvBid_RowDoubleClick(sender, null);
				e.Handled = true;
			}
		}
		
		void ContextPaymentHistory_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Заявка не выбрана!");
				return;
			}
			PaymentHistoryWindow window = new PaymentHistoryWindow(bid);
			window.ShowDialog();
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			bid.Is_archive = 0;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}
			BidViewModel.instance().UpdateArchive(bid);
		}
		
		void ContextCopy_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			TextBlock textBlock = ((ContextMenu)mi.Parent).PlacementTarget as TextBlock;
			if(textBlock == null) return;
			
			Clipboard.SetText(textBlock.Text);
		}
		
		void ContextBidFiles_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Заявка не выбрана!");
				return;
			}
			BidFilesWindow window = new BidFilesWindow(bid);
			window.ShowDialog();
		}
		
	}
}