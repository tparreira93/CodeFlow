﻿using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;
using CodeFlow.ManualOperations;
using CodeFlow.GenioManual;

namespace CodeFlow.CodeUtils
{
    /// <summary>
    /// ManualHighlight places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class ManualHighlight
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        /// <summary>
        /// Adornment brush.
        /// </summary>
        private readonly Brush brush;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen pen;

        List<string> matchFields;

        public List<string> MatchFields { get => matchFields; set => matchFields = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualHighlight"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public ManualHighlight(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            this.layer = view.GetAdornmentLayer("ManualHighlight");

            this.view = view;
            this.view.LayoutChanged += this.OnLayoutChanged;

            // Create the pen and brush to color the box behind the a's
            this.brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            this.brush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            this.pen = new Pen(penBrush, 0.5);
            this.pen.Freeze();
            MatchFields = new List<string>();
            foreach (KeyValuePair<Type, ManualMatchProvider> item in VSCodeManualMatcher.MatchProvider)
            {
                MatchFields.Add(item.Value.MatchBeginnig);
                MatchFields.Add(item.Value.MatchEnd);
            }
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                this.CreateVisuals(line);
            }
        }

        /// <summary>
        /// Adds the scarlet box behind the 'a' characters within the given line
        /// </summary>
        /// <param name="line">Line to add the adornments</param>
        private void CreateVisuals(ITextViewLine line)
        {
            try
            {
                IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;
                TryGetText(view, line, out string text);
                if (text == null)
                    return;
                foreach (string val in MatchFields)
                {
                    if(text.Contains(val))
                    {
                        SnapshotSpan span = new SnapshotSpan(this.view.TextSnapshot, Span.FromBounds(line.Start, line.End));
                        Geometry geometry = textViewLines.GetMarkerGeometry(span);
                        if (geometry != null)
                        {
                            var drawing = new GeometryDrawing(this.brush, this.pen, geometry);
                            drawing.Freeze();

                            var drawingImage = new DrawingImage(drawing);
                            drawingImage.Freeze();

                            var image = new Image
                            {
                                Source = drawingImage,
                            };

                            // Align the image with the top of the bounds of the text geometry
                            Canvas.SetLeft(image, geometry.Bounds.Left);
                            Canvas.SetTop(image, geometry.Bounds.Top);

                            this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
                            break;
                        }
                    }
                }
            }
            catch(Exception)
            { }
        }
        public static bool TryGetText(IWpfTextView textView, ITextViewLine textViewLine, out string text)
        {
            var extent = textViewLine.Extent;
            var bufferGraph = textView.BufferGraph;
            try
            {
                var collection = bufferGraph.MapDownToSnapshot(extent, SpanTrackingMode.EdgeInclusive, textView.TextSnapshot);
                var span = new SnapshotSpan(collection[0].Start, collection[collection.Count - 1].End);
                text = span.GetText();
                return true;
            }
            catch
            {
                text = null;
                return false;
            }
        }
    }
}
