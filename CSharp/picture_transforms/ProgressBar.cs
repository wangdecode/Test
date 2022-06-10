using System;

namespace test
{
    class ProgressBar
    {
        int Left=0, Top=0, Pos=-1, Width=50;

        public ProgressBar()
        {
            Left=Console.CursorLeft;
            Top=Console.CursorTop;
            Init(Left, Top);
        }
        
        public void Init(int left, int top)
        {
            // 清空显示区域
            Console.SetCursorPosition(left, top);
            for (int i = left; ++i < Console.WindowWidth;) { Console.Write(" "); }
            // 绘制进度条背景
            Console.SetCursorPosition(left, top);
            Console.Write("[");  
            Console.SetCursorPosition(left + Width-1, top);
            Console.Write("]");
        }
        
        public void Dispaly(int pos)
        {
            if (Pos != pos)
            {
                Pos = pos;
                // 绘制进度条
                Console.SetCursorPosition(Left+1, Top);
                Console.Write(new string('*', (int)Math.Round(Pos / (100.0 / (Width - 2)))));
                // 显示百分比
                Console.SetCursorPosition(Left + Width + 1, Top);
                Console.Write("{0}%", Pos);
            }
        } 
    }
}
