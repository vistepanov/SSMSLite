﻿using System.Windows;

namespace SsmsLite.Core.Ui.Controls.DropMenu
{
    /// <summary>
    /// Interaction logic for DropMenuHeader.xaml
    /// </summary>
    public partial class DropMenuHeader
    {
        public static readonly DependencyProperty ItemNameProperty =
             DependencyProperty.Register(
            "ItemName",
            typeof(string),
            typeof(DropMenuHeader),
            new PropertyMetadata(null));

        public string ItemName
        {
            get => (string)GetValue(ItemNameProperty);
            set => SetValue(ItemNameProperty, value);
        }

        public DropMenuHeader()
        {
            InitializeComponent();
        }
    }
}
