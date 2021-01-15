using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiscAcessDeny
{
    public partial class Maze : Form
    {
        public Maze()
        {
            InitializeComponent();
            current_control = Controls[0]; Controls[0].Select();
            current_control.KeyDown += new KeyEventHandler(CC_KeyDown);
        }

        private void Maze_Load(object sender, EventArgs e)
        {
            Main_start();
            Generate_maze();
            me_pos.X = maze_input;
            Draw_maze();
        }

        static System.Security.Cryptography.RNGCryptoServiceProvider generate = new System.Security.Cryptography.RNGCryptoServiceProvider();
        public static ushort Generate_random(ushort min_val, ushort max_val)
        {
            byte[] b1 = new byte[2];
            generate.GetBytes(b1);
            max_val++;
            ushort result = (ushort)Math.Round((decimal)(BitConverter.ToUInt16(b1, 0) * (max_val - min_val) / 65535 + min_val), MidpointRounding.ToEven);
            if (result > max_val) return (ushort)(result - 1);
            else return result;
        }
        public void Main_start()
        {
            for (short i = 0; i < 25; i++)
            {
                for (short i1 = 0; i1 < 25; i1++)
                {
                    maze_map[i, i1] = 1;
                }
            }
            for (short i = 0; i < 25; i++)
            {
                maze_map[i, 0] = 0;
                maze_map[i, 24] = 0;
            }
            for (short i = 1; i < 24; i++)
            {
                maze_map[0, i] = 0;
                maze_map[24, i] = 0;
            }
            maze_input = Generate_random(1, 23);
            maze_map[maze_input, 1] = 2;
            maker_position.X = maze_input; maker_position.Y = 1;
            maze_output = 0;
        }
        
        struct maze_point
        {
            public maze_point(short x, short y)
            {
                this.x = x; this.y = y;
            }
            public short x, y;
        }
        struct maze_neighbours_result
        {
            public maze_neighbours_result(short[] val)
            {
                this.val = val;
            }
            public short[] val;
        }

        void Generate_maze()
        {
            System.Collections.Stack maker_way = new System.Collections.Stack();
            Point current_position = new Point(maze_input, 1);
            bool make = true;
            while (make)
            {
                maze_neighbours_result neighbours_result = Scan_maze_for_neighbours(current_position);
                if (neighbours_result.val.Length == 0)
                {
                    if (maker_way.Count == 0)
                    {
                        make = false;
                        Set_output();
                    }
                    else
                    {
                        maze_point back_point = (maze_point)maker_way.Pop();
                        current_position.X = back_point.x; current_position.Y = back_point.y;
                    }
                }
                else
                {
                    byte r_val = (byte)Generate_random(0, (ushort)(neighbours_result.val.Length - 1));
                    maker_way.Push(new maze_point((short)current_position.X, (short)current_position.Y));
                    if (neighbours_result.val[r_val] == 2)
                    {
                        current_position.Y++;
                        maze_map[current_position.X, current_position.Y] = 2;
                    }
                    else if (neighbours_result.val[r_val] == 4)
                    {
                        current_position.X--;
                        maze_map[current_position.X, current_position.Y] = 2;
                    }
                    else if (neighbours_result.val[r_val] == 6)
                    {
                        current_position.X++;
                        maze_map[current_position.X, current_position.Y] = 2;
                    }
                    else if (neighbours_result.val[r_val] == 8)
                    {
                        current_position.Y--;
                        maze_map[current_position.X, current_position.Y] = 2;
                    }
                }
            }

        }
        void Set_output()
        {
            ushort x = Generate_random(1, 23); short rad = 0;
            while (maze_map[x + rad, 23] != 2 && maze_map[x - rad, 23] != 2)
            {
                rad++;
            }
            if (rad > 0)
            {
                byte pos = (byte)Generate_random(0, 1);
                if (pos == 0) { maze_output = (ushort)(x + rad); } else { maze_output = (ushort)(x - rad); }
            }
            else { maze_output = x; }
        }
        maze_neighbours_result Scan_maze_for_neighbours(Point current_position)
        {
            short[] val = new short[0];
            if (maze_map[current_position.X + 1, current_position.Y] == 1)
            {
                if (Scan_maze_fragment(current_position, 6))
                {
                    Array.Resize(ref val, val.Length + 1);
                    val[val.Length - 1] = 6;
                }
            }
            if (maze_map[current_position.X - 1, current_position.Y] == 1)
            {
                if (Scan_maze_fragment(current_position, 4))
                {
                    Array.Resize(ref val, val.Length + 1);
                    val[val.Length - 1] = 4;
                }
            }
            if (maze_map[current_position.X, current_position.Y + 1] == 1)
            {
                if (Scan_maze_fragment(current_position, 2))
                {
                    Array.Resize(ref val, val.Length + 1);
                    val[val.Length - 1] = 2;
                }
            }
            if (maze_map[current_position.X, current_position.Y - 1] == 1)
            {
                if (Scan_maze_fragment(current_position, 8))
                {
                    Array.Resize(ref val, val.Length + 1);
                    val[val.Length - 1] = 8;
                }
            }
            return new maze_neighbours_result(val);
        }
        bool Scan_maze_fragment(Point current_position, byte orientation)
        {
            if (orientation == 8)
            {
                if (maze_map[current_position.X, current_position.Y - 2] != 2 && maze_map[current_position.X - 1, current_position.Y - 1] != 2 && maze_map[current_position.X + 1, current_position.Y - 1] != 2 && maze_map[current_position.X - 1, current_position.Y - 2] != 2 && maze_map[current_position.X + 1, current_position.Y - 2] != 2)
                {
                    return true;
                }
            }
            else if (orientation == 4)
            {
                if (maze_map[current_position.X - 2, current_position.Y] != 2 && maze_map[current_position.X - 1, current_position.Y - 1] != 2 && maze_map[current_position.X - 1, current_position.Y + 1] != 2 && maze_map[current_position.X - 2, current_position.Y - 1] != 2 && maze_map[current_position.X - 2, current_position.Y + 1] != 2)
                {
                    return true;
                }
            }
            else if (orientation == 6)
            {
                if (maze_map[current_position.X + 2, current_position.Y] != 2 && maze_map[current_position.X + 1, current_position.Y - 1] != 2 && maze_map[current_position.X + 1, current_position.Y + 1] != 2 && maze_map[current_position.X + 2, current_position.Y - 1] != 2 && maze_map[current_position.X + 2, current_position.Y + 1] != 2)
                {
                    return true;
                }
            }
            else if (orientation == 2)
            {
                if (maze_map[current_position.X, current_position.Y + 2] != 2 && maze_map[current_position.X - 1, current_position.Y + 1] != 2 && maze_map[current_position.X + 1, current_position.Y + 1] != 2 && maze_map[current_position.X - 1, current_position.Y + 2] != 2 && maze_map[current_position.X + 1, current_position.Y + 2] != 2)
                {
                    return true;
                }
            }
            return false;
        }

        byte[,] maze_map = new byte[25, 25]; ushort maze_input = 0, maze_output = 0; Point maker_position;

        Control current_control;
       
        private void CC_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue.ToString())
            {
                case "38":
                    if (maze_map[me_pos.X, me_pos.Y - 1] == 2)
                    {
                        me_pos.Y--;
                        Draw_step_maze();
                    }
                    break;
                case "40":
                    if (maze_map[me_pos.X, me_pos.Y + 1] == 2)
                    {
                        me_pos.Y++;
                        Draw_step_maze();
                    }
                    break;
                case "37":
                    if (maze_map[me_pos.X - 1, me_pos.Y] == 2)
                    {
                        me_pos.X--;
                        Draw_step_maze();
                    }
                    break;
                case "39":
                    if (maze_map[me_pos.X + 1, me_pos.Y] == 2)
                    {
                        me_pos.X++;
                        Draw_step_maze();
                    }
                    break;
            }
        }
        void Draw_step_maze()
        {
            Bitmap bmp1 = new Bitmap(bmp);
            Graphics g = Graphics.FromImage(bmp1);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
            g.FillRectangle(Brushes.Blue, me_pos.X * 10, me_pos.Y * 10, 10, 10);
            pictureBox1.Image = bmp1;
            if (me_pos.Y == 23 && me_pos.X == maze_output)
            {
                DialogResult = DialogResult.OK;
                no_close = false;
                Close();
            }
        }
        Bitmap bmp; Point me_pos = new Point(0,1); bool no_close = true;

        private void Maze_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (no_close)
            {
                if (MessageBox.Show("Вы уверены, что хотите выйти? Если Вы не пройдёте лаберинт, то не сможете отключить защиту!\nНажмите \"Да\" для продолжения выхода.", "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        void Draw_maze()
        {
            bmp = new Bitmap(250, 250, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            Graphics g = Graphics.FromImage(bmp);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
            for (short i = 0; i < 25; i++)
            {
                for (short i1 = 0; i1 < 25; i1++)
                {
                    if (maze_map[i, i1] == 2)
                    {
                        g.FillRectangle(Brushes.LightGray, i * 10, i1 * 10, 10, 10);
                    }
                    else if (maze_map[i, i1] == 1)
                    {
                        g.FillRectangle(Brushes.DimGray, i * 10, i1 * 10, 10, 10);
                    }
                    else if (maze_map[i, i1] == 0)
                    {
                        g.FillRectangle(Brushes.Black, i * 10, i1 * 10, 10, 10);
                    }
                }
            }
            g.FillRectangle(Brushes.Green, maze_output * 10, 230, 10, 10);
            Draw_step_maze();
        }
    }
}
