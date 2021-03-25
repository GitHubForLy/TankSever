using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankSever
{
    public class ListBoxEx : ListBox
    {
        public ListBoxEx()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || Items.Count <= 0)
                return;
            e.DrawBackground();

            if (Items[e.Index] is ListBoxItem)
                e.Graphics.DrawString(Items[e.Index].ToString(), Font,
                    new SolidBrush((Items[e.Index] as ListBoxItem).TextColor), e.Bounds);
            else
                e.Graphics.DrawString(Items[e.Index].ToString(), Font,
                    new SolidBrush(e.ForeColor), e.Bounds);
        }
    }




    /// <summary>
    /// 表示listbox中的项
    /// </summary>
    class ListBoxItem
    {
        /// <summary>
        /// 显示文本的颜色
        /// </summary>
        public Color TextColor
        {
            get;
            set;
        }
        /// <summary>
        /// 显示的文字
        /// </summary>
        public string Text
        {
            get;
            set;
        }
        public override string ToString()
        {
            return Text;
        }
    }
}
