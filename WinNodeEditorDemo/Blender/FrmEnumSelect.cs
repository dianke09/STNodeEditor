using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows.Forms;
using ST.Library.UI.NodeEditor;
using SkiaSharp;

namespace WinNodeEditorDemo.Blender
{
    /// <summary>
    /// 此类仅演示 作为MixRGB节点的下拉选择框弹出菜单
    /// </summary>
    public class FrmEnumSelect : Form
    {
        private Point m_pt;
        private int m_nWidth;
        private float m_scale;
        private List<object> m_lst = new List<object>();

        public Enum Enum { get; set; }

        private bool m_bClosed;

        public FrmEnumSelect(Enum e, Point pt, int nWidth, float scale) {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            foreach (var v in Enum.GetValues(e.GetType())) m_lst.Add(v);
            this.Enum = e;
            m_pt = pt;
            m_scale = scale;
            m_nWidth = nWidth;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(255, 34, 34, 34);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            this.Location = m_pt;
            this.Width = (int)(m_nWidth * m_scale);
            this.Height = (int)(m_lst.Count * 20 * m_scale);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            SkiaDrawingHelper.RenderToGraphics(e.Graphics, this.Size, canvas => {
                canvas.Scale(m_scale, m_scale);
                using (var text = new SKPaint { Color = SKColors.White, TextSize = Math.Max(10f, this.Font.Size), IsAntialias = true }) {
                    float y = 0;
                    foreach (var v in m_lst) {
                        var metrics = text.FontMetrics;
                        float textY = y + (20 - (metrics.Descent - metrics.Ascent)) / 2 - metrics.Ascent;
                        canvas.DrawText(v.ToString(), 0, textY, text);
                        y += 20;
                    }
                }
            });
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            int nIndex = e.Y / (int)(20 * m_scale);
            if (nIndex >= 0 && nIndex < m_lst.Count) this.Enum = (Enum)m_lst[nIndex];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            m_bClosed = true;
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            if (m_bClosed) return;
            //this.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Close();
        }
    }
}
