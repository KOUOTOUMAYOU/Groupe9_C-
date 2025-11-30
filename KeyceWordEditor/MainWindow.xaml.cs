using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using KeyceWordEditor.Models;
using KeyceWordEditor.Services;
using KeyceWordEditor.Dialogs;

namespace KeyceWordEditor
{
    public partial class MainWindow : Window
    {
        private double currentZoom = 1.0;
        private string currentFileName = string.Empty;
        private PageManager? pageManager;
        private ColorPickerService? colorPicker;
        private DocxExportService? docxService;
        private PdfExportService? pdfService;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            InitializeEditor();
            InitializeRulers();
            UpdateStatusBar();
        }

        private void InitializeServices()
        {
            pageManager = new PageManager(Editor);
            colorPicker = new ColorPickerService();
            docxService = new DocxExportService();
            pdfService = new PdfExportService();
        }

        private void InitializeEditor()
        {
            if (Editor == null || FontFamilyComboBox == null)
                return;

            try
            {
                var fonts = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
                foreach (var font in fonts)
                {
                    FontFamilyComboBox.Items.Add(font.Source);
                }
                FontFamilyComboBox.SelectedItem = "Calibri";

                Editor.TextChanged += (s, e) =>
                {
                    UpdateStatusBar();
                    pageManager?.CheckAndAddPage();
                };
                Editor.SelectionChanged += Editor_SelectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'initialisation: {ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeRulers()
        {
            DrawRulerMarks(HorizontalRuler, true);
            DrawRulerMarks(VerticalRuler, false);
        }

        private void DrawRulerMarks(Canvas ruler, bool isHorizontal)
        {
            if (ruler == null) return;

            const double pixelsPerCm = 37.795; // Approximation pour 96 DPI
            const int maxCm = 50;

            for (int cm = 0; cm <= maxCm; cm++)
            {
                double position = cm * pixelsPerCm;

                // Grande marque tous les cm
                Line majorLine = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                if (isHorizontal)
                {
                    majorLine.X1 = position;
                    majorLine.X2 = position;
                    majorLine.Y1 = 20;
                    majorLine.Y2 = 29;
                }
                else
                {
                    majorLine.X1 = 20;
                    majorLine.X2 = 29;
                    majorLine.Y1 = position;
                    majorLine.Y2 = position;
                }

                ruler.Children.Add(majorLine);

                // Numéros
                if (cm % 5 == 0 && cm > 0)
                {
                    TextBlock text = new TextBlock
                    {
                        Text = cm.ToString(),
                        FontSize = 8,
                        Foreground = Brushes.Black
                    };

                    if (isHorizontal)
                    {
                        Canvas.SetLeft(text, position - 5);
                        Canvas.SetTop(text, 5);
                    }
                    else
                    {
                        Canvas.SetLeft(text, 5);
                        Canvas.SetTop(text, position - 5);
                    }

                    ruler.Children.Add(text);
                }

                // Petites marques (demi-cm)
                if (cm < maxCm)
                {
                    Line minorLine = new Line
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 0.5
                    };

                    double halfPosition = position + (pixelsPerCm / 2);

                    if (isHorizontal)
                    {
                        minorLine.X1 = halfPosition;
                        minorLine.X2 = halfPosition;
                        minorLine.Y1 = 24;
                        minorLine.Y2 = 29;
                    }
                    else
                    {
                        minorLine.X1 = 24;
                        minorLine.X2 = 29;
                        minorLine.Y1 = halfPosition;
                        minorLine.Y2 = halfPosition;
                    }

                    ruler.Children.Add(minorLine);
                }
            }
        }

        private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (HorizontalRuler != null && VerticalRuler != null)
            {
                // Déplacer les règles avec le défilement
                Canvas.SetLeft(HorizontalRuler, -e.HorizontalOffset);
                Canvas.SetTop(VerticalRuler, -e.VerticalOffset);
            }
        }

