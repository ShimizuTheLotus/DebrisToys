using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DebrisToys.UI.Control
{
    public sealed partial class Card : Microsoft.UI.Xaml.Controls.Control
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(Card),
                new PropertyMetadata(string.Empty, OnTitleChanged));
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(Card),
                new PropertyMetadata(string.Empty, OnDescriptionChanged));
        public static readonly DependencyProperty IsPointOverProperty =
            DependencyProperty.Register(
                nameof(IsPointerOver),
                typeof(bool),
                typeof(Card),
                new PropertyMetadata(false));
        public event RoutedEventHandler? Click;

        public bool IsPointerOver
        {
            get
            {
                return (bool)GetValue(IsPointOverProperty);
            }
            set
            {
                SetValue(IsPointOverProperty, value);
            }
        }
        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as Card;
            var newValue = e.NewValue as string;
            card?._titleTextBlock?.Text = newValue;
        }

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as Card;
            var newValue = e.NewValue as string;
            card?._descriptionTextBlock?.Text = newValue;

            bool isDescriptionEmpty = string.IsNullOrWhiteSpace(card?.Description);
            card?._descriptionTextBlock?.Visibility = isDescriptionEmpty ? Visibility.Collapsed : Visibility.Visible;
            card?._textContentGrid?.VerticalAlignment = isDescriptionEmpty ? VerticalAlignment.Center : VerticalAlignment.Stretch;
        }

        public IconElement? IconElement
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    _iconGrid?.Children.Clear();
                    if (value != null)
                    {
                        _iconGrid?.Children.Add(value);
                    }
                }
            }
        }


        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public UIElement? RightPartElement
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    _rightPartGrid?.Children.Clear();
                    if (value != null)
                    {
                        _rightPartGrid?.Children.Add(value);
                    }
                }
            }
        }

        private Grid? _iconGrid;
        private Grid? _textContentGrid;
        private TextBlock? _titleTextBlock;
        private TextBlock? _descriptionTextBlock;
        private Grid? _rightPartGrid;

        private bool _clearRightPartElementRequestFinished = true;

        public Card()
        {
            DefaultStyleKey = typeof(Card);
            PointerEntered += Card_PointerEntered;
            PointerExited += Card_PointerExited;
            PointerCanceled += Card_PointerCanceled;
            PointerPressed += Card_PointerPressed;
            PointerReleased += Card_PointerReleased;
            this.Tapped += Card_Tapped;
        }

        private void Card_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        private void Card_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (IsPointerOver)
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
            else
            {
                VisualStateManager.GoToState(this, stateName: "Normal", true);
            }
        }

        private void Card_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, stateName: "PointerPressed", true);
        }

        private void Card_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, stateName: "Normal", true);
        }

        private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            IsPointerOver = false;
            VisualStateManager.GoToState(this, stateName: "Normal", true);
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IsPointerOver = true;
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _iconGrid = GetTemplateChild("PART_IconGrid") as Grid;
            _titleTextBlock = GetTemplateChild("PART_TitleTextBlock") as TextBlock;
            _descriptionTextBlock = GetTemplateChild("PART_DescriptionTextBlock") as TextBlock;
            _rightPartGrid = GetTemplateChild("PART_RightAlignedContentGrid") as Grid;
            _textContentGrid = GetTemplateChild("PART_TextContentGrid") as Grid;

            if (_iconGrid != null
            && _titleTextBlock != null
            && _descriptionTextBlock != null
            && _rightPartGrid != null)
            {
                _iconGrid.Children.Clear();
                if (IconElement != null)
                {
                    _iconGrid.Children.Add(IconElement);
                }
                _titleTextBlock.Text = Title;
                _descriptionTextBlock.Text = Description;

                if (RightPartElement != null)
                {
                    _rightPartGrid.Children.Add(RightPartElement);
                }
                bool isDescriptionEmpty = string.IsNullOrWhiteSpace(_descriptionTextBlock.Text);
                _descriptionTextBlock.Visibility = isDescriptionEmpty ? Visibility.Collapsed : Visibility.Visible;
                _textContentGrid?.VerticalAlignment = isDescriptionEmpty ? VerticalAlignment.Center : VerticalAlignment.Stretch;

                if (!_clearRightPartElementRequestFinished)
                {
                    _rightPartGrid.Children.Clear();
                    _clearRightPartElementRequestFinished = true;
                }
            }
        }

        public void ClearRightPartElement()
        {
            if (_rightPartGrid == null)
            {
                _clearRightPartElementRequestFinished = false;
                return;
            }
            _rightPartGrid.Children.Clear();
            RightPartElement = null;
        }
    }
}
