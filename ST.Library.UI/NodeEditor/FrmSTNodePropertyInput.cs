using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using ST.Library.UI.NodeEditor;
using SkiaSharp;

namespace ST.Library.UI
{
    internal class FrmSTNodePropertyInput : Form
    {
        private STNodePropertyDescriptor m_descriptor;
        private Rectangle m_rect;
        private TextBox m_tbx;

        public FrmSTNodePropertyInput(STNodePropertyDescriptor descriptor) {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_rect = descriptor.RectangleR;
            m_descriptor = descriptor;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = descriptor.Control.AutoColor ? descriptor.Node.TitleColor : descriptor.Control.ItemSelectedColor;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Point pt = m_descriptor.Control.PointToScreen(m_rect.Location);
            pt.Y += m_descriptor.Control.ScrollOffset;
            this.Location = pt;
            this.Size = new System.Drawing.Size(m_rect.Width + m_rect.Height, m_rect.Height);

            m_tbx = new TextBox();
            m_tbx.Font = m_descriptor.Control.Font;
            m_tbx.ForeColor = m_descriptor.Control.ForeColor;
            m_tbx.BackColor = Color.FromArgb(255, m_descriptor.Control.ItemValueBackColor);
            m_tbx.BorderStyle = BorderStyle.None;

            m_tbx.Size = new Size(this.Width - 4 - m_rect.Height, this.Height - 2);
            m_tbx.Text = m_descriptor.GetStringFromValue();
            this.Controls.Add(m_tbx);
            m_tbx.Location = new Point(2, (this.Height - m_tbx.Height) / 2);
            m_tbx.SelectAll();
            m_tbx.LostFocus += (s, ea) => this.Close();
            m_tbx.KeyDown += new KeyEventHandler(tbx_KeyDown);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            var strokeColor = SkiaDrawingHelper.ToSKColor(m_descriptor.Control.ForeColor);
            var inputBackColor = m_tbx == null ? this.BackColor : m_tbx.BackColor;
            var fillColor = SkiaDrawingHelper.ToSKColor(inputBackColor);
            SkiaDrawingHelper.RenderToGraphics(e.Graphics, this.Size, canvas => {
                using (var fill = new SKPaint { Color = fillColor, Style = SKPaintStyle.Fill, IsAntialias = true })
                using (var stroke = new SKPaint { Color = strokeColor, Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true }) {
                    canvas.DrawRect(1, 1, this.Width - 2 - m_rect.Height, this.Height - 2, fill);
                    canvas.DrawPoints(SKPointMode.Polygon, new[] {
                        new SKPoint(this.Width - 21, this.Height - 2),
                        new SKPoint(this.Width - 14, this.Height - 2),
                        new SKPoint(this.Width - 14, this.Height - 8)
                    }, fill);

                    void Line(float x1, float y1, float x2, float y2) => canvas.DrawLine(x1, y1, x2, y2, stroke);
                    Line(this.Width - 14, this.Height - 3, this.Width - 4, this.Height - 3);
                    Line(this.Width - 4, this.Height - 3, this.Width - 4, 14);
                    Line(this.Width - 8, 13, this.Width - 4, 13);
                    Line(this.Width - 19, 11, this.Width - 4, 11);
                    Line(this.Width - 19, 3, this.Width - 16, 3);
                    Line(this.Width - 19, 6, this.Width - 16, 6);
                    Line(this.Width - 19, 9, this.Width - 16, 9);
                    Line(this.Width - 19, 3, this.Width - 19, 9);
                    Line(this.Width - 13, 3, this.Width - 10, 3);
                    Line(this.Width - 13, 6, this.Width - 10, 6);
                    Line(this.Width - 13, 9, this.Width - 10, 9);
                    Line(this.Width - 13, 3, this.Width - 13, 6);
                    Line(this.Width - 10, 6, this.Width - 10, 9);
                    Line(this.Width - 7, 3, this.Width - 4, 3);
                    Line(this.Width - 7, 9, this.Width - 4, 9);
                    Line(this.Width - 7, 3, this.Width - 7, 9);
                }
            });
        }

        void tbx_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) this.Close();
            if (e.KeyCode != Keys.Enter) return;
            try {
                m_descriptor.SetValue(((TextBox)sender).Text, null);
                m_descriptor.Control.Invalidate();//add rect;
            } catch (Exception ex) {
                m_descriptor.OnSetValueError(ex);
            }
            this.Close();
        }

        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // FrmSTNodePropertyInput
            // 
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Name = "FrmSTNodePropertyInput";
            this.ResumeLayout(false);
        }
    }
}
