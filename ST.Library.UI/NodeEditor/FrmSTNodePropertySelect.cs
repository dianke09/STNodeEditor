using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using SkiaSharp;

namespace ST.Library.UI.NodeEditor
{
    internal class FrmSTNodePropertySelect : Form
    {
        private STNodePropertyDescriptor m_descriptor;
        private int m_nItemHeight = 25;

        private static Type m_t_bool = typeof(bool);
        private Color m_clr_item_1 = Color.FromArgb(10, 0, 0, 0);// Color.FromArgb(255, 40, 40, 40);
        private Color m_clr_item_2 = Color.FromArgb(10, 255, 255, 255);// Color.FromArgb(255, 50, 50, 50);
        private object m_item_hover;

        public FrmSTNodePropertySelect(STNodePropertyDescriptor descriptor) {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_descriptor = descriptor;
            this.Size = descriptor.RectangleR.Size;
            this.ShowInTaskbar = false;
            this.BackColor = descriptor.Control.BackColor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private List<object> m_lst_item = new List<object>();

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Point pt = m_descriptor.Control.PointToScreen(m_descriptor.RectangleR.Location);
            pt.Y += m_descriptor.Control.ScrollOffset;
            this.Location = pt;
            if (m_descriptor.PropertyInfo.PropertyType.IsEnum) {
                foreach (var v in Enum.GetValues(m_descriptor.PropertyInfo.PropertyType)) m_lst_item.Add(v);
            } else if (m_descriptor.PropertyInfo.PropertyType == m_t_bool) {
                m_lst_item.Add(true);
                m_lst_item.Add(false);
            } else {
                this.Close();
                return;
            }
            this.Height = m_lst_item.Count * m_nItemHeight;
            Rectangle rect = Screen.GetWorkingArea(this);
            if (this.Bottom > rect.Bottom) this.Top -= (this.Bottom - rect.Bottom);
            this.MouseLeave += (s, ea) => this.Close();
            this.LostFocus += (s, ea) => this.Close();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            var borderColor = SkiaDrawingHelper.ToSKColor(m_descriptor.Control.AutoColor ? m_descriptor.Node.TitleColor : m_descriptor.Control.ItemSelectedColor);
            var textColor = SkiaDrawingHelper.ToSKColor(m_descriptor.Control.ForeColor);
            var selectedColor = SkiaDrawingHelper.ToSKColor(m_descriptor.Control.ItemSelectedColor);
            var hoverColor = SkiaDrawingHelper.ToSKColor(m_descriptor.Control.ItemHoverColor);
            var rowColor1 = SkiaDrawingHelper.ToSKColor(m_clr_item_1);
            var rowColor2 = SkiaDrawingHelper.ToSKColor(m_clr_item_2);
            var fontSize = Math.Max(10f, m_descriptor.Control.Font.Size);

            SkiaDrawingHelper.RenderToGraphics(e.Graphics, this.Size, canvas => {
                using (var fill = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true })
                using (var border = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 1, Color = borderColor, IsAntialias = true })
                using (var text = new SKPaint { Style = SKPaintStyle.Fill, Color = textColor, TextSize = fontSize, IsAntialias = true }) {
                    float y = 0;
                    string strVal = m_descriptor.GetStringFromValue();
                    int nIndex = 0;
                    foreach (var v in m_lst_item) {
                        fill.Color = (nIndex++ % 2 == 0) ? rowColor1 : rowColor2;
                        canvas.DrawRect(0, y, this.Width, m_nItemHeight, fill);
                        if (v == m_item_hover) {
                            fill.Color = hoverColor;
                            canvas.DrawRect(0, y, this.Width, m_nItemHeight, fill);
                        }
                        if (v.ToString() == strVal) {
                            fill.Color = selectedColor;
                            canvas.DrawRect(4, y + 10, 5, 5, fill);
                        }
                        var metrics = text.FontMetrics;
                        float textY = y + (m_nItemHeight - (metrics.Descent - metrics.Ascent)) / 2 - metrics.Ascent;
                        canvas.DrawText(v.ToString(), 10, textY, text);
                        y += m_nItemHeight;
                    }
                    canvas.DrawRect(0, 0, this.Width - 1, this.Height - 1, border);
                }
            });
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            int nIndex = e.Y / m_nItemHeight;
            if (nIndex < 0 || nIndex >= m_lst_item.Count) return;
            var item = m_lst_item[e.Y / m_nItemHeight];
            if (m_item_hover == item) return;
            m_item_hover = item;
            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            this.Close();
            int nIndex = e.Y / m_nItemHeight;
            if (nIndex < 0) return;
            if (nIndex > m_lst_item.Count) return;
            try {
                m_descriptor.SetValue(m_lst_item[nIndex], null);
            } catch (Exception ex) {
                m_descriptor.OnSetValueError(ex);
            }
        }
    }
}
