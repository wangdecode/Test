using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace modi_ocr
{
    public partial class Form2 : Form
    {
        Point offset;
        public Form2()
        {
            InitializeComponent();
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;
            Point cur = this.PointToScreen(e.Location);
            offset = new Point(cur.X - this.Left, cur.Y - this.Top);
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;
            Point cur = MousePosition; this.Location = new Point(cur.X - offset.X, cur.Y - offset.Y);
        }
    }
}