        private void Editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateFormattingUI();
        }

        private void UpdateFormattingUI()
        {
            if (Editor == null)
                return;

            try
            {
                var selection = Editor.Selection;
                if (selection != null)
                {
                    var currentFont = selection.GetPropertyValue(TextElement.FontFamilyProperty);
                    if (currentFont != DependencyProperty.UnsetValue)
                        FontFamilyComboBox.SelectedItem = ((FontFamily)currentFont).Source;

                    var currentSize = selection.GetPropertyValue(TextElement.FontSizeProperty);
                    if (currentSize != DependencyProperty.UnsetValue)
                        FontSizeComboBox.Text = ((double)currentSize).ToString();
                }
            }
            catch
            {
                // Ignorer les erreurs pendant l'initialisation
            }
        }

        private void UpdateStatusBar()
        {
            if (Editor == null || WordCountText == null || CharCountText == null || PageInfoText == null)
                return;

            try
            {
                var text = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd).Text;
                var wordCount = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                var charCount = text.Length;

                WordCountText.Text = $"Mots: {wordCount}";
                CharCountText.Text = $"Caractères: {charCount}";
                PageInfoText.Text = $"Page: {pageManager?.GetCurrentPage() ?? 1}";
            }
            catch
            {
                // Ignorer les erreurs pendant l'initialisation
            }
        }

        #region Gestion des Fichiers
        private void NewDocument_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Blocks.Clear();
            currentFileName = string.Empty;
            StatusText.Text = "Nouveau document créé";
            UpdateStatusBar();
        }

        private void OpenDocument_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Documents texte (*.txt;*.rtf)|*.txt;*.rtf|Tous les fichiers (*.*)|*.*",
                Title = "Ouvrir un document"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    TextRange range = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
                    using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        if (openFileDialog.FileName.EndsWith(".rtf"))
                            range.Load(fs, DataFormats.Rtf);
                        else
                            range.Load(fs, DataFormats.Text);
                    }
                    currentFileName = openFileDialog.FileName;
                    StatusText.Text = $"Ouvert: {GetFileName(currentFileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'ouverture: {ex.Message}", "Erreur",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveDocument_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFileName))
            {
                SaveAsDocument_Click(sender, e);
                return;
            }

            SaveToFile(currentFileName);
        }

        private void SaveAsDocument_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Document RTF (*.rtf)|*.rtf|Document texte (*.txt)|*.txt|Document Word (*.docx)|*.docx|Document PDF (*.pdf)|*.pdf",
                DefaultExt = ".rtf",
                Title = "Enregistrer le document"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FileName.EndsWith(".docx"))
                {
                    ExportToDocx_Click(sender, e);
                }
                else if (saveFileDialog.FileName.EndsWith(".pdf"))
                {
                    ExportToPdf_Click(sender, e);
                }
                else
                {
                    currentFileName = saveFileDialog.FileName;
                    SaveToFile(currentFileName);
                }
            }
        }

        private void SaveToFile(string fileName)
        {
            try
            {
                TextRange range = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    if (fileName.EndsWith(".rtf"))
                        range.Save(fs, DataFormats.Rtf);
                    else
                        range.Save(fs, DataFormats.Text);
                }
                StatusText.Text = $"Enregistré: {GetFileName(fileName)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}", "Erreur",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetFileName(string filePath)
        {
            return System.IO.Path.GetFileName(filePath);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Édition
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.CanUndo)
            {
                Editor.Undo();
                StatusText.Text = "Annulation effectuée";
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.CanRedo)
            {
                Editor.Redo();
                StatusText.Text = "Restauration effectuée";
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            Editor.Cut();
            StatusText.Text = "Texte coupé";
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Editor.Copy();
            StatusText.Text = "Texte copié";
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            Editor.Paste();
            StatusText.Text = "Texte collé";
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            Editor.SelectAll();
            StatusText.Text = "Tout sélectionné";
        }

        private void FindReplace_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité Rechercher/Remplacer à implémenter", "Info");
        }
        #endregion

        #region Formatage
        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            var fontWeight = Editor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            Editor.Selection.ApplyPropertyValue(TextElement.FontWeightProperty,
                fontWeight.Equals(FontWeights.Bold) ? FontWeights.Normal : FontWeights.Bold);
        }

        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            var fontStyle = Editor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            Editor.Selection.ApplyPropertyValue(TextElement.FontStyleProperty,
                fontStyle.Equals(FontStyles.Italic) ? FontStyles.Normal : FontStyles.Italic);
        }

        private void Underline_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            var textDecorations = Editor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            Editor.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty,
                textDecorations == TextDecorations.Underline ? null : TextDecorations.Underline);
        }

        private void Strikethrough_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            var textDecorations = Editor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            Editor.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty,
                textDecorations == TextDecorations.Strikethrough ? null : TextDecorations.Strikethrough);
        }

        private void Highlight_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
        }

        private void FontColor_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
        }

        private void BackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightYellow);
        }

        private void FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Editor == null || FontFamilyComboBox.SelectedItem == null)
                return;

            try
            {
                Editor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty,
                    new FontFamily(FontFamilyComboBox.SelectedItem.ToString()));
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Erreur de police: {ex.Message}";
            }
        }

        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Editor == null || FontSizeComboBox.SelectedItem == null)
                return;

            try
            {
                var size = ((ComboBoxItem)FontSizeComboBox.SelectedItem).Content.ToString();
                if (double.TryParse(size, out double fontSize))
                {
                    Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Erreur de taille: {ex.Message}";
            }
        }

        private void AlignLeft_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;
            Editor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
        }

        private void AlignCenter_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;
            Editor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
        }

        private void AlignRight_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;
            Editor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }

        private void AlignJustify_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;
            Editor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);
        }

        private void Style_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Editor == null || StyleComboBox.SelectedItem == null) return;

            var style = ((ComboBoxItem)StyleComboBox.SelectedItem).Content.ToString();
            switch (style)
            {
                case "Titre 1":
                    Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, 24.0);
                    Editor.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    break;
                case "Titre 2":
                    Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, 20.0);
                    Editor.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    break;
                case "Titre 3":
                    Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, 16.0);
                    Editor.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    break;
                case "Citation":
                    Editor.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                    Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightGray);
                    break;
                case "Code":
                    Editor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily("Consolas"));
                    Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightYellow);
                    break;
                default:
                    Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, 12.0);
                    Editor.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    break;
            }
        }
        #endregion

        #region Zoom
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = Math.Min(ZoomSlider.Value + 25, ZoomSlider.Maximum);
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = Math.Max(ZoomSlider.Value - 25, ZoomSlider.Minimum);
        }

        private void ZoomReset_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = 100;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Editor == null || PageContainer == null || ZoomText == null)
                return;

            try
            {
                currentZoom = e.NewValue / 100.0;
                var scaleTransform = new ScaleTransform(currentZoom, currentZoom);
                PageContainer.LayoutTransform = scaleTransform;
                ZoomText.Text = $"{(int)e.NewValue}%";
            }
            catch
            {
                // Ignorer les erreurs pendant l'initialisation
            }
        }
        #endregion

        #region Affichage
        private void PageLayout_Click(object sender, RoutedEventArgs e)
        {
            PageLayoutMenu.IsChecked = true;
            DraftLayoutMenu.IsChecked = false;
            PageContainer.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
            PageContainer.BorderThickness = new Thickness(1);
            StatusText.Text = "Mode Page activé";
        }

        private void DraftLayout_Click(object sender, RoutedEventArgs e)
        {
            PageLayoutMenu.IsChecked = false;
            DraftLayoutMenu.IsChecked = true;
            PageContainer.BorderBrush = Brushes.Transparent;
            PageContainer.BorderThickness = new Thickness(0);
            StatusText.Text = "Mode Brouillon activé";
        }
        #endregion

        #region Pages
        private void AddPage_Click(object sender, RoutedEventArgs e)
        {
            if (PagesContainer == null) return;

            try
            {
                // Créer une nouvelle page
                var newPage = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White,
                    Margin = new Thickness(20),
                    Padding = new Thickness(40),
                    Width = 816,
                    Height = 1056
                };

                var newEditor = new RichTextBox
                {
                    SpellCheck = { IsEnabled = true },
                    AcceptsReturn = true,
                    AcceptsTab = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                    BorderThickness = new Thickness(0),
                    FontSize = 12,
                    FontFamily = new FontFamily("Calibri")
                };

                newPage.Child = newEditor;
                PagesContainer.Children.Add(newPage);

                UpdateStatusBar();
                StatusText.Text = $"Page {PagesContainer.Children.Count} ajoutée";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de page: {ex.Message}",
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertPageBreak_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            var pageBreak = new Paragraph();
            pageBreak.Inlines.Add(new Run("\n=== SAUT DE PAGE ===\n"));
            pageBreak.Foreground = Brushes.Blue;
            pageBreak.FontWeight = FontWeights.Bold;
            pageBreak.TextAlignment = TextAlignment.Center;
            pageBreak.Background = Brushes.LightYellow;
            pageBreak.BorderBrush = Brushes.Blue;
            pageBreak.BorderThickness = new Thickness(0, 1, 0, 1);
            pageBreak.Padding = new Thickness(0, 5, 0, 5);

            Editor.Document.Blocks.Add(pageBreak);
            UpdateStatusBar();
            StatusText.Text = "Saut de page inséré";
        }

        private void InsertNumbering_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité Numérotation à implémenter", "Info");
        }
        #endregion

        #region Insertion - Images redimensionnables et déplaçables
        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Title = "Insérer une image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));

                    var imageContainer = CreateResizableImage(bitmap);

                    var container = new InlineUIContainer(imageContainer, Editor.CaretPosition);

                    StatusText.Text = "Image insérée (cliquez et glissez pour redimensionner)";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'insertion de l'image: {ex.Message}", "Erreur",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Grid CreateResizableImage(BitmapImage bitmap)
        {
            var grid = new Grid
            {
                Width = Math.Min(bitmap.PixelWidth, 400),
                Height = Math.Min(bitmap.PixelHeight, 400),
                Background = Brushes.Transparent
            };

            var image = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Uniform,
                Cursor = Cursors.SizeAll
            };

            grid.Children.Add(image);

            AddResizeThumb(grid, VerticalAlignment.Top, HorizontalAlignment.Left);
            AddResizeThumb(grid, VerticalAlignment.Top, HorizontalAlignment.Right);
            AddResizeThumb(grid, VerticalAlignment.Bottom, HorizontalAlignment.Left);
            AddResizeThumb(grid, VerticalAlignment.Bottom, HorizontalAlignment.Right);

            image.MouseLeftButtonDown += (s, e) =>
            {
                if (s is Image img && img.Parent is Grid parent)
                {
                    e.Handled = true;
                }
            };

            return grid;
        }

        private void AddResizeThumb(Grid parent, VerticalAlignment vAlign, HorizontalAlignment hAlign)
        {
            var thumb = new Thumb
            {
                Width = 10,
                Height = 10,
                Background = Brushes.Blue,
                Opacity = 0.7,
                Cursor = GetResizeCursor(vAlign, hAlign),
                VerticalAlignment = vAlign,
                HorizontalAlignment = hAlign,
                Margin = new Thickness(2)
            };

            thumb.DragDelta += (s, e) =>
            {
                var deltaX = e.HorizontalChange;
                var deltaY = e.VerticalChange;

                var newWidth = parent.Width;
                var newHeight = parent.Height;

                if (hAlign == HorizontalAlignment.Right)
                    newWidth += deltaX;
                else if (hAlign == HorizontalAlignment.Left)
                    newWidth -= deltaX;

                if (vAlign == VerticalAlignment.Bottom)
                    newHeight += deltaY;
                else if (vAlign == VerticalAlignment.Top)
                    newHeight -= deltaY;

                if (newWidth > 50)
                    parent.Width = newWidth;
                if (newHeight > 50)
                    parent.Height = newHeight;
            };

            parent.Children.Add(thumb);
        }

        private Cursor GetResizeCursor(VerticalAlignment vAlign, HorizontalAlignment hAlign)
        {
            if ((vAlign == VerticalAlignment.Top && hAlign == HorizontalAlignment.Left) ||
                (vAlign == VerticalAlignment.Bottom && hAlign == HorizontalAlignment.Right))
                return Cursors.SizeNWSE;
            else
                return Cursors.SizeNESW;
        }

        private void InsertTable_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            var dialog = new TableDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var table = CreateTable(dialog.Rows, dialog.Columns, dialog.BorderColor,
                                          dialog.BackgroundColor, dialog.BorderThickness);

                    var container = new BlockUIContainer(table);
                    Editor.Document.Blocks.Add(container);

                    StatusText.Text = $"Tableau {dialog.Rows}x{dialog.Columns} inséré";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'insertion du tableau: {ex.Message}",
                                  "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Grid CreateTable(int rows, int columns, Color borderColor, Color backgroundColor, double borderThickness)
        {
            var table = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 10)
            };

            // Créer les lignes
            for (int i = 0; i < rows; i++)
            {
                table.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            }

            // Créer les colonnes
            for (int i = 0; i < columns; i++)
            {
                table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            }

            // Remplir le tableau
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var cell = new Border
                    {
                        BorderBrush = new SolidColorBrush(borderColor),
                        BorderThickness = new Thickness(borderThickness),
                        Background = new SolidColorBrush(backgroundColor)
                    };

                    var textBox = new TextBox
                    {
                        BorderThickness = new Thickness(0),
                        Background = Brushes.Transparent,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(5)
                    };

                    cell.Child = textBox;
                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);
                    table.Children.Add(cell);
                }
            }

            return table;
        }

        private void InsertShape_Click(object sender, RoutedEventArgs e)
        {
            if (Editor == null) return;

            var dialog = new ShapeDialog();
            if (dialog.ShowDialog() == true && dialog.CreatedShape != null)
            {
                try
                {
                    // Créer un conteneur redimensionnable pour la forme
                    var shapeContainer = CreateResizableContainer(dialog.CreatedShape);

                    var container = new BlockUIContainer(shapeContainer);
                    Editor.Document.Blocks.Add(container);

                    StatusText.Text = "Forme insérée";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'insertion de la forme: {ex.Message}",
                                  "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Grid CreateResizableContainer(UIElement element)
        {
            var grid = new Grid
            {
                Width = 200,
                Height = 200,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10)
            };

            // Ajouter l'élément
            grid.Children.Add(element);

            // Ajouter les poignées de redimensionnement
            AddResizeThumb(grid, VerticalAlignment.Top, HorizontalAlignment.Left);
            AddResizeThumb(grid, VerticalAlignment.Top, HorizontalAlignment.Right);
            AddResizeThumb(grid, VerticalAlignment.Bottom, HorizontalAlignment.Left);
            AddResizeThumb(grid, VerticalAlignment.Bottom, HorizontalAlignment.Right);

            return grid;
        }
        #endregion

        #region Couleurs avancées
        private void AdvancedFontColor_Click(object sender, RoutedEventArgs e)
        {
            var currentColor = Editor.Selection.GetPropertyValue(TextElement.ForegroundProperty) as SolidColorBrush;
            var newColor = colorPicker?.ShowColorPicker(currentColor?.Color);

            if (newColor.HasValue)
            {
                colorPicker?.ApplyTextColor(Editor, newColor.Value);
                StatusText.Text = "Couleur du texte appliquée";
            }
        }

        private void AdvancedBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            var currentColor = Editor.Selection.GetPropertyValue(TextElement.BackgroundProperty) as SolidColorBrush;
            var newColor = colorPicker?.ShowColorPicker(currentColor?.Color);

            if (newColor.HasValue)
            {
                colorPicker?.ApplyBackgroundColor(Editor, newColor.Value);
                StatusText.Text = "Couleur de fond appliquée";
            }
        }

        private void AdvancedHighlight_Click(object sender, RoutedEventArgs e)
        {
            var newColor = colorPicker?.ShowColorPicker(Colors.Yellow);

            if (newColor.HasValue)
            {
                colorPicker?.ApplyHighlightColor(Editor, newColor.Value);
                StatusText.Text = "Surligneur appliqué";
            }
        }
        #endregion

        #region Export
        private void ExportToDocx_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Document Word (*.docx)|*.docx",
                DefaultExt = ".docx",
                Title = "Exporter en DOCX"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                docxService?.ExportToDocx(Editor.Document, saveFileDialog.FileName);
                StatusText.Text = $"Exporté en DOCX: {GetFileName(saveFileDialog.FileName)}";
            }
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Document PDF (*.pdf)|*.pdf",
                DefaultExt = ".pdf",
                Title = "Exporter en PDF"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                pdfService?.ExportToPdf(Editor.Document, saveFileDialog.FileName);
                StatusText.Text = $"Exporté en PDF: {GetFileName(saveFileDialog.FileName)}";
            }
        }

        private void PrintPreview_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité Aperçu avant impression à implémenter", "Info");
        }

        private void PrintDocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)Editor.Document).DocumentPaginator, "Impression");
                    StatusText.Text = "Document envoyé à l'imprimante";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'impression: {ex.Message}", "Erreur",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Outils
        private void SpellCheck_Click(object sender, RoutedEventArgs e)
        {
            Editor.SpellCheck.IsEnabled = !Editor.SpellCheck.IsEnabled;
            StatusText.Text = Editor.SpellCheck.IsEnabled ? "Vérification orthographique activée" : "Vérification orthographique désactivée";
        }

        private void WordCount_Click(object sender, RoutedEventArgs e)
        {
            var text = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd).Text;
            var wordCount = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var charCount = text.Length;
            var charNoSpaces = text.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Length;

            MessageBox.Show($"Statistiques du document:\n\n" +
                          $"Mots: {wordCount}\n" +
                          $"Caractères (avec espaces): {charCount}\n" +
                          $"Caractères (sans espaces): {charNoSpaces}",
                          "Compteur de mots", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AITools_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité Assistant IA à implémenter", "Info");
        }

        private void VoiceDictation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité Dictée vocale à implémenter", "Info");
        }
        #endregion
    }
}